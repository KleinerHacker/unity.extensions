using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using PcSoft.SaveGame._90_Scripts.Types;
using UnityEngine;
using UnityEngine.SceneManagement;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public abstract class SaveGameSerializer
    {
        private const string DefaultFilename = "SaveGame.dat";

        #region Static Factory

        public static PlainSaveGameSerializer<T> CreatePlain<T>(ulong version, string filename, T defaultSaveGame = default)
        {
            return new PlainSaveGameSerializer<T>(filename, version, defaultSaveGame);
        }

        public static PlainSaveGameSerializer<T> CreatePlain<T>(ulong version, T defaultSaveGame = default)
        {
            return new PlainSaveGameSerializer<T>(DefaultFilename, version, defaultSaveGame);
        }

        public static PlainSaveGameSerializer CreatePlain(ulong version, string filename, object defaultSaveGame = default)
        {
            return new PlainSaveGameSerializer(filename, version, defaultSaveGame);
        }

        public static PlainSaveGameSerializer CreatePlain(ulong version, object defaultSaveGame = default)
        {
            return new PlainSaveGameSerializer(DefaultFilename, version, defaultSaveGame);
        }

        public static CollectionSaveGameSerializer<T> CreateCollection<T>(ulong version, string filename = DefaultFilename)
        {
            return new CollectionSaveGameSerializer<T>(filename, version);
        }

        public static CollectionSaveGameSerializer CreateCollection(ulong version, string filename = DefaultFilename)
        {
            return new CollectionSaveGameSerializer(filename, version);
        }

        public static SlotSaveGameSerializer<TS, TM> CreateSlot<TS, TM>(ulong version, string filename = DefaultFilename) where TS : SlotData
        {
            return new SlotSaveGameSerializer<TS, TM>(filename, version);
        }

        public static SlotSaveGameSerializer CreateSlot(ulong version, string filename = DefaultFilename)
        {
            return new SlotSaveGameSerializer(filename, version);
        }

        #endregion

        private readonly string _filename;
        private readonly ulong _version;

        protected SaveGameSerializer(string filename, ulong version)
        {
            _filename = filename;
            _version = version;
        }

        public void Load()
        {
            using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Open))
            {
                using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    var versionBytes = new byte[sizeof(ulong)];
                    zipStream.Read(versionBytes, 0, versionBytes.Length);
                    var version = BitConverter.ToUInt64(versionBytes, 0);

                    if (version < _version)
                    {
                        Debug.Log("Migrate from version " + version);
                        LoadMigration(zipStream, version);

                        return;
                    }

                    Load(zipStream);
                }
            }
        }

        public AsyncSerialization LoadAsync()
        {
            var op = new AsyncSerialization();
            Task.Run(() =>
            {
                using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Open))
                {
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        var versionBytes = new byte[sizeof(ulong)];
                        zipStream.Read(versionBytes, 0, versionBytes.Length);
                        var version = BitConverter.ToUInt64(versionBytes, 0);

                        if (version != _version)
                        {
                            Debug.Log("Migrate from version " + version);
                            LoadMigrationAsync(zipStream, version, op);

                            return;
                        }

                        LoadAsync(zipStream, op);
                    }
                }
            });

            return op;
        }

        public void Save()
        {
            using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Create))
            {
                using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    var versionBytes = BitConverter.GetBytes(_version);
                    zipStream.Write(versionBytes, 0, versionBytes.Length);

                    Save(zipStream);
                }
            }
        }

        public AsyncSerialization SaveAsync()
        {
            var op = new AsyncSerialization();
            Task.Run(() =>
            {
                using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Create))
                {
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                    {
                        var versionBytes = BitConverter.GetBytes(_version);
                        zipStream.Write(versionBytes, 0, versionBytes.Length);

                        SaveAsync(zipStream, op);
                    }
                }
            });

            return op;
        }

        private void Load(Stream stream)
        {
            LoadAsync(stream, null);
        }
        protected abstract void LoadAsync(Stream stream, AsyncSerialization asyncSerialization);

        private void LoadMigration(Stream stream, ulong version)
        {
            LoadMigrationAsync(stream, version, null);
        }
        protected abstract void LoadMigrationAsync(Stream stream, ulong version, AsyncSerialization asyncSerialization);

        private void Save(Stream stream)
        {
            SaveAsync(stream, null);
        }
        protected abstract void SaveAsync(Stream stream, AsyncSerialization asyncSerialization);
    }
}