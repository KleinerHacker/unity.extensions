using System;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra
{
    public class AssetChooserAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public AssetChooserAttribute(Type type)
        {
            Type = type;
        }
    }
}