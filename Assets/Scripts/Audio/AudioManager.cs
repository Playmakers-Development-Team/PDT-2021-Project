using Audio.Commands;
using Commands;
using Managers;
using UnityEngine;
using Utilities;

namespace Audio
{
    public class AudioManager : Manager
    {
        private CommandManager commandManager;

        private AudioSettings audioSettings = new AudioSettings();

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            
            LoadVolumeSettings();
        }

        public void SetVolume(VolumeParameter volumeParameter, float value)
        {
            audioSettings.SetVolume(volumeParameter, value);

            AkSoundEngine.SetRTPCValue(volumeParameter.ToString(), value);
            
            SaveVolumeSettings();
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
        
        private void SaveVolumeSettings()
        {
            PlayerPrefs.SetString("AudioSettings", JsonUtility.ToJson(audioSettings));
        }

        private void LoadVolumeSettings()
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("AudioSettings"), audioSettings);

            foreach (var (volumeParameter, value) in audioSettings.GetVolumes())
            {
                AkSoundEngine.SetRTPCValue(volumeParameter.ToString(), value);
            }
        }
    }
}
