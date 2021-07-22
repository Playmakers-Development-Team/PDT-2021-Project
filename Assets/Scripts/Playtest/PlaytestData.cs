using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playtest
{
    
    [CreateAssetMenu(menuName = "PlaytestData")]
    public class PlaytestData : ScriptableObject
    {
        
        public Scene activeScene;
        [field : SerializeField]public int TimesMoved { get; set; }

        


    }
}
