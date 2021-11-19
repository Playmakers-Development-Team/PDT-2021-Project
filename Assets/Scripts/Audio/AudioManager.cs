using Audio.Commands;
using Commands;
using Managers;

namespace Audio
{
    public class AudioManager : Manager
    {
        private CommandManager commandManager;

        private AudioSettings audioSettings = new AudioSettings();

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        public void SetVolume(VolumeParameter volumeParameter, float value)
        {
            audioSettings.SetVolume(volumeParameter, value);

            AkSoundEngine.SetRTPCValue(volumeParameter.ToString(), value);
        }

        public float GetVolume(VolumeParameter volumeParameter) =>
            audioSettings.GetVolume(volumeParameter);

        /// <summary>
        /// Update the current music of the game
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="stateName"></param>
        public void ChangeMusicState(string stateGroup, string stateName) =>
            commandManager.ExecuteCommand(new ChangeMusicStateCommand(stateGroup, stateName));
    }
}
