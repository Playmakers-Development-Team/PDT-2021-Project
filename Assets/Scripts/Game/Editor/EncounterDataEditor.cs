using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(EncounterData))]
    public class EncounterDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox(
                "You may either set this encounter to have a specific level OR to pull a random "
                + "level from the level pool. \n"
                + "\nDO NOT PUT BOTH. If both is set, the specific level will always be chosen.",
                MessageType.Info);
        }
    }
}
