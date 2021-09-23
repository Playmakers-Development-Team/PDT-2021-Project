﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class EncounterData : ScriptableObject
    {
        public SceneReference encounterScene;
        public LevelPool levelPool;

        /// <summary>
        /// Get all the scenes that is involved with this encounter data.
        /// Useful for build tools, to figure out what scenes should be in the build.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SceneReference> GetAllPossibleScenes() =>
            encounterScene != null ? new[] { encounterScene } : levelPool.LevelScenes;

        /// <summary>
        /// Get the next scene for the encounter. It may be one from the level pool or a specific scene.
        /// </summary>
        public SceneReference PullEncounterScene()
        {
            if (string.IsNullOrEmpty(encounterScene?.ScenePath) && levelPool == null)
                throw new Exception($"Encounter data {name} has no scene assigned!");
            
            return !string.IsNullOrEmpty(encounterScene?.ScenePath) ? encounterScene : levelPool.PullLevel();
        }
    }
}
