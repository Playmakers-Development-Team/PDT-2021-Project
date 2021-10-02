using System;
using Commands;
using Managers;
using UnityEngine;

namespace Game.Map
{
    /// <summary>
    /// A simple script that activates a linear run of the map
    /// </summary>
    public class LinearMapRunner : MonoBehaviour
    {
        [Tooltip("Should we automatically tell the game manager to run the map in a linear fashion?")]
        public bool shouldRunLinearMap = true;
        public MapData map;

        private GameManager gameManager;
        
        private void Awake()
        {
            gameManager = ManagerLocator.Get<GameManager>();
        }

        private void Start()
        {
            if (shouldRunLinearMap && map != null)
                gameManager.RunLinearMap(map).Forget();
        }
    }
}
