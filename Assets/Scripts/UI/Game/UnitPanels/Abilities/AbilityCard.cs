using System.Collections.Generic;
using Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityCard : PanelButton
    {
        [SerializeField] private Image tenetIcon;
        // TODO: Remove this and replace with global reference...
        [SerializeField] private List<Sprite> tenetIcons;
        
        
        internal Ability Ability { get; private set; }

        
        #region UIComponent

        protected override void OnSelected() => dialogue.abilitySelected.Invoke(Ability);

        protected override void OnDeselected() => dialogue.abilityDeselected.Invoke(Ability);

        #endregion
        
        
        #region Listeners
        
        #endregion


        #region Drawing
        
        internal void Assign(Ability ability)
        {
            Ability = ability;
            labelText.text = ability.DisplayName;
            tenetIcon.sprite = tenetIcons[(int) ability.RepresentedTenet];
        }
        
        internal void Destroy() => DestroyImmediate(gameObject);
        
        #endregion
        
        
        #region PanelButton

        protected override void OnHoverEnter() => dialogue.abilityHoverEnter.Invoke(this);

        protected override void OnHoverExit() => dialogue.abilityHoverExit.Invoke(this);

        #endregion
    }
}
