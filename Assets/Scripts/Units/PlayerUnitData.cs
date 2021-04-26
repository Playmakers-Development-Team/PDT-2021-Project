using System;
using UnityEngine;

namespace Units
{
    [Serializable]
    public class PlayerUnitData : UnitDataBase
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int health;
        [Space]
        [SerializeField] private int maxDamage;
        [SerializeField] private int damage;
    }
}
