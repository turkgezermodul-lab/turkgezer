using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace TurkGezer.Editor
{
    /// <summary>GUID'leri koruyarak, aktif projeye yinelenen assembly sokmadan UnityPackage olusturur.</summary>
    public static class UnityPackageWriter
    {
        public static void Write(string output, Dictionary<string, string> files)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var usedGuids = new HashSet<string>();
            using (var stream = File.Create(output))
            using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
            {
                foreach (var pair in files)
                {
                    string metaPath = pair.Key + ".meta";
                    string meta = File.Exists(metaPath) ? File.ReadAllText(metaPath) : "";
                    var match = Regex.Match(meta, @"(?m)^guid:\s*([a-f0-9]{32})");
                    string guid = match.Success ? match.Groups[1].Value : Guid.NewGuid().ToString("N");
                    if (!usedGuids.Add(guid)) throw new InvalidOperationException("Tekrarlanan asset GUID: " + pair.Key);
                    if (meta.Length == 0) meta = "fileFormatVersion: 2\nguid: " + guid + "\n";
                    Entry(gzip, guid + "/asset", File.ReadAllBytes(pair.Key));
                    Entry(gzip, guid + "/asset.meta", Encoding.UTF8.GetBytes(meta));
                    Entry(gzip, guid + "/pathname", Encoding.UTF8.GetBytes(pair.Value));
                }
                gzip.Write(new byte[1024], 0, 1024);
            }
        }
        private static void Entry(Stream output, string name, byte[] data)
        {
            var header = new byte[512];
            Put(header, 0, 100, name);
            Put(header, 100, 8, "0000644");
            Put(header, 108, 8, "0000000"); Put(header, 116, 8, "0000000");
            Put(header, 124, 12, Convert.ToString(data.LongLength, 8).PadLeft(11, '0'));
            Put(header, 136, 12, Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 8).PadLeft(11, '0'));
            for (int i = 148; i < 156; i++) header[i] = 32;
            header[156] = (byte)'0';
            Put(header, 257, 6, "ustar"); Put(header, 263, 2, "00");
            int checksum = 0; foreach (byte value in header) checksum += value;
            Put(header, 148, 8, Convert.ToString(checksum, 8).PadLeft(6, '0') + "\0 ");
            output.Write(header, 0, header.Length);
            output.Write(data, 0, data.Length);
            int padding = (512 - data.Length % 512) % 512;
            if (padding > 0) output.Write(new byte[padding], 0, padding);
        }
        private static void Put(byte[] header, int offset, int length, string value)
        {
            byte[] data = Encoding.ASCII.GetBytes(value);
            Array.Copy(data, 0, header, offset, Math.Min(length, data.Length));
        }
    }
}
