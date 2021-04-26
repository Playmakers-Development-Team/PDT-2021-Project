using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : IManager
    {
        private readonly List<IUnit> players = new List<IUnit>();

        
        public int Count => players.Count;


        public void Clear()
        {
            players.Clear();
        }
        
        public IUnit Spawn<T>(Vector2Int gridPosition) where T : PlayerUnitData
        {
            IUnit unit = Unit.Spawn<T>(gridPosition);

            players.Add(unit);
            
            return unit;
        }
    }
}
