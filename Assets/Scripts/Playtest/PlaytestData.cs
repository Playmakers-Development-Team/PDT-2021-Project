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

        public List<Tuple<string, string>> Entries = new List<Tuple<string, string>>();

        public string InitialUnitOrder { get; set; }


    }
}
