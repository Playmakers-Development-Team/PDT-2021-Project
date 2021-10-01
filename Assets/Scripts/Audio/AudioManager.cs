using Audio.Commands;
using Commands;
using Managers;

namespace Audio
{
    public class AudioManager : Manager
    {

        private CommandManager commandManager;
        
        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        public void ChangeVolume(string volume, float value) => AkSoundEngine.SetRTPCValue(volume, value);
        
        /// <summary>
        /// Update the current music of the game
        /// </summary>
        /// <param name="StateGroup"></param>
        /// <param name="stateName"></param>
        public void ChangeMusicState(string stateGroup, string stateName) => commandManager.ExecuteCommand(new ChangeMusicStateCommand(stateGroup,stateName));
        
        
    }
}
