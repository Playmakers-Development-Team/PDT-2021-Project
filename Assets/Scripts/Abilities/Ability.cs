using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Parsing;
using Abilities.Shapes;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using TenetStatuses;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject, ISerializationCallbackReceiver
    {
        [Tooltip("Complete description of the ability")]
        [SerializeField, TextArea(4, 8)] private string description;
        [Tooltip("Is there an ability that this one can be upgraded to? Otherwise keep it's okay to keep empty/None")]
        [SerializeField] private Ability upgradedAbility;
        [SerializeField] private BasicShapeData shape;
        [HideInInspector, SerializeField] private bool excludeUserFromTargets = true;
        // [SerializeField] private int knockback;
        [SerializeField] [Range(-5,5)] private int speed;
        [SerializeField] private TenetType representedTenet;

        [FormerlySerializedAs("targetEffects")]
        [SerializeField] private List<Effect> effects;
        // We're not using this anymore, but we are supporting backwards compat so keep it here
        [HideInInspector, SerializeField] private List<Effect> userEffects;

        /// <summary>
        /// A complete description of the ability.
        /// </summary>
        public string Description => description;
        /// <summary>
        /// The ability that this ability can upgrade into. MAY BE NULL if this ability can't be upgraded.
        /// </summary>
        public Ability UpgradedAbility => upgradedAbility;
        /// <summary>
        /// Describes what and how the ability can hit units.
        /// </summary>
        public IShape Shape => shape;
        /// <summary>
        /// The speed which will be added on top of Abilities
        /// </summary>
        public int Speed => speed;
        /// <summary>
        /// The tenet that this ability represents. This would be shown in the UI.
        /// </summary>
        public TenetType RepresentedTenet => representedTenet;
        /// <summary>
        /// All keywords that should be shown in game which is used by this ability.
        /// Keywords with the same display names are not shown more than once.
        /// </summary>
        public IEnumerable<Keyword> AllKeywords => AllDefinedKeywords
            .Where(k => k.IsVisibleInGame)
            .GroupBy(k => k.DisplayName)
            .Select(group => group.First());
        /// <summary>
        /// All keywords used by this ability regardless whether they should be shown to
        /// the player or not.
        /// </summary>
        [Obsolete("Use AllKeywords")]
        public IEnumerable<Keyword> AllVisibleKeywords => AllKeywords;
        /// All keywords used by this ability regardless whether they should be shown to
        /// the player or not.
        public IEnumerable<Keyword> AllDefinedKeywords => TargetKeywords.Concat(UserKeywords);

        private IEnumerable<Keyword> TargetKeywords => effects.SelectMany(e => e.Keywords);
        private IEnumerable<Keyword> UserKeywords => userEffects.SelectMany(e => e.Keywords);

        internal void Use(IAbilityUser user, Vector2Int originCoordinate, ShapeDirection direction)
        {
            user.AddSpeed(speed);
            var targets = shape.GetTargets(originCoordinate, direction);
            AbilityParser abilityParser = new AbilityParser(user, effects, targets.OfType<IAbilityUser>());
            abilityParser.ParseAll();
            abilityParser.ApplyChanges();
        }
        
        public IEnumerable<IVirtualAbilityUser> ProjectAbilityUsers(IAbilityUser user, Vector2Int originCoordinate, ShapeDirection direction)
        {
            var targets = shape.GetTargets(originCoordinate, direction);
            AbilityParser abilityParser = new AbilityParser(user, effects, targets.OfType<IAbilityUser>());
            abilityParser.ParseAll();
            return abilityParser.Targets.Prepend(abilityParser.User);
        }
        
        internal void Undo(IAbilityUser user, Vector2Int originCoordinate, ShapeDirection direction)
        {
            user.AddSpeed(-speed);
            var targets = shape.GetTargets(originCoordinate, direction);
            AbilityParser abilityParser = new AbilityParser(user, effects, targets.OfType<IAbilityUser>());
            abilityParser.UndoAll();
            abilityParser.ApplyChanges();
        }

        public void OnBeforeSerialize()
        {
            // May be null when we are just creating the object
            if (userEffects != null)
            {
                // cache array, prevent modification exceptions
                var userEffectsCopy = userEffects.ToArray();

                if (!excludeUserFromTargets)
                {
                    foreach (Effect targetEffect in effects)
                        targetEffect.affectUser = true;

                    excludeUserFromTargets = true;
                }

                foreach (Effect userEffect in userEffectsCopy)
                {
                    userEffects.Remove(userEffect);
                    userEffect.affectTargets = false;
                    userEffect.affectUser = true;
                    effects.Add(userEffect);
                }
                
#if UNITY_EDITOR
                if (userEffectsCopy.Length > 0)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public void OnAfterDeserialize() {}
    }
}
