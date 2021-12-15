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
    internal sealed class StorageEntry
    {
        public string Key;
        public byte[] Data;
    }

    [Serializable]
    internal sealed class StorageFile
    {
        public StorageEntry[] Entries;
    }

    internal class SdCardKeyStorage : IKeyStorage, IDisposable
    {
        public readonly string FileName;
        private StorageController sd;
        private IDriveProvider drive;

        public SdCardKeyStorage(string fileName)
        {
            this.FileName = fileName;
            this.sd = StorageController.FromName(SC20100.StorageController.SdCard);
            this.drive = FileSystem.Mount(sd.Hdc);
        }

        private string FullFileName(IDriveProvider drive)
        {
            return $@"{drive.Name}{this.FileName}";
        }

        public void Clear()
        {
            try
            {
                var fn = this.FullFileName(drive);
                drive.Delete(fn);
            }
            finally
            {
                FileSystem.Flush(sd.Hdc);
            }

        }

        private StorageFile deserialize(string fn)
        {
            StorageFile file = null;
            try
            {
                var bytes = System.IO.File.ReadAllBytes(fn);
                var text = UTF8Encoding.UTF8.GetString(bytes);
                file = (StorageFile)JsonConverter.DeserializeObject(text, typeof(StorageFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (file == null)
                file = new StorageFile();
            if (file.Entries == null)
                file.Entries = new StorageEntry[0];
            return file;
        }

        private void serialize(string fn, StorageFile file)
        {
            var text = JsonConverter.Serialize(file).ToString();
            var buffer = UTF8Encoding.UTF8.GetBytes(text);
            System.IO.File.WriteAllBytes(fn, buffer);
        }

        public void Delete(string key)
        {
            try
            {
                // load
                var fn = this.FullFileName(drive);
                var file = this.deserialize(fn);

                // delete
                var entries = file.Entries;
                var newEntries = new StorageEntry[entries.Length - 1];
                var k = 0;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    if (entry.Key != key)
                        newEntries[k++] = entry;
                }
                file.Entries = newEntries;

                // save back
                this.serialize(fn, file);
            }
            finally
            {
                FileSystem.Flush(this.sd.Hdc);
            }
        }

        public string[] GetKeys()
        {
            try
            {
                // load
                var fn = this.FullFileName(drive);
                var file = this.deserialize(fn);
                var entries = file.Entries;
                var keys = new string[entries.Length];
                for (var i = 0; i < entries.Length; i++)
                {
                    keys[i] = entries[i].Key;
                }
                return keys;
            }
            finally
            {
                FileSystem.Flush(sd.Hdc);
            }
        }

        public byte[] Read(string key)
        {
            try
            {
                // load
                var fn = this.FullFileName(drive);
                var file = this.deserialize(fn);

                // delete
                var entries = file.Entries;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    if (entry.Key == key)
                        return entry.Data;
                }
            }
            finally
            {
                FileSystem.Flush(this.sd.Hdc);
            }

            return null;
        }

        public void Write(string key, byte[] buffer)
        {
            try
            {
                // load
                var fn = this.FullFileName(drive);
                var file = this.deserialize(fn);

                // update
                var updated = false;
                var entries = file.Entries;
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    if (entry.Key == key)
                    {
                        entry.Data = buffer;
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
                        Data = buffer
                    };
                    file.Entries = newEntries;
                }

                // save back
                this.serialize(fn, file);
            }
            finally
            {
                FileSystem.Flush(this.sd.Hdc);
            }
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
    }
}
