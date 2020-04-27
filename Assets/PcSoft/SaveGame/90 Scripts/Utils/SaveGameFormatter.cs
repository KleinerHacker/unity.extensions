using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using PcSoft.SaveGame._90_Scripts.Utils.Extensions;
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
            if (o == null)
                throw new ArgumentException("object is null");

            new BinaryWriter(BaseStream, Encoding.UTF8).Write(_version);
            SerializeNext(BaseStream, o);
        }

        private void SerializeNext(Stream stream, object o)
        {
            stream.WriteByte((byte) (o == null ? 0x00 : 0xFF));
            if (o == null)
                return;

            var type = o.GetType();

            if (!type.IsSerializable)
                throw new SerializationException("Type " + type.FullName + " is not marked as serializable");

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

                if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
                {
                    var value = field.GetValue(o);
                    SerializePrimitiveValue(stream, value);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    var value = (IEnumerable) field.GetValue(o);
                    stream.WriteByte((byte) (value == null ? 0x00 : 0xFF));

                    new BinaryWriter(stream, Encoding.UTF8).Write(value.GetType().FullName);
                    new BinaryWriter(stream).Write(value.Count());
                    foreach (var item in value)
                    {
                        SerializeNext(stream, item);
                    }
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
            var nullByte = stream.ReadByte();
            if (nullByte == 0x00) //Value was NULL
                return null;

            var typeName = new BinaryReader(stream, Encoding.UTF8).ReadString();
            var type = FindType(typeName, Assembly.GetEntryAssembly());
            if (type == null)
                throw new SerializationException("Unable to find needed type <" + typeName + "> (" + stream.Position + ")");

            if (!type.IsSerializable)
                throw new SerializationException("Type " + type.FullName + " is not marked as serializable");

            var o = Activator.CreateInstance(type, true);
            foreach (var field in type.GetRuntimeFields())
            {
                if (field.IsNotSerialized)
                    continue;

                var versionHint = field.GetCustomAttribute<VersionHintAttribute>();
                if (versionHint != null && (objectVersion < versionHint.SinceVersion || objectVersion > versionHint.UntilVersion))
                    continue;

                if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
                {
                    var value = DeserializePrimitiveValue(stream);
                    field.SetValue(o, value);
                }
                else if (field.FieldType.IsArray)
                {
                    var nByte = stream.ReadByte();
                    if (nByte == 0x00)
                        continue;

                    var enumTypeName = new BinaryReader(stream, Encoding.UTF8).ReadString();
                    var enumType = FindType(enumTypeName, Assembly.GetEntryAssembly());
                    var count = new BinaryReader(stream).ReadInt32();

                    var value = (IEnumerable) Activator.CreateInstance(enumType);
                    for (var i = 0; i < count; i++)
                    {
                        var item = DeserializeNext(stream, objectVersion);
                        value.Append(item);
                    }
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

        private static Type FindType(string name, Assembly assembly)
        {
            var type = assembly.GetType(name);
            if (type != null)
                return type;

            return assembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Select(asm => FindType(name, asm))
                .FirstOrDefault(t => t != null);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VersionHintAttribute : Attribute
    {
        public ulong SinceVersion { get; } = ulong.MinValue;
        public ulong UntilVersion { get; } = ulong.MaxValue;
    }
}