using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using PcSoft.SaveGame._90_Scripts.Types;
using PcSoft.SaveGame._90_Scripts.Utils;
using UnityEngine;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public abstract class SaveGameSerializer
    {
        private const string DefaultFilename = "SaveGame.dat";

        #region Static Factory

        public static PlainSaveGameSerializer<T> CreatePlain<T>(ulong version, string filename, T defaultSaveGame = default) where T : class
        {
            return new PlainSaveGameSerializer<T>(filename, version, defaultSaveGame);
        }

        public static PlainSaveGameSerializer<T> CreatePlain<T>(ulong version, T defaultSaveGame = default) where T : class
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

        public static CollectionSaveGameSerializer<T> CreateCollection<T>(ulong version, string filename = DefaultFilename) where T : class
        {
            return new CollectionSaveGameSerializer<T>(filename, version);
        }

        public static CollectionSaveGameSerializer CreateCollection(ulong version, string filename = DefaultFilename)
        {
            return new CollectionSaveGameSerializer(filename, version);
        }

        public static SlotSaveGameSerializer<TS, TM> CreateSlot<TS, TM>(ulong version, string filename = DefaultFilename) where TS : SlotData where TM : class
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
            var saveGameFilename = Application.persistentDataPath + "/" + _filename;
            try
            {
                Debug.Log("Try to open save game " + saveGameFilename);
                using (var fileStream = new FileStream(saveGameFilename, FileMode.Open))
                {
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        Load(new SaveGameFormatter(zipStream, _version));
                    }
                }
            }
            catch (IOException)
            {
                Debug.LogWarning("Unable to load save game data from " + saveGameFilename + ", ignore...");
            }
        }

        public AsyncSerialization LoadAsync()
        {
            var op = new AsyncSerialization();
            Task.Run(() =>
            {
                var saveGameFilename = Application.persistentDataPath + "/" + _filename;
                try
                {
                    Debug.Log("Try to open save game " + saveGameFilename);
                    using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Open))
                    {
                        using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            LoadAsync(new SaveGameFormatter(zipStream, _version), op);
                        }
                    }
                }
                catch (IOException)
                {
                    Debug.LogWarning("Unable to load save game data from " + saveGameFilename + ", ignore...");
                }
            });

            return op;
        }

        public void Save()
        {
            var saveGameFilename = Application.persistentDataPath + "/" + _filename;

            Debug.Log("Try to store save game " + saveGameFilename);
            using (var fileStream = new FileStream(saveGameFilename, FileMode.Create))
            {
                using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    var versionBytes = BitConverter.GetBytes(_version);
                    zipStream.Write(versionBytes, 0, versionBytes.Length);

                    Save(new SaveGameFormatter(zipStream, _version));
                }
            }
        }

        public AsyncSerialization SaveAsync()
        {
            var op = new AsyncSerialization();
            Task.Run(() =>
            {
                var saveGameFilename = Application.persistentDataPath + "/" + _filename;

                Debug.Log("Try to store save game " + saveGameFilename);
                using (var fileStream = new FileStream(Application.persistentDataPath + "/" + _filename, FileMode.Create))
                {
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                    {
                        var versionBytes = BitConverter.GetBytes(_version);
                        zipStream.Write(versionBytes, 0, versionBytes.Length);

                        SaveAsync(new SaveGameFormatter(zipStream, _version), op);
                    }
                }
            });

            return op;
        }

        private void Load(SaveGameFormatter formatter)
        {
            LoadAsync(formatter, null);
        }

        protected abstract void LoadAsync(SaveGameFormatter formatter, AsyncSerialization asyncSerialization);

        private void Save(SaveGameFormatter formatter)
        {
            SaveAsync(formatter, null);
        }

        protected abstract void SaveAsync(SaveGameFormatter formatter, AsyncSerialization asyncSerialization);
    }
}