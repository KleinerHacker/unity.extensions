using System;

namespace PcSoft.EasyTutorial._90_Scripts._90_Editor.Components
{
    public abstract class ParallelTutorialSystemEditor<T> : TutorialSystemEditor<T> where T : Enum
    {
        protected ParallelTutorialSystemEditor(T noneValue) : base(noneValue)
        {
        }
    }
}