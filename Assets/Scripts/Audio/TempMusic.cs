using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class TempMusic : MonoBehaviour
    {
        private static TempMusic Instance { get; set; }

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += ChangeMusic;
                AkSoundEngine.PostEvent("Play_Mountain_Trail_Theme", gameObject);
            }
        }

        private void ChangeMusic(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "Playtest Beta Map" || scene.name == "Map Test")
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");
            else
                AkSoundEngine.SetState("CombatState", "In_Combat");
        }
    }
}
