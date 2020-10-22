using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace PcSoft.SavePrefs._90_Scripts.Utils
{
    public static class PlayerPrefsEx
    {
        private static readonly DateTimeFormat DateFormat = new DateTimeFormat("dd-MM-yyyy hh:mm:ss");

        #region Events

        public static event EventHandler<PlayerPrefsChangeEventArgs> OnChanged;

        #endregion

        public static bool GetBool(string key, bool def) => PlayerPrefs.GetInt(key, def ? 1 : 0) != 0;

        public static void SetBool(string key, bool val, bool autoSave = false)
        {
            PlayerPrefs.SetInt(key, val ? 1 : 0);
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Boolean, key);
        }

        public static int GetInt(string key, int def) => PlayerPrefs.GetInt(key, def);

        public static void SetInt(string key, int val, bool autoSave = false)
        {
            PlayerPrefs.SetInt(key, val);
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Int, key);
        }
        
        public static long GetLong(string key, long def) => PlayerPrefs.HasKey(key) ? long.Parse(PlayerPrefs.GetString(key)) : def;

        public static void SetLong(string key, long val, bool autoSave = false)
        {
            PlayerPrefs.SetString(key, val.ToString());
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Long, key);
        }

        public static float GetFloat(string key, float def) => PlayerPrefs.GetFloat(key, def);

        public static void SetFloat(string key, float val, bool autoSave = false)
        {
            PlayerPrefs.SetFloat(key, val);
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Float, key);
        }
        
        public static string GetString(string key, string def) => PlayerPrefs.GetString(key, def);

        public static void SetString(string key, string val, bool autoSave = false)
        {
            PlayerPrefs.SetString(key, val);
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.String, key);
        }
        
        public static byte[] GetBytes(string key, byte[] def) => Convert.FromBase64String(PlayerPrefs.GetString(key, Convert.ToBase64String(def)));

        public static void SetBytes(string key, byte[] val, bool autoSave = false)
        {
            PlayerPrefs.SetString(key, Convert.ToBase64String(val));
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Binary, key);
        }
        
        public static DateTime GetDateTime(string key, DateTime def)
        {
            return DateTime.Parse(PlayerPrefs.GetString(key, def.ToString(DateFormat.FormatProvider)), DateFormat.FormatProvider);
        }

        public static void SetDateTime(string key, DateTime val, bool autoSave = false)
        {
            PlayerPrefs.SetString(key, val.ToString(DateFormat.FormatProvider));
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.DateTime, key);
        }

        public static TimeSpan GetTimeSpan(string key, TimeSpan def)
        {
            return TimeSpan.Parse(PlayerPrefs.GetString(key, def.ToString()));
        }

        public static void SetTimeSpan(string key, TimeSpan value, bool autoSave = false)
        {
            PlayerPrefs.SetString(key, value.ToString());
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.TimeSpan, key);
        }

        public static T GetEnum<T>(string key, T def) where T : Enum
        {
            var s = PlayerPrefs.GetString(key, def.ToString());
            return (T) Enum.Parse(typeof(T), s);
        }

        public static void SetEnum<T>(string key, T value, bool autoSave = false) where T : Enum
        {
            PlayerPrefs.SetString(key, value.ToString());
            if (autoSave)
            {
                PlayerPrefs.Save();
            }
            RaiseChange(PlayerPrefsChangeType.AddOrUpdate, PlayerPrefsDataType.Enum, key);
            
        }

        public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
        
        public static bool HasKeys(KeySearchType type, params string[] keys)
        {
            foreach (var key in keys)
            {
                switch (type)
                {
                    case KeySearchType.All:
                        if (!PlayerPrefs.HasKey(key))
                            return false;
                        break;
                    case KeySearchType.Any:
                        if (PlayerPrefs.HasKey(key))
                            return true;
                        break;
                    case KeySearchType.None:
                        if (PlayerPrefs.HasKey(key))
                            return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            switch (type)
            {
                case KeySearchType.All:
                    return true;
                case KeySearchType.Any:
                    return false;
                case KeySearchType.None:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            RaiseChange(PlayerPrefsChangeType.Delete, PlayerPrefsDataType.Unspecific, key);
        }

        public static void DeleteKeys(params string[] keys)
        {
            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey(key);
            }
            RaiseChange(PlayerPrefsChangeType.Delete, PlayerPrefsDataType.Unspecific, keys);
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            RaiseChange(PlayerPrefsChangeType.DeleteAll, PlayerPrefsDataType.Unspecific);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        private static void RaiseChange(PlayerPrefsChangeType changeType, PlayerPrefsDataType dataType, params string[] keys)
        {
            OnChanged?.Invoke(null, new PlayerPrefsChangeEventArgs(keys, changeType, dataType));
        }
    }

    public enum KeySearchType
    {
        All,
        Any,
        None
    }

    public class PlayerPrefsChangeEventArgs : EventArgs
    {
        public string[] Keys { get; }
        public PlayerPrefsChangeType ChangeType { get; }
        public PlayerPrefsDataType DataType { get; }

        public PlayerPrefsChangeEventArgs(string[] keys, PlayerPrefsChangeType changeType, PlayerPrefsDataType dataType)
        {
            Keys = keys;
            ChangeType = changeType;
            DataType = dataType;
        }
    }

    public enum PlayerPrefsChangeType
    {
        AddOrUpdate,
        Delete,
        DeleteAll
    }

    public enum PlayerPrefsDataType
    {
        Unspecific,
        String,
        Int,
        Long,
        Float,
        Boolean,
        Binary,
        DateTime,
        TimeSpan,
        Enum
    }
}