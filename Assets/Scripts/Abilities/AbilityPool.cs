using System.Collections.Generic;
using System.Linq;
using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "AbilityPool", menuName = "Ability Pool", order = 250)]
    public class AbilityPool : ScriptableObject
    {
        [SerializeField] private List<Ability> abilities;
        
        public IReadOnlyList<Ability> Abilities => abilities.AsReadOnly();

        public IEnumerable<Ability> PickAbilitiesByTenet(TenetType tenetType)
        {
            return abilities.Where(a => a.RepresentedTenet == tenetType);
        }
    }
}
