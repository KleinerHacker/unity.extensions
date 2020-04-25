using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PcSoft.SaveGame._90_Scripts.Types;
using PcSoft.SaveGame._90_Scripts.Utils.Extensions;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class SlotSaveGameSerializer<TS, TM> : SaveGameSerializer where TS : SlotData
    {
        public TS[] Slots
        {
            get
            {
                lock (_saveGameDictionary)
                {
                    return _saveGameDictionary.Keys.ToArray();
                }
            }
        }

        public Func<ulong, TS, TS> SlotMigrator { get; set; }
        public Func<ulong, TM, TM> ModelMigrator { get; set; }

        private readonly IDictionary<TS, TM> _saveGameDictionary = new Dictionary<TS, TM>();

        internal SlotSaveGameSerializer(string filename, ulong version) : base(filename, version)
        {
        }

        public void AddOrSetData(TS slotData, TM modelData)
        {
            lock (_saveGameDictionary)
            {
                if (_saveGameDictionary.ContainsKey(slotData))
                {
                    _saveGameDictionary[slotData] = modelData;
                }
                else
                {
                    _saveGameDictionary.Add(slotData, modelData);
                }
            }
        }

        public bool HasData(TS slotData)
        {
            lock (_saveGameDictionary)
            {
                return _saveGameDictionary.ContainsKey(slotData);
            }
        }

        public TM GetData(TS slotData)
        {
            lock (_saveGameDictionary)
            {
                if (!_saveGameDictionary.ContainsKey(slotData))
                    return default;

                return _saveGameDictionary[slotData];
            }
        }

        public void RemoveData(TS slotData)
        {
            lock (_saveGameDictionary)
            {
                if (!_saveGameDictionary.ContainsKey(slotData))
                    return;

                _saveGameDictionary.Remove(slotData);
            }
        }

        protected override void LoadAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameDictionary)
            {
                _saveGameDictionary.Clear();

                using (var reader = new BinaryReader(stream))
                {
                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / count;
                        }

                        var slotData = (TS) formatter.Deserialize(stream);
                        var modelData = (TM) formatter.Deserialize(stream);

                        _saveGameDictionary.Add(slotData, modelData);
                    }
                }
            }

            op?.Completed();
        }

        protected override void LoadMigrationAsync(Stream stream, ulong version, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameDictionary)
            {
                _saveGameDictionary.Clear();

                using (var reader = new BinaryReader(stream))
                {
                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / count;
                        }

                        var oldSlotData = (TS) formatter.Deserialize(stream);
                        var oldModelData = (TM) formatter.Deserialize(stream);

                        var slotData = SlotMigrator != null ? SlotMigrator.Invoke(version, oldSlotData) : oldSlotData;
                        var modelData = ModelMigrator != null ? ModelMigrator.Invoke(version, oldModelData) : oldModelData;

                        _saveGameDictionary.Add(slotData, modelData);
                    }
                }
            }

            op?.Completed();
        }

        protected override void SaveAsync(Stream stream, AsyncSerialization op)
        {
            var formatter = new BinaryFormatter();
            lock (_saveGameDictionary)
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(_saveGameDictionary.Count);
                    for (var i = 0; i < _saveGameDictionary.Count; i++)
                    {
                        if (op != null)
                        {
                            op.Progress = (float) i / _saveGameDictionary.Count;
                        }

                        var pair = _saveGameDictionary.ElementAt(i);

                        formatter.Serialize(stream, pair.Key);
                        formatter.Serialize(stream, pair.Value);
                    }
                }
            }

            op?.Completed();
        }
    }

    public class SlotSaveGameSerializer : SlotSaveGameSerializer<SlotData, object>
    {
        internal SlotSaveGameSerializer(string filename, ulong version) : base(filename, version)
        {
        }
    }

    public abstract class SlotData
    {
        public abstract object Identifier { get; }

        #region Equals / Hashcode

        protected bool Equals(SlotData other)
        {
            return Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SlotData) obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        #endregion
    }
}