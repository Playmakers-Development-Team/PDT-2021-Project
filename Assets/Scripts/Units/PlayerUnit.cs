using System;
using Units;
using GridObjects;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit
    {
        public PlayerUnit(
        int healthPoints,
        int movementActionPoints,
        int speed,
        Vector2Int position, 
        Stat dealDamageModifier,
        Stat takeDamageModifier,
        Stat takeKnockbackModifier
        ) : base(healthPoints,movementActionPoints,speed,position, dealDamageModifier, 
        takeDamageModifier, 
        takeKnockbackModifier) {}
        
    }
}
  

