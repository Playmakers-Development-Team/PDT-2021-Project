using UnityEngine;

namespace Managers
{
    public class AudioManager : IManager
    {
        public void ChangeVolume(string volume, float value)
        {
            AkSoundEngine.SetRTPCValue(volume, value);
        }
    }
}
