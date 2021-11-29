using Commands;

namespace Audio.Commands
{
    public class PostSound : Command
    {
        public string SoundName { get; }

        public PostSound(string soundName)
        {
            this.SoundName = soundName;
        }
    }
    
    public class ChangeWalkingStateCommand : Command
    {
        public bool IsWalking { get; }
        
        public bool IsGrass { get; }

        public ChangeWalkingStateCommand(bool isWalking, bool isGrass)
        {
            this.IsWalking = isWalking;
            this.IsGrass = isGrass;
        }
    }
    
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
