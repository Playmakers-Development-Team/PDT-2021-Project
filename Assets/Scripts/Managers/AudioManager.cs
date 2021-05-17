using UnityEngine;

namespace Managers
{
    public class AudioManager : Manager
    {
        public void ChangeVolume(string volume, float value)
        {
            AkSoundEngine.SetRTPCValue(volume, value);
        }
    }
}
