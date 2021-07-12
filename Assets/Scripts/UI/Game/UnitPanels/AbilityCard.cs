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
        
        internal void Assign(Ability ability)
        {
            Ability = ability;
            
            titleText.text = ability.name;
            descriptionText.text = ability.Description;
        }

        private void OnEnable()
        {
            manager.abilityDeselected.AddListener(OnAbilityDeselect);
            manager.unitDeselected.AddListener(OnUnitDeselected);
        }

        private void OnDisable()
        {
            manager.abilityDeselected.RemoveListener(OnAbilityDeselect);
            manager.unitDeselected.AddListener(OnUnitDeselected);
        }

        public void OnCardClicked()
        {
            clicked = !clicked;
            if (clicked)
            {
                // TODO: this should call manager.abilityDeselected.Invoke(manager.SelectedAbility)...
                manager.abilityDeselected.Invoke(Ability);
                manager.abilitySelected.Invoke(Ability);
                button.image.color = Color.green;
            }
            else
            {
                manager.abilityDeselected.Invoke(Ability);
            }
        }

        private void OnAbilityDeselect(Ability ability)
        {
            button.image.color = Color.white;
        }

        private void OnUnitDeselected()
        {
            manager.abilityDeselected.Invoke(Ability);
        }

        internal void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
