﻿using System.Collections.Generic;
using Abilities;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout
{
    public class UnitCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] protected Image render;
        [SerializeField] protected List<Image> abilityRender = new List<Image>();
        [SerializeField] private List<Ability> abilities = new List<Ability>();

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.panelSwap.Invoke(AbilityLoadoutPanelType.AbilitySelect);
        }
        
        #endregion
        
        #region Drawing
        
        internal void Redraw(AbilityLoadoutDialogue.UnitInfo unit)
        {
            render.sprite = unit.Render;
            abilities = unit.Unit.Abilities;
        }

        #endregion
    }
}
