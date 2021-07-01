using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    public interface IColorSampler
    {
        Color SampleColorFromScreenSpace(Vector2 screenPos);
    }

    public class ColorDecalColorSampler : MonoBehaviour, IColorSampler
    {
        
        public Color SampleColorFromScreenSpace(Vector2 screenPos)
        {
            Debug.LogWarning("There is no color sampling implementation yet");
            return Color.cyan;
        }
    }
}