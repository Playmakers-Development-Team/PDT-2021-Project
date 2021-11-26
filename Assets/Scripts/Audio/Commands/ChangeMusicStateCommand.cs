using Commands;

namespace Audio.Commands
{
    public class PlaySoundCommand : Command
    {
        public string SoundName { get; }

        public PlaySoundCommand(string soundName)
        {
            this.SoundName = soundName;
        }
    }
    
    public class StopSoundCommand : Command
    {
        public string SoundName { get; }

        public StopSoundCommand(string soundName)
        {
            this.SoundName = soundName;
        }
    }
    
    public class ChangeWalkingStateCommand : Command
    {
        public bool IsWalking { get; }

        public ChangeWalkingStateCommand(bool isWalking)
        {
            this.IsWalking = isWalking;
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
