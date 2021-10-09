using Commands;

namespace Audio.Commands
{
    public class ChangeMusicStateCommand : Command
    {
        public string StateGroup { get; }
    
        public string StateName { get; }
    
        public ChangeMusicStateCommand(string StateGroup, string StateName)
        {
            this.StateGroup = StateGroup;
            this.StateName = StateName;
        }
    }
}
