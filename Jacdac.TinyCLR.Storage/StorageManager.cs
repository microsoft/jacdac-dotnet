using GHIElectronics.TinyCLR.Data.Json;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.IO;
using Jacdac.Servers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Jacdac.Storage
{
    [Serializable]
    public sealed class StorageEntry
    {
        public string Key;
        public string Data;
    }

    public sealed class StorageManager : IDisposable
    {
        private StorageController sd;
        private IDriveProvider drive;

        public StorageManager(StorageController sd)
        {
            this.sd = sd;
            this.drive = FileSystem.Mount(sd.Hdc);
        }

        private void Flush()
        {
            FileSystem.Flush(sd.Hdc);
        }

        public void Dispose()
        {
            if (this.sd != null)
            {
                FileSystem.Unmount(this.sd.Hdc);
                this.drive = null;
                this.sd = null;
            }
        }

        public ISpecificationStorage MountSpecificationStorage(string dirName)
        {
            return new SpecificationStorage(this, dirName);
        }

        class SpecificationStorage : ISpecificationStorage
        {
            readonly StorageManager Parent;
            readonly string DirName;

            public SpecificationStorage(StorageManager parent, string dirName)
            {
                this.Parent = parent;
                this.DirName = dirName;
            }

            public uint[] GetServiceClasses()
            {
                var drive = this.Parent.drive;
                var info = new DirectoryInfo(Path.Combine(drive.Name, this.DirName));
                if (!info.Exists)
                    return new uint[0];

                var files = info.GetFiles();
                var serviceClasses = new uint[files.Length];
                var k = 0;
                foreach (var file in files)
                {
                    var name = file.Name;
                    if (file.Length < "0x.json".Length || file.Extension != ".json" || name.Substring(0, 2) != "0x")
                        continue;
                    var text = name.Substring(2, name.Length - "0x.json".Length);
                    uint serviceClass = (uint)System.Convert.ToInt32(text, 16);
                    serviceClasses[k++] = serviceClass;
                }
                if (k == serviceClasses.Length) return serviceClasses;
                else
                {
                    var res = new uint[k];
                    Array.Copy(serviceClasses, 0, res, 0, k);
                    return res;
                }
            }
            public string Read(uint serviceClass)
            {
                var n = $"0x{serviceClass.ToString("x1")}.json";
                var drive = this.Parent.drive;
                var path = Path.Combine(drive.Name, Path.Combine(this.DirName, n));
                var info = new FileInfo(path);
                if (!info.Exists)
                    return null;

                Debug.WriteLine($"storage: read {path}");
                var bytes = System.IO.File.ReadAllBytes(path);
                var text = System.Text.UTF8Encoding.UTF8.GetString(bytes);
                return text;
            }

            public void Write(uint serviceClass, string spec)
            {
                try
                {
                    var n = $"0x{serviceClass.ToString("x1")}.json";
                    var drive = this.Parent.drive;
                    var dpath = Path.Combine(drive.Name, this.DirName);
                    var dinfo = new DirectoryInfo(dpath);
                    if (!dinfo.Exists)
                    {
                        Debug.WriteLine($"storage: create directory {dpath}");
                        dinfo.Create();
                    }

                    var path = Path.Combine(dpath, n);
                    Debug.WriteLine($"storage: write {path}");
                    var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(spec);
                    System.IO.File.WriteAllBytes(path, bytes);
                }
                finally
                {
                    this.Parent.Flush();
                }
            }

            public void Clear()
            {
                try
                {
                    var drive = this.Parent.drive;
                    var path = Path.Combine(drive.Name, this.DirName);
                    var info = new DirectoryInfo(path);
                    if (info.Exists)
                    {
                        Debug.WriteLine($"storage: clear {path}");
                        info.Delete(true);
                    }
                }
                finally
                {
                    this.Parent.Flush();
                }
            }
        }

        public ISettingsStorage MountSettingsStorage(string fileName)
        {
            return new SettingsStorage(this, fileName);
        }

        class SettingsStorage : ISettingsStorage
        {
            readonly StorageManager Parent;
            readonly string FileName;

            public SettingsStorage(StorageManager parent, string fileName)
            {
                this.Parent = parent;
                this.FileName = fileName;
            }

            private string FullFileName()
            {
                var drive = this.Parent.drive;
                return $@"{drive.Name}{this.FileName}";
            }

            public void Clear()
            {
                try
                {
                    var fn = this.FullFileName();
                    this.Parent.drive.Delete(fn);
                }
                finally
                {
                    this.Parent.Flush();
                }

            }

            private StorageEntry[] deserialize(string fn)
            {
                StorageEntry[] entries = null;
                try
                {
                    var bytes = System.IO.File.ReadAllBytes(fn);
                    var text = UTF8Encoding.UTF8.GetString(bytes);
                    entries = (StorageEntry[])JsonConverter.DeserializeObject(text, typeof(StorageEntry[]),
                        (string instancePath, JToken token, Type baseType, string fieldName, int length) =>
                        {
                            if (instancePath == "/")
                                return new StorageEntry();

                            return null;
                        });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (entries == null)
                    entries = new StorageEntry[0];
                return entries;
            }

            private void serialize(string fn, StorageEntry[] entries)
            {
                var text = JsonConverter.Serialize(entries).ToString();
                var buffer = UTF8Encoding.UTF8.GetBytes(text);
                System.IO.File.WriteAllBytes(fn, buffer);
            }

            public void Delete(string key)
            {
                try
                {
                    // load
                    var fn = this.FullFileName();
                    var entries = this.deserialize(fn);

                    // delete
                    var newEntries = new StorageEntry[entries.Length - 1];
                    var k = 0;
                    for (var i = 0; i < entries.Length; i++)
                    {
                        var entry = entries[i];
                        if (entry.Key != key)
                            newEntries[k++] = entry;
                    }
                    entries = newEntries;

                    // save back
                    this.serialize(fn, entries);
                }
                finally
                {
                    this.Parent.Flush();
                }
            }

            public string[] GetKeys()
            {
                var fn = this.FullFileName();
                var entries = this.deserialize(fn);
                var keys = new string[entries.Length];
                for (var i = 0; i < entries.Length; i++)
                {
                    keys[i] = entries[i].Key;
                }
                return keys;
            }

            public byte[] Read(string key)
            {
                var fn = this.FullFileName();
                var entries = this.deserialize(fn);
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    if (entry.Key == key)
                        return HexEncoding.ToBuffer(entry.Data);
                }
                return null;
            }

            public void Write(string key, byte[] buffer)
            {
                try
                {
                    var data = HexEncoding.ToString(buffer);
                    // load
                    var fn = this.FullFileName();
                    var entries = this.deserialize(fn);

                    // update
                    var updated = false;
                    for (var i = 0; i < entries.Length; i++)
                    {
                        var entry = entries[i];
                        if (entry.Key == key)
                        {
                            entry.Data = data;
                            updated = true;
                        }
                    }

                    if (!updated)
                    {
                        var newEntries = new StorageEntry[entries.Length + 1];
                        entries.CopyTo(newEntries, 0);
                        newEntries[newEntries.Length - 1] = new StorageEntry
                        {
                            Key = key,
                            Data = data
                        };
                        entries = newEntries;
                    }

                    // save back
                    this.serialize(fn, entries);
                }
                finally
                {
                    this.Parent.Flush();
                }
            }
        }
    }
}
