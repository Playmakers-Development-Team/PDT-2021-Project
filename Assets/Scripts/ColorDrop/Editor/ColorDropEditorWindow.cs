using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    public class ColorDropEditorWindow : EditorWindow
    {
        Texture2D gameObject;
        UnityEditor.Editor gameObjectEditor;

        public static void Open(ColorDropSettings settingsObject)
        {
            ColorDropEditorWindow window = GetWindow<ColorDropEditorWindow>(" ColorDrop Settings Window");
        }

        private void OnGUI()
        {
            //gameObject = (Texture2D)EditorGUILayout.ObjectField(gameObject, typeof(Texture2D), true);
            gameObject = new Texture2D(256, 256);

            GUIStyle bgColor = new GUIStyle();
            bgColor.normal.background = Texture2D.grayTexture;

            if (gameObject != null)
            {
                if (gameObjectEditor == null)
                    gameObjectEditor = UnityEditor.Editor.CreateEditor(gameObject);

                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
            }
        }
    }
}
