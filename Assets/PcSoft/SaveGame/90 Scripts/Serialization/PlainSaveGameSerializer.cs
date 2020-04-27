using System;
using PcSoft.SaveGame._90_Scripts.Types;
using PcSoft.SaveGame._90_Scripts.Utils;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class PlainSaveGameSerializer<T> : SaveGameSerializer where T : class
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

        protected override void LoadAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGame)
            {
                SaveGame = formatter.Deserialize(Migrator);
            }

            op?.Completed();
        }

        protected override void SaveAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGame)
            {
                formatter.Serialize(SaveGame);
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