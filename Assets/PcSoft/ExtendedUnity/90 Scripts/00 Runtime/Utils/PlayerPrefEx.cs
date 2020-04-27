using System;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils
{
    public static class PlayerPrefEx
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
    }
}