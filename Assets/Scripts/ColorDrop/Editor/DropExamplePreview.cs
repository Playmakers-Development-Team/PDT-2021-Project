using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    [CustomPreview(typeof(ColorDropEditorWindow))]
    public class DropExamplePreview : ObjectPreview
    {
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            GUI.Label(r, target.name + " is being previews");
        }
    }
}
