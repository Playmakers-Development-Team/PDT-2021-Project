using Managers;
using UnityEngine;

namespace UI
{
    public class VolumeControl : MonoBehaviour
    {
        [SerializeField] private string volume; // Volume parameter set by Wwise
        private AudioManager audioManager;

        private void Start()
        {
            audioManager = ManagerLocator.Get<AudioManager>();
        }

        // Called by sliders to change volume
        public void ChangeVolume(float value)
        {
            audioManager.ChangeVolume(volume, value);
        }
    }
}
