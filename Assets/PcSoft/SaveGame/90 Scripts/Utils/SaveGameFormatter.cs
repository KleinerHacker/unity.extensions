using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace PcSoft.SaveGame._90_Scripts.Utils
{
    public sealed class SaveGameFormatter
    {
        private readonly ulong _version;

        public Stream BaseStream { get; }

        public SaveGameFormatter(Stream baseStream, ulong version)
        {
            BaseStream = baseStream;
            _version = version;
        }

        public T Deserialize<T>(Func<ulong, T, T> migrator = null) where T : class
        {
            var objectVersion = new BinaryReader(BaseStream, Encoding.UTF8).ReadUInt64();
            var o = (T) DeserializeNext(BaseStream, objectVersion);

            if (objectVersion > _version)
                throw new SerializationException("Found version " + objectVersion + " is higher than current version " + _version);
            if (objectVersion < _version)
            {
                Debug.LogWarning("Found older version " + objectVersion + ", migrate to " + _version);
                return migrator?.Invoke(objectVersion, o);
            }

            return o;
        }

        public void Serialize<T>(T o) where T : class
        {
            new BinaryWriter(BaseStream, Encoding.UTF8).Write(_version);
            SerializeNext(BaseStream, o);
        }

        private void SerializeNext(Stream stream, object o)
        {
            var type = o.GetType();

            if (!type.IsSerializable)
                throw new SerializationException("Type " + type.FullName + " is not marked as serializable");
            if (type.GetConstructor(new Type[0]) == null)
                throw new SerializationException("Type " + type.FullName + " has no default constructor");

            new BinaryWriter(stream, Encoding.UTF8).Write(type.FullName);
            foreach (var field in type.GetRuntimeFields())
            {
                if (field.IsStatic)
                    continue;
                if (field.IsNotSerialized)
                    continue;

                var versionHint = field.GetCustomAttribute<VersionHintAttribute>();
                if (versionHint != null && (_version < versionHint.SinceVersion || _version > versionHint.UntilVersion))
                    continue;

                if (field.FieldType.IsPrimitive)
                {
                    var value = field.GetValue(o);
                    SerializePrimitiveValue(stream, value);
                }
                else
                {
                    var value = field.GetValue(o);
                    try
                    {
                        SerializeNext(stream, value);
                    }
                    catch (SerializationException e)
                    {
                        throw new SerializationException("Unable to serialize field " + field.Name + " in class " + type.FullName, e);
                    }
                }
            }
        }

        private object DeserializeNext(Stream stream, ulong objectVersion)
        {
            var typeName = new BinaryReader(stream, Encoding.UTF8).ReadString();
            var type = Type.GetType(typeName);
            if (type == null)
                throw new SerializationException("Unable to find needed type " + typeName);

            if (!type.IsSerializable)
                throw new SerializationException("Type " + type.FullName + " is not marked as serializable");
            var defaultConstructor = type.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                throw new SerializationException("Type " + type.FullName + " has no default constructor");

            var o = defaultConstructor.Invoke(new object[0]);
            foreach (var field in type.GetRuntimeFields())
            {
                if (field.IsNotSerialized)
                    continue;

                var versionHint = field.GetCustomAttribute<VersionHintAttribute>();
                if (versionHint != null && (objectVersion < versionHint.SinceVersion || objectVersion > versionHint.UntilVersion))
                    continue;

                if (field.FieldType.IsPrimitive)
                {
                    var value = DeserializePrimitiveValue(stream);
                    field.SetValue(o, value);
                }
                else
                {
                    try
                    {
                        var value = DeserializeNext(stream, objectVersion);
                        field.SetValue(o, value);
                    }
                    catch (SerializationException e)
                    {
                        throw new SerializationException("Unable to serialize field " + field.Name + " in class " + type.FullName, e);
                    }
                }
            }

            return o;
        }

        private static void SerializePrimitiveValue(Stream stream, object value)
        {
            new BinaryFormatter().Serialize(stream, value);
        }

        private static object DeserializePrimitiveValue(Stream stream)
        {
            return new BinaryFormatter().Deserialize(stream);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VersionHintAttribute : Attribute
    {
        public ulong SinceVersion { get; } = ulong.MinValue;
        public ulong UntilVersion { get; } = ulong.MaxValue;
    }
}