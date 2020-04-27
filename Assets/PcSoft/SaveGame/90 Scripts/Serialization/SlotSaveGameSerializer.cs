using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PcSoft.SaveGame._90_Scripts.Types;
using PcSoft.SaveGame._90_Scripts.Utils;

namespace PcSoft.SaveGame._90_Scripts.Serialization
{
    public class SlotSaveGameSerializer<TS, TM> : SaveGameSerializer where TS : SlotData where TM : class
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

        public void AddOrReplaceData(TS slotData, TM modelData)
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

        public bool HasData(object identifier)
        {
            lock (_saveGameDictionary)
            {
                return _saveGameDictionary.Keys.Any(x => Equals(x.Identifier, identifier));
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

        public TM GetData(object identifier)
        {
            lock (_saveGameDictionary)
            {
                if (!HasData(identifier))
                    return default;

                return _saveGameDictionary.First(x => Equals(x.Key.Identifier, identifier)).Value;
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

        public void RemoveData(object identifier)
        {
            lock (_saveGameDictionary)
            {
                if (!HasData(identifier))
                    return;

                _saveGameDictionary.Remove(_saveGameDictionary.Keys.First(x => Equals(x.Identifier, identifier)));
            }
        }

        protected override void LoadAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGameDictionary)
            {
                _saveGameDictionary.Clear();

                var count = new BinaryReader(formatter.BaseStream).ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    if (op != null)
                    {
                        op.Progress = (float) i / count;
                    }

                    var slotData = formatter.Deserialize(SlotMigrator);
                    var modelData = formatter.Deserialize(ModelMigrator);

                    _saveGameDictionary.Add(slotData, modelData);
                }
            }

            op?.Completed();
        }

        protected override void SaveAsync(SaveGameFormatter formatter, AsyncSerialization op)
        {
            lock (_saveGameDictionary)
            {
                new BinaryWriter(formatter.BaseStream).Write(_saveGameDictionary.Count);
                for (var i = 0; i < _saveGameDictionary.Count; i++)
                {
                    if (op != null)
                    {
                        op.Progress = (float) i / _saveGameDictionary.Count;
                    }

                    var pair = _saveGameDictionary.ElementAt(i);

                    formatter.Serialize(pair.Key);
                    formatter.Serialize(pair.Value);
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

    [Serializable]
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