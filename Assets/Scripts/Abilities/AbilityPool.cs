using System;
using System.Collections.Generic;
using System.Linq;
using TenetStatuses;
using UnityEngine;
using Utilities;

namespace Abilities
{
    [CreateAssetMenu(fileName = "AbilityPool", menuName = "Ability Pool", order = 250)]
    public class AbilityPool : ScriptableObject
    {
        [Tooltip("Should the abilities that this pool provides only belong to the tenet specified? Otherwise, it instead just biases it")]
        [SerializeField] private bool pickFromSpecificTenetOnly;
        [Tooltip("How many times more often is an ability from a specific tenet should be picked up? If set to 1, then there will be no bias. Make sure to have 'Pick From Specific Tenet Only' disabled")]
        [SerializeField, Min(1)] private float pickTenetBias = 3;
        
        [SerializeField] private List<Ability> abilities;
        
        public IReadOnlyList<Ability> Abilities => abilities.AsReadOnly();

        public IEnumerable<Ability> PickAbilitiesByTenet(TenetType tenetType)
        {
            if (pickFromSpecificTenetOnly)
            {
                return GetAbilitiesFromTenet(tenetType);
            }
            else
            {
                return GetAbilitiesWithTenetBias(tenetType);
            }
        }

        private IEnumerable<Ability> GetAbilitiesWithTenetBias(TenetType tenetType) =>
            new WeightedBag<Ability>()
                .AddRange(Abilities, ability => (int)(ability.RepresentedTenet == tenetType ? Mathf.Max(1, pickTenetBias) : 1))
                .PullSortedOrder();

        private IEnumerable<Ability> GetAbilitiesFromTenet(TenetType tenetType) =>
            abilities.Where(a => a.RepresentedTenet == tenetType);
        
        public Ability PickAbilitiesByName(string name)
        {
            foreach (var ability in abilities)
            {
                if (ability.name.Equals(name))
                    return ability;
            }

            return null;
        }
        
        public Ability PickUpgrade(Ability ability) => PickAbilitiesByName(ability.DisplayName);

        public Ability PickUpgradeForName(string name) => PickAbilitiesByName(name + "+");
    }
}
