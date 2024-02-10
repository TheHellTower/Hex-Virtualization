using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using Hex.VM.Core.Helper;
using Hex.VM.Core.Protections.Impl.Virtualization.RuntimeProtections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// github.com/hexck
namespace Hex.VM.Core.Protections.Impl.Virtualization
{
    public class Virtualization : IProtection
	{
		public override void Execute(Context context)
		{
            protectRuntime();
            IMethod runVm = context.Module.Import(Context.Instance.theMethod);
            foreach (var type in context.Module.GetTypes().ToArray())
			{
				if (type.FullName.StartsWith(Context.Instance.RTModule.Assembly.Name))
					continue;
				
				Context.Instance.Log.Information($"Virtualizing type: {type.FullName}");
				
				foreach (var method in type.Methods.Where(M => M.HasBody && M.Body.HasInstructions).ToArray())
				{
					string methodName = method.MDToken.ToInt32().ToString();					
					
                    if (method.IsRuntime)
						continue;

                    Context.Instance.Log.Information($"Virtualizing method: {method.FullName}");
					
					var name = Generator.RandomName();
					
					var conv = new Converter(method, name);
					conv.Save();
					
					if (!conv.Compatible)
					{
						Context.Instance.Log.Warning($"Skipped method because of incompatibilities: {method.FullName}");
						continue;
					}
					
					method.Body = new CilBody();

					if (method.Parameters.Count() == 0)
					{
                        method.Body.Instructions.Add(new Instruction(OpCodes.Ldnull));
                    } else
					{
                        method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, method.Parameters.Count));
                        method.Body.Instructions.Add(OpCodes.Newarr.ToInstruction(context.Module.CorLibTypes.Object));
                        for (var i = 0; i < method.Parameters.Count; i++)
                        {
                            method.Body.Instructions.Add(new Instruction(OpCodes.Dup));
                            method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
                            method.Body.Instructions.Add(new Instruction(OpCodes.Ldarg, method.Parameters[i]));
                            method.Body.Instructions.Add(new Instruction(OpCodes.Box, method.Parameters[i].Type.ToTypeDefOrRef()));
                            method.Body.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
                        }
                    }
                    method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, methodName));
                    method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldftn, runVm));
                    method.Body.Instructions.Add(Instruction.Create(OpCodes.Calli, runVm.MethodSig));

                    if (method.HasReturnType)
						method.Body.Instructions.Add(new Instruction(OpCodes.Unbox_Any, method.ReturnType.ToTypeDefOrRef()));
					else
						method.Body.Instructions.Add(OpCodes.Pop.ToInstruction());
					
					method.Body.Instructions.Add(new Instruction(OpCodes.Ret));
					
					method.Body.UpdateInstructionOffsets();

                    Context.Instance.VirtualizedMethods.Add(method.FullName);
                }
			}
            injectRuntime();
        }

        private void injectRuntime()
        {
            var runtimeName = $"{Context.Instance.RTModule.Assembly.Name}.THT";

            #region Inject Runtime DLL
            var opts = new ModuleWriterOptions(Context.Instance.RTModule) { Logger = DummyLogger.NoThrowInstance };

            opts.MetadataOptions.Flags = MetadataFlags.PreserveAll;

            MemoryStream ms = new MemoryStream();
            //...
            Context.Instance.RTModule.Write(ms, opts);

            byte[] array = ms.ToArray();
            Context.Instance.Module.Resources.Add(new EmbeddedResource(runtimeName, Compression.Compress(array), ManifestResourceAttributes.Private));
            #endregion
            #region Inject Runtime DLL Loader
            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(LoadDLLRuntime.VMInitialize).Module);
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(LoadDLLRuntime.VMInitialize).MetadataToken));
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, Context.Instance.Module.GlobalType, Context.Instance.Module);
            MethodDef init = (MethodDef)members.Single(method => method.Name == "InitializeRuntime");
            MethodDef AssemblyResolve = (MethodDef)members.Single(method => method.Name == "CurrentDomain_AssemblyResolve");

            foreach (var variable in AssemblyResolve.Body.Variables)
                variable.Name = Generator.RandomName();

            foreach (IDnlibDef member in members)
                member.Name = Generator.RandomName();

            Context.Instance.Module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Insert(0, new Instruction(OpCodes.Calli, init.MethodSig));
            Context.Instance.Module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Insert(0, new Instruction(OpCodes.Ldftn, init));
            #endregion

            #region Protect Virtualized Methods
            ProxyInteger.Execute(Context.Instance.Module);
            Strings.Execute(Context.Instance.Module);
            CallToCalli.Execute(Context.Instance.Module);
            #endregion
        }

        private void protectRuntime()
        {
            /*Patch RunVM GetCallingAssembly*/
            foreach (var x in Context.Instance.theMethod.Body.Instructions)
                if (x.OpCode == OpCodes.Ldstr && x.Operand.ToString().Contains("{{TExecutingHAssemblyT}}"))
                    x.Operand = Context.Instance.Module.Assembly.FullName;
            /*Patch RunVM GetCallingAssembly*/

            #region Protect Runtime
            Strings.Execute(Context.Instance.RTModule);
            /*CFlow.Execute(Context.Instance.RTModule);
            ProxyInteger.Execute(Context.Instance.RTModule);
            CallToCalli.Execute(Context.Instance.RTModule);*/
            Renamer.Execute(Context.Instance.RTModule); Context.Instance.theMethod.Name = Generator.RandomName();
            #endregion
        }
    }
}