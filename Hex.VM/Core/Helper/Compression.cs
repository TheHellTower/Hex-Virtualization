using System.IO;
using System.IO.Compression;

namespace Hex.VM.Core.Helper
{
    internal class Compression
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                dstream.Write(data, 0, data.Length);

            return output.ToArray();
        }
    }
}