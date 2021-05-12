using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorDropGenerator))]
public class ColorDropEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ColorDropGenerator generator = (ColorDropGenerator)target;

        if (GUILayout.Button("Generate Color Drop"))
        {
            Debug.Log("Button Triggered");
            generator.GenerateDrop();
        }
    }
}
