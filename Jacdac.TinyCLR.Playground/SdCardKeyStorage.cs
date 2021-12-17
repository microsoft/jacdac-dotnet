using GHIElectronics.TinyCLR.Data.Json;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.IO;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Jacdac
{
    [Serializable]
    public sealed class StorageEntry
    {
        public string Key;
        public string Data;
    }

    internal class SdCardKeyStorage : IDisposable
    {
        private StorageController sd;
        private IDriveProvider drive;

        public SdCardKeyStorage()
        {
            this.sd = StorageController.FromName(SC20100.StorageController.SdCard);
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

        public IKeyStorage MountKeyStorage(string fileName)
        {
            return new KeyStorage(this, fileName);
        }

        class KeyStorage : IKeyStorage
        {
            readonly SdCardKeyStorage Parent;
            readonly string FileName;

            public KeyStorage(SdCardKeyStorage parent, string fileName)
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
