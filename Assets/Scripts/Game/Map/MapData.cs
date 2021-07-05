using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu]
    public class MapData : ScriptableObject
    {
        public List<EncounterNode> encounterNodes;
        private EncounterNode currentEncounterNode;

        public void Initialise()
        {
            encounterNodes.ForEach(encounterNode => encounterNode.Initialise(encounterNodes));

            currentEncounterNode = encounterNodes[0];
        }
    }
}
