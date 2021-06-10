using System.Collections;
using System.Collections.Generic;
using System;
using Managers;
using TMPro;
using Units;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {

        public ValueStat MovementActionPoints { get; protected set; }

        public ModifierStat TakeKnockbackModifier { get; protected set; }
        
        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);

        private GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            gridManager.AddGridObject(Coordinate, this);
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }
    }
}
