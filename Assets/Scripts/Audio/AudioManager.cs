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

        public float GetVolume(string volume)
        {
            int type = 1;

            AkSoundEngine.GetRTPCValue(volume, null, 0, out var value, ref type);

            return value;
        }

        /// <summary>
        /// Update the current music of the game
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="stateName"></param>
        public void ChangeMusicState(string stateGroup, string stateName) =>
            commandManager.ExecuteCommand(new ChangeMusicStateCommand(stateGroup, stateName));
    }
}
