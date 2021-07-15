using System;
using Audio;
using Managers;
using UnityEngine;

namespace UI.Settings.Volume
{
    public class VolumeSettingSlider : UIComponent<SettingsDialogue>
    {
        [SerializeField] private VolumeParameter volumeParameter;

        private AudioManager audioManager;


        // TODO: Assign slider value once AudioManager.GetVolume() implemented...
        protected override void OnComponentAwake()
        {
            audioManager = ManagerLocator.Get<AudioManager>();
        }


        public void OnValueChanged(float volume)
        {
            audioManager.ChangeVolume(volumeParameter.ToString(), volume);
        }

        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
