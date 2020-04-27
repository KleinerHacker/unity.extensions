using System;
using UnityEngine;

namespace PcSoft.SavePrefs._90_Scripts.Utils
{
    public static class PlayerPrefsEx
    {
        private static bool _autoSave;

        public static bool AutoSave
        {
            get => _autoSave;
            set
            {
                _autoSave = value;
                if (value)
                {
                    PlayerPrefs.Save();
                }
            }
        }

        public static bool GetBool(string key, bool def) => PlayerPrefs.GetInt(key, def ? 1 : 0) != 0;

        public static void SetBool(string key, bool val)
        {
            PlayerPrefs.SetInt(key, val ? 1 : 0);
            if (AutoSave)
            {
                PlayerPrefs.Save();
            }
        }

        public static int GetInt(string key, int def) => PlayerPrefs.GetInt(key, def);

        public static void SetInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
            if (AutoSave)
            {
                PlayerPrefs.Save();
            }
        }

        public static float GetFloat(string key, float def) => PlayerPrefs.GetFloat(key, def);

        public static void SetFloat(string key, float val)
        {
            PlayerPrefs.SetFloat(key, val);
            if (AutoSave)
            {
                PlayerPrefs.Save();
            }
        }
        
        public static string GetString(string key, string def) => PlayerPrefs.GetString(key, def);

        public static void SetString(string key, string val)
        {
            PlayerPrefs.SetString(key, val);
            if (AutoSave)
            {
                PlayerPrefs.Save();
            }
        }
        
        public static byte[] GetBytes(string key, byte[] def) => Convert.FromBase64String(PlayerPrefs.GetString(key, Convert.ToBase64String(def)));

        public static void SetBytes(string key, byte[] val)
        {
            PlayerPrefs.SetString(key, Convert.ToBase64String(val));
            if (AutoSave)
            {
                PlayerPrefs.Save();
            }
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

        public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);

        public static void DeleteKeys(params string[] keys)
        {
            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        public static void DeleteAll() => PlayerPrefs.DeleteAll();

        public static void Save()
        {
            if (AutoSave)
                throw new InvalidOperationException("Auto save is on, please do not call this method or disable auto save");
            
            PlayerPrefs.Save();
        }
    }

    public enum KeySearchType
    {
        All,
        Any,
        None
    }
}