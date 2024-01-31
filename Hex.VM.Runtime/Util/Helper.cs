using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Hex.VM.Runtime.Util
{
    public class Helper
    {
        internal static byte[] Extract(string resourceName, int key)
        {
            using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes.Select(bb => (byte)(bb ^ key)).ToArray();
            }
        }

        internal static TypeInfo ResolveType(int mdtoken)
        {
            foreach (var module in Assembly.GetEntryAssembly().Modules)
            {
                try
                {
                    return (TypeInfo)module.ResolveType(mdtoken);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        internal static FieldInfo ResolveField(int mdtoken)
        {
            foreach (var module in Assembly.GetEntryAssembly().Modules)
            {
                try
                {
                    return module.ResolveField(mdtoken);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        internal static ConstructorInfo ResolveConstructor(int mdtoken)
        {
            foreach (var module in Assembly.GetEntryAssembly().Modules)
            {
                try
                {
                    return (ConstructorInfo)module.ResolveMethod(mdtoken);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        internal static MethodInfo ResolveMethod(int mdtoken)
        {
            foreach (var module in Assembly.GetEntryAssembly().Modules)
            {
                try
                {
                    return (MethodInfo)module.ResolveMethod(mdtoken);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }
        internal static MethodInfo ResolveMember(int mdtoken)
        {
            foreach (var module in Assembly.GetEntryAssembly().Modules)
            {
                try
                {
                    return (MethodInfo)module.ResolveMember(mdtoken);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        public static int ReadPrefix(string txt, int idx = 0) => int.Parse(txt[idx].ToString());
        public static List<object> GetMethodParameters(Context ctx, MethodBase method)
        {
            var parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
            var parameters = new object[parameterTypes.Length];

            Dictionary<TypeCode, Func<object, object>> conversionMap = new Dictionary<TypeCode, Func<object, object>>
            {
                { TypeCode.Empty, obj => obj },
                { TypeCode.Object, obj => obj },
                { TypeCode.DBNull, obj => DBNull.Value },
                { TypeCode.Boolean, obj => obj is int I32B ? Convert.ToBoolean(I32B) : Convert.ToBoolean(obj) },
                { TypeCode.Char, obj => Convert.ToChar(obj) },
                { TypeCode.SByte, obj => Convert.ToSByte(obj) },
                { TypeCode.Byte, obj => Convert.ToByte(obj) },
                { TypeCode.Int16, obj => Convert.ToInt16(obj) },
                { TypeCode.UInt16, obj => Convert.ToUInt16(obj) },
                { TypeCode.Int32, obj => Convert.ToInt32(obj) },
                { TypeCode.UInt32, obj => Convert.ToUInt32(obj) },
                { TypeCode.Int64, obj => Convert.ToInt64(obj) },
                { TypeCode.UInt64, obj => Convert.ToUInt64(obj) },
                { TypeCode.Single, obj => Convert.ToSingle(obj) },
                { TypeCode.Double, obj => Convert.ToDouble(obj) },
                { TypeCode.Decimal, obj => Convert.ToDecimal(obj) },
                { TypeCode.DateTime, obj => Convert.ToDateTime(obj) },
                { TypeCode.String, obj => Convert.ToString(obj) },
            };

            for (int i = parameterTypes.Length - 1; i >= 0; i--)
            {
                var type = parameterTypes[i];
                var pop = ctx.Stack.Pop();
                var obj = pop?.GetObject();

                if (type.IsByRef)
                    throw new NotSupportedException();

                if (conversionMap.TryGetValue(Type.GetTypeCode(type), out var conversion))
                {
                    parameters[i] = conversion(obj);
                }
                else
                {
                    parameters[i] = obj;
                }
            }

            return parameters.ToList();
        }
        public static Type[] GetParameterTypes(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = new Type[parameters.Length + (method.IsStatic ? 0 : 1)];

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                if (method.IsStatic)
                {
                    parameterTypes[i] = parameters[i].ParameterType;
                }
                else
                {
                    if (i == 0)
                    {
                        parameterTypes[0] = method.IsVirtual ? method.DeclaringType.BaseType : method.DeclaringType;
                    }
                    else
                    {
                        parameterTypes[i] = parameters[i - 1].ParameterType;
                    }
                }
            }
            return parameterTypes;
        }

        public static Delegate CreateProxyMethodCall(MethodBase method)
        {
            List<Type> parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToList();

            if (!method.IsStatic) parameterTypes.Insert(0, method.DeclaringType);

            var returnType = method is MethodInfo ? ((MethodInfo)method).ReturnType : typeof(void);
            var dm = new DynamicMethod(method.Name, returnType, parameterTypes.ToArray(), method.DeclaringType.Module, true);
            var gen = dm.GetILGenerator();

            for (var i = 0; i < parameterTypes.Count; i++)
            {
                if (!method.IsStatic && i == 0 && parameterTypes[0].IsValueType)
                {
                    gen.Emit(OpCodes.Ldarga_S, i);
                }
                else
                {
                    gen.Emit(OpCodes.Ldarg, i);
                }
            }

            if (method is MethodInfo)
            {
                gen.Emit(OpCodes.Call, (MethodInfo)method);
            }
            else
            {
                gen.Emit(OpCodes.Call, (ConstructorInfo)method);
            }
            gen.Emit(OpCodes.Ret);

            return dm.CreateDelegate(Expression.GetDelegateType(parameterTypes.Concat(new Type[] { returnType }).ToArray()));
        }
    }
}