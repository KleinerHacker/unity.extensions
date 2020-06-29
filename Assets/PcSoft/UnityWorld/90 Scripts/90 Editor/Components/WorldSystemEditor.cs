using System;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor;
using PcSoft.UnityScene._90_Scripts._90_Editor.Components;
using UnityEditor;
using UnityEngine;

namespace PcSoft.UnityWorld._90_Scripts._90_Editor.Components
{
    public abstract class WorldSystemEditor<T> : SceneSystemEditor<T> where T : Enum
    {
    }
}