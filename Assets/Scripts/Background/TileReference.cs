using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
    /// <summary>
    /// A ScriptableObject that stores references to related tiles of each <see cref="TileType"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Background/Tile Reference", fileName = "NewTileReference", order = 50)]
    public class TileReference : ScriptableObject
    {
        [SerializeField] private Tile line;
        [SerializeField] private Tile colour;
        [SerializeField] private Tile fill;
        [SerializeField] private Tile preview;

        
        public void Initialize(Tile lineSprite, Tile colourSprite, Tile fillSprite, Tile previewSprite)
        {
            line = lineSprite;
            colour = colourSprite;
            fill = fillSprite;
            preview = previewSprite;
        }

        /// <summary>
        /// Get a Tile given a <see cref="TileType"/>.
        /// </summary>
        /// <param name="type">The type of tile to retrieve.</param>
        /// <returns>The tile of type <c>type</c>.</returns>
        public Tile GetTile(TileType type)
        {
            return type switch
            {
                TileType.Line => line,
                TileType.Colour => colour,
                TileType.Fill => fill,
                TileType.Preview => preview,
                _ => null
            };
        }

        /// <summary>
        /// Check if the TileReference has a given Tile.
        /// </summary>
        /// <param name="tile">The tile to check for.</param>
        /// <returns>True if a reference to <c>tile</c> is stored in this TileReference.</returns>
        public bool HasTile(Tile tile) => line.Equals(tile) || colour.Equals(tile) || fill.Equals(tile) || preview.Equals(tile);

        /// <summary>
        /// Gets the <see cref="TileType"/> corresponding to a given Tile.
        /// </summary>
        /// <param name="tile">The Tile to classify.</param>
        /// <returns>The TileType corresponding to <c>tile</c> if found, otherwise <c>0</c>.</returns>
        public TileType GetType(Tile tile)
        {
            if (tile.Equals(line))
                return TileType.Line;

            if (tile.Equals(colour))
                return TileType.Colour;

            if (tile.Equals(fill))
                return TileType.Fill;

            if (tile.Equals(preview))
                return TileType.Preview;

            return 0;
        }
    }
}
