using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    [Serializable]
    public class EncounterNode
    {
        [SerializeField] private EncounterData encounterData;
        [SerializeField] private List<EncounterData> connectedEncounters;

        public EncounterData EncounterData => encounterData;

        private EncounterNodeState state = EncounterNodeState.Locked;
        [NonSerialized] private List<EncounterNode> connectedNodes = new List<EncounterNode>();

        public void Initialise(List<EncounterNode> allEncounterNodes)
        {
            foreach (var connectedEncounter in connectedEncounters)
            {
                connectedNodes.Add(allEncounterNodes.Find(
                    encounterNode => encounterNode.encounterData == connectedEncounter
                ));
            }
        }

        public override string ToString() => encounterData.name + " [" + state + "]";
    }
}
