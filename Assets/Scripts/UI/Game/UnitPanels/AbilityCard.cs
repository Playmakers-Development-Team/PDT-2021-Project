using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityCard : Element
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private bool clicked;

        
        internal Ability Ability { get; private set; }
        
        
        internal void Disable() => button.interactable = false;

        internal void Assign(Ability ability)
        {
            Ability = ability;
            
            titleText.text = ability.name;
            descriptionText.text = ability.Description;
        }

        public void OnCardClicked()
        {
            clicked = !clicked;
            if (clicked)
                manager.abilitySelected.Invoke(Ability);
            else
                manager.abilityDeselected.Invoke(Ability);
        }

        internal void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
