using System;

namespace GenericInputs
{
    [Serializable]
    public abstract class Size : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    
}
