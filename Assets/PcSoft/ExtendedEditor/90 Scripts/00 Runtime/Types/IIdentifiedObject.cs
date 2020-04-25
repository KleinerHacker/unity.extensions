using System;

namespace PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types
{
    public interface IIdentifiedObject<out T> where T : Enum
    {
        T Identifier { get; }
    }
}