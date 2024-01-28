using Hex.VM.Runtime.Util;

// github.com/hexck
namespace Hex.VM.Runtime.Handler
{
    public abstract class HxOpCode
    {
        public abstract void Execute(Context vmContext, HxInstruction instruction);
    }
}