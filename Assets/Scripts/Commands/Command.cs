using System;
using System.Linq;

namespace Commands
{
    public abstract class Command
    {
        public Type FinishedCommandType
        {
            get
            {
                CommandFinishAtAttribute attribute = GetType()
                    .GetCustomAttributes(typeof(CommandFinishAtAttribute), true)
                    .FirstOrDefault() as CommandFinishAtAttribute;
                return attribute?.FinishedCommandType;
            }
        }
        public bool CanCommandFinish => FinishedCommandType != null;
        
        public virtual void Execute() {}
    }
}
