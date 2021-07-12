using Managers;

namespace Audio
{
    public class AudioManager : Manager
    {
        public void ChangeVolume(string volume, float value)
        {
            AkSoundEngine.SetRTPCValue(volume, value);
        }
    }
}
