﻿using Audio.Commands;
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
                AkSoundEngine.PostEvent("Play_Mountain_Trail_Theme", gameObject);
            }
        }

        private void Start()
        {
            commandManager.ListenCommand<ChangeMusicStateCommand>(cmd => ChangeMusicState(cmd.StateGroup,
                cmd.StateName));
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand<ChangeMusicStateCommand>(cmd => ChangeMusicState(cmd.StateGroup,
                cmd.StateName));
        }

        private void ChangeMusic(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "Playtest Beta Map" || scene.name == "Map Test")
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");
            else
                AkSoundEngine.SetState("CombatState", "In_Combat");
        }
    
        private static void ChangeMusicState(string Group, string Name) => AkSoundEngine.SetState(Group, Name);
        
        
    }
}