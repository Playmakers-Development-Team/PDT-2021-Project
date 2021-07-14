using System;
using UnityEngine;

namespace Abilities.Bonuses
{
    /// <summary>
    /// A more complete version of <see cref="CompositeBonus"/>.
    /// We use this class as the final bonus class to prevent recursion errors in the editor.
    /// </summary>
    [Serializable]
    public class WholeBonus : CompositeBonus
    {
        [CompositeChild((int) BonusType.Shape)]
        [SerializeField] private ShapeBonus shapeBonus;

        protected override IBonus ChildBonus => bonusType switch
        {
            BonusType.Shape => shapeBonus,
            _ => base.ChildBonus
        };
    }
}
