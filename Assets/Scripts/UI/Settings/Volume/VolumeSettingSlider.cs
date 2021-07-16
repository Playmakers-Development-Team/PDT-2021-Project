using Audio;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Settings.Volume
{
    public class VolumeSettingSlider : UIComponent<SettingsDialogue>
    {
        [SerializeField] private VolumeParameter volumeParameter;

        private AudioManager audioManager;


        #region UIComponent
        
        // TODO: Assign slider value once AudioManager.GetVolume() implemented...
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            audioManager = ManagerLocator.Get<AudioManager>();
        }

        #endregion
        
        
        #region Listeners
        
        public void OnValueChanged(float volume)
        {
            audioManager.ChangeVolume(volumeParameter.ToString(), volume);
        }
        
        #endregion
    }
}
