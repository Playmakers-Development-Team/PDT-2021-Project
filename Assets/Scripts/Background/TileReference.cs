using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
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
    }
}
