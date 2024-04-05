using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using UnityEngine;

public static class BasicCompressor
{
    /// <summary>
    /// Convert a string to bytes, then call compressBytes.
    /// Then converts to base 64.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CompressString(string str)
    {
        byte[] compressBytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(CompressBytes(compressBytes));
    }

    /// <summary>
    /// Compress bytearray using gzip
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] CompressBytes(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (GZipStream gStream = new GZipStream(stream, System.IO.Compression.CompressionLevel.Optimal))
            {
                gStream.Write(bytes, 0, bytes.Length);
            }
            return stream.ToArray();
        }
    }

    public static byte[] DecompressBytes(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream gStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    gStream.CopyTo(output);
                }
                return output.ToArray();
            }
        }
    }

    public static string DecompressString(string str)
    {
        byte[] decompressBytes = Convert.FromBase64String(str);
        byte[] decompressed = DecompressBytes(decompressBytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}
