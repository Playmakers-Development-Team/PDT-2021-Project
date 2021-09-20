using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu]
    public class MapData : ScriptableObject
    {
        public SceneReference mapScene;
        public List<EncounterNode> encounterNodes;
        
        public void Initialise() =>
            encounterNodes.ForEach(encounterNode => encounterNode.Initialise(encounterNodes));

        public void EncounterCompleted(EncounterData encounterData)
        {
            var encounterNode = GetEncounterNode(encounterData);

            if (!encounterNodes.Contains(encounterNode))
            {
                Debug.LogWarning("Could not complete encounter. That encounter does not exist on the current map.");
                return;
            }

            encounterNode.State = EncounterNodeState.Completed;

            foreach (var connectedNode in encounterNode.ConnectedNodes)
                connectedNode.State = EncounterNodeState.Available;
        }

        private EncounterNode GetEncounterNode(EncounterData encounterData) =>
            encounterNodes.FirstOrDefault(n => n.EncounterData == encounterData);
    }
}
