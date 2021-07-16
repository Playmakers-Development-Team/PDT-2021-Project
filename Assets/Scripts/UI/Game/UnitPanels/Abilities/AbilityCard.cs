using Abilities;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityCard : UIComponent<GameDialogue>
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private bool clicked;

        
        internal Ability Ability { get; private set; }

        
        #region UIComponent
        
        protected override void Subscribe()
        {
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
        }

        protected override void Unsubscribe()
        {
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
        }
        
        #endregion
        
        
        #region Listeners
        
        public void OnCardClicked()
        {
            if (!clicked)
            {
                dialogue.abilitySelected.Invoke(Ability);
                button.image.color = Color.green;
                clicked = true;
            }
            else
            {
                dialogue.abilityDeselected.Invoke(Ability);
            }
        }

        private void OnAbilityDeselected(Ability ability)
        {
            clicked = false;
            button.image.color = Color.white;
        }

        #endregion
        
        
        #region Drawing
        
        internal void Assign(Ability ability)
        {
            Ability = ability;
            
            titleText.text = ability.name;
            descriptionText.text = ability.Description;
        }
        
        internal void Destroy() => Destroy(gameObject);
        
        #endregion
    }
}
