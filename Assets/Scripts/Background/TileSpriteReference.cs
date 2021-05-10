using UnityEngine;

namespace Background
{
    public class TileSpriteReference : ScriptableObject
    {
        [SerializeField] private Sprite line;
        [SerializeField] private Sprite colour;
        [SerializeField] private Sprite fill;
        [SerializeField] private Sprite preview;

        
        public void Initialize(Sprite lineSprite, Sprite colourSprite, Sprite fillSprite, Sprite previewSprite)
        {
            line = lineSprite;
            colour = colourSprite;
            fill = fillSprite;
            preview = previewSprite;
        }
    }
}
