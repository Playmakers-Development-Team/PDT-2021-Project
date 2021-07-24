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
        [SerializeField] private bool startingNode;
        
        public EncounterData EncounterData => encounterData;

        public EncounterNodeState State { get; set; } = EncounterNodeState.Locked;

        public List<EncounterNode> ConnectedNodes { get; } = new List<EncounterNode>();

        public void Initialise(List<EncounterNode> allEncounterNodes)
        {
            // TODO: This might be doubling up with code in MapData
            foreach (var connectedEncounter in connectedEncounters)
            {
                ConnectedNodes.Add(allEncounterNodes.Find(
                    encounterNode => encounterNode.encounterData == connectedEncounter
                ));
            }

            if (startingNode)
                State = EncounterNodeState.Available;
        }

        public override string ToString() => encounterData.name + " [" + State + "]";
    }
}
