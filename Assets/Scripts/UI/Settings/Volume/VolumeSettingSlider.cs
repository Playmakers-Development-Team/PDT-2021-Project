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


        protected override void OnComponentAwake()
        {
            audioManager = ManagerLocator.Get<AudioManager>();
        }


        public void OnValueChanged(Single volume)
        {
            audioManager.ChangeVolume(volumeParameter.ToString(), volume);
        }

        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
