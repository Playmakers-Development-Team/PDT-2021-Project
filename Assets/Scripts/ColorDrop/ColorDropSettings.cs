using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    [CreateAssetMenu(menuName = "Settings/Color Drop Setting")]
    public class ColorDropSettings : ScriptableObject
    {
        public int scaleWidth = 256;
        public int scaleHeight = 256;

        [SerializeField]
        public ColorSelection[] colorSelections;
        [SerializeField]
        public SDFSelection[] sdfSelections;
        public Texture2D[] textureSelections;

        public void CreateNewColorSelection()
        {
            ColorSelection[] tempArray = new ColorSelection[colorSelections.Length + 1];
            ColorSelection selection = new ColorSelection();

            for (int i = 0; i < colorSelections.Length; i++){
                tempArray[i] = colorSelections[i];
            }

            tempArray[tempArray.Length - 1] = selection;
            colorSelections = tempArray;
        }
        
        public void CreateNewSDFSelection()
        {
            SDFSelection[] tempArray = new SDFSelection[sdfSelections.Length + 1];
            SDFSelection selection = new SDFSelection();

            for (int i = 0; i < sdfSelections.Length; i++){
                tempArray[i] = sdfSelections[i];
            }

            tempArray[tempArray.Length - 1] = selection;
            sdfSelections = tempArray;
        }

        public void CreateNewTextureSelection()
        {
            Texture2D[] tempArray = new Texture2D[textureSelections.Length + 1];
            Texture2D selection = textureSelections.Length == 0 ? Texture2D.whiteTexture : textureSelections[textureSelections.Length - 1];

            for (int i = 0; i < textureSelections.Length; i++)
            {
                tempArray[i] = textureSelections[i];
            }

            tempArray[tempArray.Length - 1] = selection;
            textureSelections = tempArray;
        }
    }
}
