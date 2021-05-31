using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    [CustomEditor(typeof(ColorDropParticleSystem))]
    public class ColorDropParticleRendererEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            ColorDropParticleSystem particleSystem = (ColorDropParticleSystem)target;

            
        }
    }
}
