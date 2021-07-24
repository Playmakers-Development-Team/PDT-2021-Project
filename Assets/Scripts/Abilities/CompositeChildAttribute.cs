using System;

namespace Abilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CompositeChildAttribute : Attribute
    {
        public int EnumValue { get; }

        public CompositeChildAttribute(int enumValue)
        {
            this.EnumValue = enumValue;
        }
    }
}
