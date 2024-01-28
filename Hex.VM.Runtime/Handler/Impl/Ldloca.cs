using Hex.VM.Runtime.Util;

namespace Hex.VM.Runtime.Handler.Impl
{
    public class Ldloca : HxOpCode
    {
        public override void Execute(Context vmContext, HxInstruction instruction)
        {
            var local = vmContext.Locals.Get((int)instruction.Operand.GetObject());
            vmContext.Stack.Push(local);
            vmContext.Index++;
        }
    }
}