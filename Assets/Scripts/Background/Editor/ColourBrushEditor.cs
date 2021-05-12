using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background.Editor
{
    [CustomEditor(typeof(ColourBrush))]
    public class ColourBrushEditor : UnityEditor.Tilemaps.GridBrushEditorBase
    {
        public override GameObject[] validTargets
        {
            get
            {
                Tilemap[] maps = FindObjectsOfType<Tilemap>();
                List<GameObject> washMaps = new List<GameObject>();

                foreach (Tilemap map in maps)
                {
                    if (((1 << map.gameObject.layer) & Settings.WashLayer) == Settings.WashLayer)
                        washMaps.Add(map.gameObject);
                }

                return washMaps.Count == 0 ? null : washMaps.ToArray();
            }
        }
    }
}
