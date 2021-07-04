using TMPro;
using Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityCard : MonoBehaviour
    {
        [SerializeField] private Image abilityImg;
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private TextMeshProUGUI abilityDescription;

        public Ability Ability { get; private set; }

        public void SetAbility(Ability ability)
        {
            Ability = ability;
            gameObject.SetActive(ability != null);
            SetAbilityText(ability.name);
            abilityDescription.text = ability.Description;
        }

        private void SetAbilityText(string abilityName)
        {
            this.abilityName.text = abilityName;
        }

        public void HighlightAbility()
        {
            abilityImg.color = Color.red;
        }

        public void UnHighlightAbility()
        {
            abilityImg.color = Color.black;

        }
        
        
    }
}
