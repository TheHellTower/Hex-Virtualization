using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Hex.VM.Core.Protections.LoadDLLRuntime
{
    public static class VMInitialize
    {
        private static Assembly assembly = null;

        private static Stream manifestResourceStream = null;

        private static byte[] array = null;

        private static MemoryStream input;

        private static MemoryStream output;

        public static void InitializeRuntime() => AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        private static byte[] DecompressData(byte[] data)
        {
            input = new MemoryStream(data);
            output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                dstream.CopyTo(output);

            return output.ToArray();
        }
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Hex.VM.Runtime.THT");
            array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            assembly = Assembly.Load(DecompressData(array));
            return assembly;
        }
    }
}