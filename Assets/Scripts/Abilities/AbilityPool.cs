using System.Collections.Generic;
using System.Linq;
using TenetStatuses;
using UnityEngine;

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
            abilities
                .Select(ability =>
                {
                    const float randomScale = 20;
                    float bias = ability.RepresentedTenet == tenetType ? Mathf.Max(1, pickTenetBias) : 1;
                    float score = UnityEngine.Random.Range(0, randomScale * bias);
                    return (ability, score);
                })
                .OrderBy(p => p.score)
                .Select(p => p.ability);

        private IEnumerable<Ability> GetAbilitiesFromTenet(TenetType tenetType) =>
            abilities.Where(a => a.RepresentedTenet == tenetType);
    }
}
