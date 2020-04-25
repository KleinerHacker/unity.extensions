using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PcSoft.SaveGame._90_Scripts.Types;
using UnityEngine;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class CollectionSaveGameSerializer<T> : SaveGameSerializer
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

        protected override void LoadAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameList)
            {
                SaveGameList.Clear();
                using (var reader = new BinaryReader(stream))
                {
                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / count;
                        }

                        var saveGame = (T) formatter.Deserialize(stream);
                        SaveGameList.Add(saveGame);
                    }
                }
            }

            op?.Completed();
        }

        protected override void LoadMigrationAsync(Stream stream, ulong version, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameList)
            {
                SaveGameList.Clear();
                using (var reader = new BinaryReader(stream))
                {
                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / count;
                        }

                        var oldSaveGame = (T) formatter.Deserialize(stream);
                        var saveGame = Migrator != null ? Migrator.Invoke(version, oldSaveGame) : oldSaveGame;
                        SaveGameList.Add(saveGame);
                    }
                }
            }

            op?.Completed();
        }

        protected override void SaveAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameList)
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(SaveGameList.Count);
                    for (var i = 0; i < SaveGameList.Count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / SaveGameList.Count;
                        }

                        var saveGame = SaveGameList[i];
                        formatter.Serialize(stream, saveGame);
                    }
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