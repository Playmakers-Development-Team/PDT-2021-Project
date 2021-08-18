using System.Collections.Generic;
using Abilities;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout
{
    public class UnitCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private Sprite render;
        [SerializeField] private List<Ability> unitAbilities = new List<Ability>();

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
        
        internal void Assign(AbilityLoadoutDialogue.UnitInfo unit)
        {
            render = unit.Render;
            unitAbilities = unit.Unit.Abilities;
        }

        #endregion
    }
}
