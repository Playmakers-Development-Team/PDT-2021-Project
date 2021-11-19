using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioSettings
    {
        [SerializeField] private Dictionary<VolumeParameter, float> volumes = new Dictionary<VolumeParameter, float>();

        public void SetVolume(VolumeParameter volumeParameter, float volume)
        {
            volumes[volumeParameter] = volume;
        }

        public float GetVolume(VolumeParameter volumeParameter)
        {
            if (volumes.ContainsKey(volumeParameter))
                return volumes[volumeParameter];
            
            return 50f;
        }
    }
}