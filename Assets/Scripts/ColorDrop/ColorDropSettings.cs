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

        [Range(0, 1)]
        public float alpha = 1;


        [SerializeField]
        public ColorSelection[] colorSelections;
        [SerializeField]
        public SDFSelection[] sdfSelections;

        public Sprite[] textureShapes;
        public Texture[] textureDetails;
        public Texture[] texturePatterns;
        public GameObject particlePrefab;

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

        public Texture SelectRandomisedTextureDetail()
        {
            if (textureDetails == null || textureDetails.Length == 0) return new Texture2D(256, 256);

            return textureDetails[Random.Range(0, textureDetails.Length - 1)];
        }

        public Texture SelectRandomisedTexturePattern()
        {
            if (texturePatterns == null || texturePatterns.Length == 0) return new Texture2D(256, 256);

            return texturePatterns[Random.Range(0, texturePatterns.Length - 1)];
        }
    }
}
