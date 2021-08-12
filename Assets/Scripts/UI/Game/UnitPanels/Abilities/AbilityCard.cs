using Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityCard : PanelButton
    {
        [SerializeField] private Image tenetIcon;
        
        
        internal Ability Ability { get; private set; }

        
        #region UIComponent

        protected override void OnSelected() => dialogue.abilitySelected.Invoke(Ability);

        protected override void OnDeselected() => dialogue.abilityDeselected.Invoke(Ability);

        #endregion


        #region Drawing
        
        internal void Assign(Ability ability)
        {
            Ability = ability;
            labelText.text = ability.name;
        }
        
        internal void Destroy() => DestroyImmediate(gameObject);
        
        #endregion
    }
}
