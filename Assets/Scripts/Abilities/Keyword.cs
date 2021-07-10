using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Keyword", menuName = "Ability Keyword", order = 250)]
    public class Keyword : ScriptableObject
    {
        [Tooltip("The name of the keyword that should be shown in game")]
        [SerializeField] private string displayName;
        [Tooltip("Should the keyword be shown when the ability is hovered by the player")]
        [SerializeField] private bool isVisibleInGame = true;
        [Tooltip("Complete description of the keyword")]
        [SerializeField, TextArea(4, 8)] private string description;
        [SerializeField] private Effect effect;

        /// <summary>
        /// The name of the keyword that should be shown in game.
        /// </summary>
        public string DisplayName => displayName;
        /// <summary>
        /// Should the keyword be shown when the ability is hovered by the player.
        /// </summary>
        public bool IsVisibleInGame => isVisibleInGame;
        /// <summary>
        /// A complete description of the keyword.
        /// </summary>
        public string Description => description;
        public Effect Effect => effect;
    }
}
