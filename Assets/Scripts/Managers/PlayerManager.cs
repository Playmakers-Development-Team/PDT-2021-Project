using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : IManager
    {
        private readonly List<IUnit> players = new List<IUnit>();

        
        public int Count => players.Count;

        
        public IUnit Spawn(GameObject prefab, Vector2Int gridPosition)
        {
            IUnit unit = Unit.Spawn(prefab, gridPosition);
            
            if (!(unit is PlayerUnit))
                return null;
            
            players.Add(unit);
            
            return unit;
        }
        
        public IUnit Spawn(string playerName, Vector2Int gridPosition)
        {
            IUnit unit = Unit.Spawn(playerName, gridPosition);

            if (!(unit is PlayerUnit))
                return null;
            
            players.Add(unit);
            
            return unit;
        }
        
        public void Clear()
        {
            players.Clear();
        }

        public void Clean()
        {
            for (int i = players.Count; i >= 0; i--)
            {
                if (players[i] is null)
                    players.RemoveAt(i);
            }
        }
    }
}
