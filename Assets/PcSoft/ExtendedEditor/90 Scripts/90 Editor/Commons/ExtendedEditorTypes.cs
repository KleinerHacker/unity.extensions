using System;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Commons
{
    public sealed class TabItem
    {
        public string Title { get; }
        public Action OnGUI { get; }

        public TabItem(string title, Action onGui)
        {
            Title = title;
            OnGUI = onGui;
        }
    }
}