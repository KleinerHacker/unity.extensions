using System;
using System.Collections.Generic;
using System.IO;
using PcSoft.SaveGame._90_Scripts.Types;
using PcSoft.SaveGame._90_Scripts.Utils;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class CollectionSaveGameSerializer<T> : SaveGameSerializer where T : class
    {
        private readonly IList<T> _saveGameList = new List<T>();

        public IList<T> SaveGameList
        {
            get
            {
                lock (_saveGameList)
                {
                    return _saveGameList;
                }
            }
        }

        public Func<ulong, T, T> Migrator { get; set; }

        internal CollectionSaveGameSerializer(string filename, ulong version) : base(filename, version)
        {
        }

        protected override void LoadAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGameList)
            {
                SaveGameList.Clear();

                var count = new BinaryReader(formatter.BaseStream).ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    if (op != null)
                    {
                        op.Progress = (float) i / count;
                    }

                    var saveGame = formatter.Deserialize(Migrator);
                    SaveGameList.Add(saveGame);
                }
            }

            op?.Completed();
        }

        protected override void SaveAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGameList)
            {
                new BinaryWriter(formatter.BaseStream).Write(SaveGameList.Count);
                for (var i = 0; i < SaveGameList.Count; i++)
                {
                    if (op != null)
                    {
                        op.Progress = (float) i / SaveGameList.Count;
                    }

                    var saveGame = SaveGameList[i];
                    formatter.Serialize(saveGame);
                }
            }

            op?.Completed();
        }
    }

    public class CollectionSaveGameSerializer : CollectionSaveGameSerializer<object>
    {
        internal CollectionSaveGameSerializer(string filename, ulong version) : base(filename, version)
        {
        }
    }
}