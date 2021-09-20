using System;

namespace Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandFinishAtAttribute : Attribute
    {
        public Type FinishedCommandType { get; }

        public CommandFinishAtAttribute(Type finishedCommandType)
        {
            FinishedCommandType = finishedCommandType;
        }
    }
}
