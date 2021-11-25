using System;
using System.Collections.Generic;
using Units.Players;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private List<PlayerUnitData> unitData;
        [SerializeField] private List<SceneReference> visitedLevels;

        public SaveData(List<PlayerUnitData> unitData, List<SceneReference> visitedLevels)
        {
            this.unitData = unitData;
            this.visitedLevels = visitedLevels;
        }

        public List<PlayerUnitData> GetUnitData() => unitData;

        public List<SceneReference> GetVisitedLevels() => visitedLevels;
    }
}
