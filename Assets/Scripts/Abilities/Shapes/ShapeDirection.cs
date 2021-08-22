using UnityEngine;
using Utilities;

namespace Abilities.Shapes
{
    /// <summary>
    /// <p>A useful function for handling and determining or directing the direction of a shape or ability.
    /// Contains a bunch of conversion methods between the shape space and the world space.</p>
    ///
    /// <p>This class is designed to be immutable, for easier and more robust usage.</p>
    /// </summary>
    public readonly struct ShapeDirection
    {
        /// <summary>
        /// The vector in a normal xyz space
        /// </summary>
        public Vector2 ShapeVector { get; }
        /// <summary>
        /// The vector in an isometric space, this is in world space for an isometric game
        /// </summary>
        public Vector2 IsometricVector => Quaternion.AngleAxis(45, Vector3.forward) * ShapeVector;
        
        public OrdinalDirection OrdinalDirection => OrdinalDirectionUtility.From(Vector2.zero, ShapeVector);
        public CardinalDirection CardinalDirection => CardinalDirectionUtility.From(Vector2.zero, ShapeVector);

        private ShapeDirection(Vector2 shapeVector) => this.ShapeVector = shapeVector;

        /// <summary>
        /// Creates a shape direction from an IAbilityUser to another.
        /// </summary>
        public static ShapeDirection Towards(IAbilityUser from, IAbilityUser to) =>
            FromOrthogonal(to.Coordinate - from.Coordinate);

        /// <summary>
        /// <p>Creates a shape direction from regular a xyz space, the space a normal shape understand.
        /// <b>This is not world space since our game is isometric!</b></p>
        ///
        /// <p> This xyz space is where (0, 1) is up, (1, 0) is right. Remember that the game is isometric,
        /// so most of the time you want to use <see cref="ShapeDirection.FromIsometric"/></p>
        /// </summary>
        public static ShapeDirection FromOrthogonal(Vector2 directionVector) =>
            new ShapeDirection(directionVector);

        /// <summary>
        /// Creates a shape direction from isometric space.
        /// This is usually in world space since our game is isometric.
        /// </summary>
        public static ShapeDirection FromIsometric(Vector2 directionVector) =>
            new ShapeDirection(Quaternion.AngleAxis(-45, Vector3.forward) * directionVector);
        
        /// <summary>
        /// Create a shape direction from a <see cref="OrdinalDirection"/>
        /// </summary>
        public static ShapeDirection FromOrdinalDirection(OrdinalDirection ordinalDirection) =>
            FromOrthogonal(ordinalDirection.ToVector2());

        /// <summary>
        /// Create a shape direction from a <see cref="CardinalDirection"/>
        /// </summary>
        public static ShapeDirection FromCardinalDirection(CardinalDirection cardinalDirection) =>
            FromOrthogonal(cardinalDirection.ToVector2());
        
        /// <summary>
        /// A shape direction that does not point towards any direction.
        /// Some shapes actually don't care or have a direction, so this is where this comes in handy.
        /// </summary>
        public static readonly ShapeDirection None = new ShapeDirection(Vector2.zero);
    }
}
