using Audio;
using Managers;
using UnityEngine;
using Utility;

namespace UI
{
    public class VolumeControl : MonoBehaviour
    {
        // Volume parameter set by Wwise
        [SerializeField] private VolumeParameter volumeParameter; 
        private AudioManager audioManager;

        private void Start()
        {
            audioManager = ManagerLocator.Get<AudioManager>();
        }

        // Called by sliders to change volume
        public void ChangeVolume(float value)
        {
            if (volumeParameter == VolumeParameter.None)
                return;
            audioManager.ChangeVolume(volumeParameter.ToString(), value);
        }
    }
}
