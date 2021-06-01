using TMPro;
using Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI abilityName;
        
        public Ability Ability { get; private set; }

        public void SetAbility(Ability ability)
        {
            Ability = ability;
            gameObject.SetActive(ability != null);
            SetAbilityText(ability.name);
        }

        private void SetAbilityText(string abilityName)
        {
            this.abilityName.text = abilityName;
        }
    }
}
