using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu]
    public class MapData : ScriptableObject
    {
        public List<EncounterNode> encounterNodes;
        
        public void Initialise()
        {
            encounterNodes.ForEach(encounterNode => encounterNode.Initialise(encounterNodes));
        }
    }
}
