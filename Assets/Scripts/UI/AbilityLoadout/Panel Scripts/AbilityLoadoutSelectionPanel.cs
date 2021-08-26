using System.Collections.Generic;
using UI.AbilityLoadout.Abilities;
using UI.AbilityLoadout.Unit;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Panel_Scripts
{
    public class AbilityLoadoutSelectionPanel : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private UnitCard unitCard;
        [SerializeField] private UnitAbilitiesCard abilitiesCard;
        [SerializeField] private NewAbilityOptions newAbilityOptions;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion

        #region Drawing
        
        internal void Redraw(AbilityLoadoutDialogue.UnitInfo unit)
        {
            unitCard.Redraw(unit);
            abilitiesCard.Redraw(unit.AbilityInfo);
            newAbilityOptions.Redraw(unit.Unit.Tenet);
        }
        
        #endregion
    }
}
