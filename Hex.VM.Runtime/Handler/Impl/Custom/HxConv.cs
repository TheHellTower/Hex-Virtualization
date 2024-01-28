using Hex.VM.Runtime.Util;
using System;

namespace Hex.VM.Runtime.Handler.Impl.Custom
{
    public class HxConv : HxOpCode
    {
        public override void Execute(Context vmContext, HxInstruction instruction)
        {
            var id = (int)instruction.Operand.GetObject();
            object item = vmContext.Stack.Pop().GetObject();

            vmContext.Stack.Push(id switch
            {
                0 => (object)(Convert.ToSingle(item)), // convert to "float". Conv_R4
                1 => (object)(Convert.ToDouble(item)), // convert to "double". Conv_R8
                2 => (object)(Convert.ToInt32(item)), // convert to "int32". Conv_I4
                3 => (object)(Convert.ToInt64(item)), // convert to "int64". Conv_I8
                4 => (object)((Int32)Convert.ToByte(item)), // convert to "unsigned int8" then extends to "int32". Conv_U1
                5 => (object)((Int64)Convert.ToUInt64(item)), // convert to "unsigned int64" then extends to "int64". Conv_U8
                6 => (object)((Int32)Convert.ToUInt16(item)), // convert to "unsigned int16", then extends to "int32". Conv_U2
                7 => (object)((Int32)Convert.ToUInt32(item)), // converts to "unsigned int32", then extends to "int32". Conv_U4
                _ => item //Should not happen, otherwise I or YOU messed up in the converter.. And yes this is a useless comment.
            });

            vmContext.Index++;
        }
    }
}