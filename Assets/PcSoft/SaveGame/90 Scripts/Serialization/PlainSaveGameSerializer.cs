using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PcSoft.SaveGame._90_Scripts.Types;
using UnityEngine;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class PlainSaveGameSerializer<T> : SaveGameSerializer
    {
        private T _saveGame;

        public T SaveGame
        {
            get
            {
                lock (_saveGame)
                {
                    return _saveGame;
                }
            }
            set
            {
                lock (_saveGame)
                {
                    _saveGame = value;
                }
            }
        }

        public Func<ulong, T, T> Migrator { get; set; }

        internal PlainSaveGameSerializer(string filename, ulong version, T defaultSaveGame) : base(filename, version)
        {
            SaveGame = defaultSaveGame;
        }

        protected override void LoadAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGame)
            {
                SaveGame = (T) formatter.Deserialize(stream);
            }

            op?.Completed();
        }

        protected override void LoadMigrationAsync(Stream stream, ulong version, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGame)
            {
                var oldSaveGame = (T) formatter.Deserialize(stream);
                SaveGame = Migrator != null ? Migrator.Invoke(version, oldSaveGame) : oldSaveGame;
            }

            op?.Completed();
        }

        protected override void SaveAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGame)
            {
                formatter.Serialize(stream, SaveGame);
            }

            op?.Completed();
        }
    }

    public class PlainSaveGameSerializer : PlainSaveGameSerializer<object>
    {
        internal PlainSaveGameSerializer(string filename, ulong version, object defaultSaveGame) : base(filename, version, defaultSaveGame)
        {
        }
    }
}