using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Servers
{
    /// <summary>
    /// A settings storage backed by a file
    /// </summary>
    public sealed class FileSettingsStorage : ISettingsStorage
    {
        readonly string fileName;
        public FileSettingsStorage(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            this.fileName = fileName;
        }

        public void Clear()
        {
            this.Save(new Dictionary<string, byte[]>());
        }

        public void Delete(string key)
        {
            var dic = this.Read();
            dic.Remove(key);
            this.Save(dic);
        }

        private Dictionary<string, byte[]> Read()
        {
            var dic = new Dictionary<string, byte[]>();
            var lines = File.Exists(this.fileName) ? File.ReadAllLines(this.fileName) : new string[0];
            foreach (var line in lines)
            {
                var kv = line.Split(":");
                if (kv.Length != 2) continue;
                var key = kv[0].Trim();
                if (key.Length == 0) continue;
                var value = HexEncoding.ToBuffer(kv[1].Trim());
                dic[key] = value;
            }
            return dic;
        }

        private void Save(Dictionary<string, byte[]> dic)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(this.fileName));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using var writer = new StreamWriter(this.fileName);
            foreach (var key in dic.Keys)
            {
                writer.Write(key.Replace(Environment.NewLine, " "));
                writer.Write(": ");
                writer.Write(HexEncoding.ToString(dic[key]));
                writer.WriteLine();
            }
        }

        public string[] GetKeys()
        {
            var dic = this.Read();
            return dic.Keys.ToArray();
        }

        public byte[] Read(string key)
        {
            var dic = this.Read();
            byte[] res;
            if (dic.TryGetValue(key, out res))
                return res;
            return null;
        }

        public void Write(string key, byte[] buffer)
        {
            var dic = this.Read();
            if (buffer == null)
                dic.Remove(key);
            else
                dic[key] = buffer;
            this.Save(dic);
        }
    }
}
