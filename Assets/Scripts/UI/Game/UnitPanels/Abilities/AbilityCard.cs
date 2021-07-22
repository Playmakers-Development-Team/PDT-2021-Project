using Abilities;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityCard : PanelButton
    {
        internal Ability Ability { get; private set; }

        
        #region UIComponent

        protected override void OnSelected() => dialogue.abilitySelected.Invoke(Ability);

        #endregion


        #region Drawing
        
        internal void Assign(Ability ability)
        {
            Ability = ability;
            labelText.text = ability.name;
        }
        
        internal void Destroy() => Destroy(gameObject);
        
        #endregion
    }
}
