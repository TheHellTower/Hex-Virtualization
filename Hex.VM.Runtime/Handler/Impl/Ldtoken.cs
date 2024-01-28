using Hex.VM.Runtime.Util;

namespace Hex.VM.Runtime.Handler.Impl
{
    public class Ldtoken : HxOpCode
    {
        public override void Execute(Context vmContext, HxInstruction instruction)
        {
            var str = (string)instruction.Operand.GetObject();

            var prefix = Helper.ReadPrefix(str);
            var mdtoken = int.Parse(str.Substring(1));

            vmContext.Stack.Push(prefix switch
            {
                0 => Helper.ResolveMethod(mdtoken).MethodHandle, // Method
                1 => Helper.ResolveMember(mdtoken).MethodHandle, // MemberRef
                2 => Helper.ResolveField(mdtoken).FieldHandle, // IField
                3 => Helper.ResolveType(mdtoken).TypeHandle, // ITypeOrDef
            });

            vmContext.Index++;
        }
    }
}