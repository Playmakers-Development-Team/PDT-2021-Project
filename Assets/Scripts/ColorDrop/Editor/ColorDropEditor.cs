using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    [CustomEditor(typeof(ColorDropGenerator))]
    public class ColorDropEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ColorDropGenerator generator = (ColorDropGenerator)target;

            GUILayout.Space(20);
            if (GUILayout.Button("Generate Color Drop"))
            {
                Debug.Log("Button Triggered");
                generator.GenerateDrop();
            }
        }
    }
}
