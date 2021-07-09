using UnityEditor;

namespace Abilities.Shapes.Editor
{
    [CustomEditor(typeof(BasicShapeData))]
    public class BasicShapeDataEditor : UnityEditor.Editor
    {
        private SerializedProperty shouldFollowMouseProperty;
        
        private void Awake()
        {
            shouldFollowMouseProperty = serializedObject.FindProperty("shouldFollowMouse");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BasicShapeData basicShapeData = (BasicShapeData) serializedObject.targetObject;
            
            if (basicShapeData.HasNoDirection)
            {
                EditorGUILayout.PropertyField(shouldFollowMouseProperty);
            }
            else
            {
                shouldFollowMouseProperty.boolValue = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
