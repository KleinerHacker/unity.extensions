using System;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Commons
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializedPropertyReferenceAttribute : Attribute
    {
        public string Name { get; set; }

        public SerializedPropertyReferenceAttribute(string name)
        {
            Name = name;
        }
    }

    #region Representation

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class SerializedPropertyRepresentationAttribute : Attribute
    {
        public uint Order { get; set; } = 0;
        public float PreSpace { get; set; } = 0f;
        public float PostSpace { get; set; } = 0f;
        public string Title { get; set; } = null;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyDefaultRepresentationAttribute : SerializedPropertyRepresentationAttribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyImplicitRepresentationAttribute : SerializedPropertyRepresentationAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyIdentifiedArrayRepresentationAttribute : SerializedPropertyRepresentationAttribute
    {
        public Type EnumType { get; set; }

        public SerializedPropertyIdentifiedArrayRepresentationAttribute(string title, Type enumType)
        {
            Title = title;
            EnumType = enumType;
        }
    }

    #endregion

    #region Grouping

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class SerializedPropertyGroupAttribute : Attribute
    {
        public string Title { get; set; }
        public uint Order { get; set; } = 0;

        protected SerializedPropertyGroupAttribute(string title)
        {
            Title = title;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyTabGroupAttribute : SerializedPropertyGroupAttribute
    {
        public string Name { get; set; }
        
        public SerializedPropertyTabGroupAttribute(string title, string name) : base(title)
        {
            Name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyFoldingGroupAttribute : SerializedPropertyGroupAttribute
    {
        public bool UseIntent { get; set; } = true;
        
        public SerializedPropertyFoldingGroupAttribute(string title) : base(title)
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedPropertyLabeledGroupAttribute : SerializedPropertyGroupAttribute
    {
        public bool UseIntent { get; set; } = true;
        
        public SerializedPropertyLabeledGroupAttribute(string title) : base(title)
        {
        }
    }

    #endregion
}