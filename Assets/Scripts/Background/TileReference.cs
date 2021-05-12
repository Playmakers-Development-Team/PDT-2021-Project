using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
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

        public bool HasTile(Tile tile) => line.Equals(tile) || colour.Equals(tile) || fill.Equals(tile) || preview.Equals(tile);

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
