using UnityEngine;

namespace Background
{
    public class TileSpriteReference : ScriptableObject
    {
        [SerializeField] private Sprite line;
        [SerializeField] private Sprite colour;
        [SerializeField] private Sprite fill;

        
        public void Initialize(Sprite lineSprite, Sprite colourSprite, Sprite fillSprite)
        {
            line = lineSprite;
            colour = colourSprite;
            fill = fillSprite;
        }
    }
}
