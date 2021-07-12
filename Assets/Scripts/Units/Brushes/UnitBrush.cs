using System.Linq;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Units.Brushes
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Unit Brush")]
    public class UnitBrush : PrefabBrush
    {
        private enum Units
        {
            Estelle,
            Helena,
            Niles,
            Enemy,
            Obstacle
        };
        
        [Tooltip("List of all units/obstacles that can be placed in the scene")]
        [SerializeField] private GameObject[] m_Prefabs;
        [SerializeField] private int curIndex = 0;
        [SerializeField] private Quaternion prefabRotation;
        
        [SerializeField] private Units units;

        public bool EraseAnyObject { get; set; }

        
        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            var angle = layout == GridLayout.CellLayout.Hexagon ? 60f : 90f;
            prefabRotation = Quaternion.Euler(0f, 0f,
                direction == RotationDirection.Clockwise
                    ? prefabRotation.eulerAngles.z + angle
                    : prefabRotation.eulerAngles.z - angle);
        }
        
        /// <summary>
        /// Gets the correct index from the currently selected unit from the enum
        /// </summary>
        /// <returns>The index of the selected unit in the m_prefabs array</returns>
        private int GetIndexFromUnits()
        {
            for (int i = 0; i < m_Prefabs.Length; i++)
            {
                if (m_Prefabs[i].name == units.ToString())
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Paints GameObject from containing Prefab into a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (!brushTarget.CompareTag("LevelTilemap") && !brushTarget.CompareTag("UnitPalette"))
            {
                Debug.LogWarning("Do not use this tilemap. Use 'Level Tilemap' instead");
                return;
            }
         
            curIndex = GetIndexFromUnits();

            var objectsInCell = GetObjectsInCell(grid, brushTarget.transform, position);
            var existPrefabObjectInCell = objectsInCell.Any(objectInCell =>
                PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) ==
                m_Prefabs[curIndex]);

            if (!existPrefabObjectInCell)
                base.InstantiatePrefabInCell(grid, brushTarget, position, m_Prefabs[curIndex],
                    prefabRotation);
           
        }

        /// <summary>
        /// Paints the PrefabBrush instance's prefab into all positions specified by the box fill tool.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the box fill operation. By default the currently selected GameObject.</param>
        /// <param name="bounds">The cooridnate boundries to fill.</param>
        public override void BoxFill(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                Paint(grid, brushTarget, tilePosition);
        }

        public override void BoxErase(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                Erase(grid, brushTarget, tilePosition);
        }

        /// <summary>
        /// If "Erase Any Objects" is true, erases any GameObjects that are in a given position within the selected layers.
        /// If "Erase Any Objects" is false, erases only GameObjects that are created from owned Prefab in a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab erasing functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            curIndex = GetIndexFromUnits();

            foreach (var objectInCell in GetObjectsInCell(grid, brushTarget.transform, position))
                Undo.DestroyObjectImmediate(objectInCell);
            
        }

        /// <summary>
        /// Pick prefab from selected Tilemap, given the coordinates of the cells.
        /// </summary>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position,
                                  Vector3Int pickStart)
        {
            curIndex = GetIndexFromUnits();

            if (brushTarget == null)
                return;
            

            foreach (var objectInCell in GetObjectsInCell(gridLayout, brushTarget.transform,
                position.position))
            {
                if (objectInCell)
                {
                    m_Prefabs[curIndex] =
                        PrefabUtility.GetCorrespondingObjectFromSource(objectInCell);
                }
            }
        }
    }

    [CustomEditor(typeof(UnitBrush))]
    public class UnitBrushEditor : BasePrefabBrushEditor
    {
        private UnitBrush unitBrush => target as UnitBrush;
        private SerializedProperty m_Prefab;
    
        /// <summary>
        /// OnEnable for the UnitPrefabBrusheditor
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Prefab = m_SerializedObject.FindProperty(nameof(m_Prefab));
        }
    
        /// <summary>
        /// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
        /// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            const string eraseAnyObjectsTooltip =
                "If true, erases any GameObjects that are in a given position " +
                "within the selected layers with Erasing. " +
                "Otherwise, erases only GameObjects that are created " +
                "from owned Prefab in a given position within the selected layers with Erasing.";
    
            base.OnPaintInspectorGUI();
    
            m_SerializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(m_Prefab, true);
            unitBrush.EraseAnyObject = EditorGUILayout.Toggle(
                new GUIContent("Erase Any Objects", eraseAnyObjectsTooltip),
                unitBrush.EraseAnyObject);
    
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
