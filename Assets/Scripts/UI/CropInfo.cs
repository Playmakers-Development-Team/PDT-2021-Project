using UnityEngine;

namespace UI
{
    public class CropInfo : ScriptableObject
    {
        [SerializeField] private Texture image;
        [SerializeField] private Rect uvRect = new Rect(0, 0, 1, 1);
        [SerializeField] private Color colour = Color.white;


        internal Texture Image => image;
        internal Rect UVRect => uvRect;
        internal Color Colour => colour;
        
        
        public void Apply(Texture newImage, Rect newRect)
        {
            image = newImage;
            uvRect = newRect;
        }
    }
}
