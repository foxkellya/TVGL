using System;

namespace GenericInputs
{
    [Serializable]
    public abstract class Inputs : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

namespace GenericInputs
{
    [AttributeUsage(AttributeTargets.All)]
    public class AvailableSizeAttribute : Attribute
    {
        public readonly bool IsAvailableSize;

        public AvailableSizeAttribute(bool isAvailableSize)
        {
            IsAvailableSize = isAvailableSize;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class HideFromViews : Attribute
    {
        public readonly bool HideFromView;

        public HideFromViews(bool hideFromView)
        {
            HideFromView = hideFromView;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class SizeDimensionTypeIndexAttribute : Attribute
    {
        public readonly int Index;

        public SizeDimensionTypeIndexAttribute(int index)
        {
            Index = index;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class IndexAttribute : Attribute
    {
        public readonly int Index;

        public IndexAttribute(int index)
        {
            Index = index;
        }
    }

}
