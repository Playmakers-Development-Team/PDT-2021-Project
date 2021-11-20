using System;
using UnityEngine;
using Utilities;

namespace Audio
{
    [Serializable]
    public class AudioSettings
    {
        [SerializeField] private SerializableDictionary<VolumeParameter, float> volumes = new SerializableDictionary<VolumeParameter, float>();

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