using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Refactored
{
    public class AbilityCard : Element
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        internal Ability Ability { get; private set; }
        
        internal void Disable() => button.interactable = false;

        internal void Assign(Ability ability)
        {
            Ability = ability;
            
            // TODO: Add button functionality.
            // TODO: Add image functionality once abilities have icons.
            
            titleText.text = ability.name;
            descriptionText.text = ability.Description;
        }
    }
}
