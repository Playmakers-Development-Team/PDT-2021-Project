using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playtest
{
    
    [CreateAssetMenu(menuName = "PlaytestData")]
    public class PlaytestData : ScriptableObject
    {
        
        public string activeScene;
        [field : SerializeField]public int TimesMoved { get; set; }
        
        public string InitialUnits { get; set; }
        
        public string EndStateUnits { get; set; }

        public string AbilityUsage { get; set; }

        public int AmountOfTurnsManipulated { get; set; }
        
        public int RoundCount { get; set; }
        
        public string RoundEntry { get; set; }
        
        public List<Tuple<string, string>> Entries = new List<Tuple<string, string>>();

        public string InitialUnitOrder { get; set; }
        
        


    }
}
