using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Parsing;
using Abilities.Shapes;
using Abilities.VFX;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using TenetStatuses;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject, ISerializationCallbackReceiver
    {
        [Tooltip("The name of the ability shown to player, in the UI. If left empty, the file name will be used.")]
        [SerializeField] private string displayName;
        [Tooltip("Complete description of the ability")]
        [SerializeField, TextArea(4, 8)] private string description;
        [SerializeField] private BasicShapeData shape;
        [HideInInspector, SerializeField] private bool excludeUserFromTargets = true;
        // [SerializeField] private int knockback;
        [SerializeField] [Range(-5,5)] private int speed;
        [SerializeField] private AbilitySpeedType speedType;
        [SerializeField] private TenetType representedTenet;

        [FormerlySerializedAs("targetEffects")]
        [SerializeField] private List<Effect> effects;
        [SerializeField] private GameObject visualEffects;
        // We're not using this anymore, but we are supporting backwards compat so keep it here
        [HideInInspector, SerializeField] private List<Effect> userEffects;

        /// <summary>
        /// The name of the ability, that the player can refer to. And shown in the UI.
        /// </summary>
        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        /// <summary>
        /// A complete description of the ability.
        /// </summary>
        public string Description => description;
        /// <summary>
        /// Describes what and how the ability can hit units.
        /// </summary>
        public IShape Shape => shape;
        /// <summary>
        /// The speed which will be added on top of Abilities.
        /// </summary>
        public int Speed => speed;
        /// <summary>
        /// The type of speed which will be shown in the UI.
        /// </summary>
        public AbilitySpeedType SpeedType => speedType;
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
            // user.AddSpeed(speed);
            var targets = shape.GetTargets(originCoordinate, direction);
            AbilityParser abilityParser = new AbilityParser(user, effects, targets.OfType<IAbilityUser>());
            abilityParser.ParseAll();
            abilityParser.ApplyChanges();
            SpawnVisualEffects(visualEffects, targets);
        }

        private void PlayAudioEffects(IAbilityUser user)
        {
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();
            
            switch (user.Name)
            {
                case "Niles":
                    commandManager.ExecuteCommand(new PostSound("Play_Niles_Ability"));
                    break;
                case "Helena":
                    commandManager.ExecuteCommand(new PostSound("Play_Helena_Ability"));
                    break;
                case "Estelle":
                    commandManager.ExecuteCommand(new PostSound("Play_Estelle_Ability"));
                    break;
                default:
                    commandManager.ExecuteCommand(new PostSound("Play_Brush_Stroke"));
                    break;
            }
        }

        private void SpawnVisualEffects(GameObject vfx, IEnumerable<GridObject> targets)
        {
            if (vfx == null)
            {
                Debug.LogWarning(this.name + " does not have an assigned visual ability" +
                                 "effect. No ability vfx will play");
                return;
            }
            
            foreach (var target in targets)
            {
                if(target.GetType() == typeof(Obstacle))
                    continue;
                
                Vector2 targetPos = target.transform.position;
                Vector2 startPos = Vector2.up + targetPos;

                Stroke strokeEffect = Instantiate(vfx, startPos, Quaternion.identity)
                    .GetComponent<Stroke>();
                strokeEffect.Execute(targetPos);
            }
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
            // user.AddSpeed(-speed);
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
