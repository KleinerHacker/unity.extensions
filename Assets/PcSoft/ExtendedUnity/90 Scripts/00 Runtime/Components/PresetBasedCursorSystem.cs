using System;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Assets;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    public abstract class PresetBasedCursorSystem<T, TDefault, TDefaultEnum, TSpecific, TSpecificEnum> : CursorSystem<TDefault, TSpecific>
        where T : CursorPreset<TDefault, TDefaultEnum, TSpecific, TSpecificEnum>
        where TSpecific : CursorItem<TSpecificEnum>
        where TDefault : CursorItem<TDefaultEnum>
        where TSpecificEnum : Enum
        where TDefaultEnum : Enum
    {
        #region Inspector Data

        [SerializeField]
        private T preset;

        #endregion

        public void OverwriteDefaultCursor(TDefaultEnum cursor)
        {
            var cursorItem = preset.Find(cursor);
            base.OverwriteDefaultCursor(cursorItem);
        }

        public void ShowSpecificCursor(TSpecificEnum cursor)
        {
            var cursorItem = preset.Find(cursor);
            base.ShowSpecificCursor(cursorItem);
        }

        public void HideSpecificCursor(TSpecificEnum cursor)
        {
            var cursorItem = preset.Find(cursor);
            base.HideSpecificCursor(cursorItem);
        }
    }
}