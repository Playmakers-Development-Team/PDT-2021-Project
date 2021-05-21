using System;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        [Header("PlayerUnitStats")]
        [SerializeField] private int healthPoints;
        [SerializeField] private int movementActionPoints;
        [SerializeField] private float damageModifier;
        [SerializeField] private float defenceModifier;

        void Start()
        {
            
        }

        void TakeDamage(int amount)
        {
            data.CurrentHealth = amount;
        }


        //[Header("AvailableAbilities")]
        // private List<Abilities> availableAbilities;
        


    }
}