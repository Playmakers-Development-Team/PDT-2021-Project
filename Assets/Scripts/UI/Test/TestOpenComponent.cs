using UnityEngine;

namespace UI.Test
{
    public class TestOpenComponent : UIComponent<TestDialogue>
    {
        [SerializeField] private GameObject popupPrefab;

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        public void OnOpenClick()
        {
#if UNITY_EDITOR
            UnityEditor.PrefabUtility.InstantiatePrefab(popupPrefab, dialogue.transform.parent);
#else
            Instantiate(popupPrefab, dialogue.transform.parent);
#endif
        }
    }
}
