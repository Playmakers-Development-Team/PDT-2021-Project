using System;
using System.Collections.Generic;
using Abilities;
using Units;
using UnityEngine;

namespace UI.CombatEndUI
{
    [Serializable]
    public class LoadoutUnitInfo
    {
        [SerializeField] private CropInfo profileCropInfo;
        [SerializeField] private List<LoadoutAbilityInfo> abilityInfo;
        
        internal CropInfo ProfileCropInfo => profileCropInfo;

        public IUnit Unit { get; private set; }
        public List<LoadoutAbilityInfo> AbilityInfo { get; private set; }
        
        internal void SetUnit(IUnit newUnit) => Unit = newUnit;
        internal void SetAbilityInfo(List<LoadoutAbilityInfo> newAbilityInfo) => AbilityInfo = newAbilityInfo;
    }
    
    [Serializable]
    public class LoadoutAbilityInfo
    {
        [SerializeField] private Sprite render;
        
        internal Sprite Render
        {
            get => render;
            set => render = value;
        }

        public Ability Ability { get; internal set; }
        
        internal void SetAbility(Ability newUnit) => Ability = newUnit;
    }
}
