using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [CreateAssetMenu(fileName = "Level Pool", menuName = "Level Pool", order = 0)]
    public class LevelPool : ScriptableObject
    {
        [SerializeField] private List<SceneReference> levelScenes;

        public IReadOnlyList<SceneReference> LevelScenes => levelScenes;

        /// <summary>
        /// Get a random level from the level pool.
        /// </summary>
        public SceneReference PullLevel()
        {
            if (levelScenes.Count == 0)
            {
                throw new Exception(
                    $"Trying to pull a level from level pool {name}, but it has no scenes assigned!");
            }

            int randomIndex = UnityEngine.Random.Range(0, levelScenes.Count);
            return levelScenes[randomIndex];
        }
        
        /// <summary>
        /// Get a random arrangement of all possible levels from the level pool.
        /// </summary>
        public IEnumerable<SceneReference> PullLevels()
        {
            if (levelScenes.Count == 0)
            {
                throw new Exception(
                    $"Trying to pull a level from level pool {name}, but it has no scenes assigned!");
            }

            return levelScenes.OrderBy((s) => Random.value);
        }
    }
}