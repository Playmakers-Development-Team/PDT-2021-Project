using Audio;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSettings.Volume
{
    public class VolumeSettingSlider : DialogueComponent<SettingsDialogue>
    {
        [SerializeField] private VolumeParameter volumeParameter;
        [SerializeField] private Slider slider;

        private AudioManager audioManager;


        #region UIComponent
        
        // TODO: Assign slider value once AudioManager.GetVolume() implemented...
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            audioManager = ManagerLocator.Get<AudioManager>();

            slider.value = audioManager.GetVolume(volumeParameter.ToString());
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
