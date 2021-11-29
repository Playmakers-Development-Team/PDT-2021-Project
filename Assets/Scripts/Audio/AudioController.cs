using System;
using Audio.Commands;
using Commands;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        private CommandManager commandManager;
        private static AudioController Instance { get; set; }

        private String previousSceneName = "";
        private bool mountainTrailPlayed = false;

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();

            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += ChangeMusic;
            }
        }

        private void Start()
        {
            commandManager.ListenCommand<ChangeMusicStateCommand>(cmd => ChangeMusicState(cmd.StateGroup,
                cmd.StateName));
            
            commandManager.ListenCommand<ChangeWalkingStateCommand>(cmd => ChangeWalkingState(cmd));
            
            commandManager.ListenCommand<PostSound>(cmd => PostSound(cmd.SoundName));
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand<ChangeMusicStateCommand>(cmd => ChangeMusicState(cmd.StateGroup,
                cmd.StateName));
            
            commandManager.UnlistenCommand<ChangeWalkingStateCommand>(cmd => ChangeWalkingState(cmd));
            
            commandManager.UnlistenCommand<PostSound>(cmd => PostSound(cmd.SoundName));
        }

        private void PostSound(string soundName)
        {
            uint soundID = AkSoundEngine.GetIDFromString(soundName);
            AkSoundEngine.PostEvent(soundID, gameObject);
        }

        /// <summary>
        /// Could not figure out how states worked for walking so instead I made a work around
        /// </summary>
        private void ChangeWalkingState(ChangeWalkingStateCommand cmd)
        {
            if (cmd.IsWalking)
            {
                if(cmd.IsGrass)
                    PostSound("Play_Grass_Footsteps");
                else
                    PostSound("Play_Unit_Walking");
            }
            else
            {
                //AkSoundEngine.SetState("WalkingState", "Not_Walking");
                PostSound("Stop_Unit_Walking");
            }
        }

        private void ChangeMusic(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (SceneManager.GetActiveScene().name.ToUpper())
            {
                case string levelName when levelName.Contains("MENU") && !previousSceneName.Contains("MENU"):
                    AkSoundEngine.PostEvent("Stop_Credits_Theme", gameObject);
                    AkSoundEngine.PostEvent("Stop_Mountain_Trail_Theme", gameObject);
                    AkSoundEngine.PostEvent("Stop_Ruined_City_Theme", gameObject);
                    AkSoundEngine.PostEvent("Play_Opening_Theme", gameObject);
                    mountainTrailPlayed = false;
                    break;
                case string levelName when levelName.Contains("CITY") && !previousSceneName.Contains("CITY"):
                    AkSoundEngine.PostEvent("Stop_Opening_Theme", gameObject);
                    AkSoundEngine.PostEvent("Stop_Mountain_Trail_Theme", gameObject);
                    AkSoundEngine.PostEvent("Play_Ruined_City_Theme", gameObject);
                    mountainTrailPlayed = false;
                    break;
                default:
                    if (!mountainTrailPlayed)
                    {
                        AkSoundEngine.PostEvent("Stop_Opening_Theme", gameObject);
                        AkSoundEngine.PostEvent("Play_Mountain_Trail_Theme", gameObject);
                        mountainTrailPlayed = true;
                    }
                    break;
            }
            
            if (scene.name == "Playtest Beta Map" || scene.name == "Map Test")
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");
            else
                AkSoundEngine.SetState("CombatState", "In_Combat");

            previousSceneName = SceneManager.GetActiveScene().name.ToUpper();
        }
    
        private static void ChangeMusicState(string Group, string Name) => AkSoundEngine.SetState(Group, Name);
        
        
    }
}
