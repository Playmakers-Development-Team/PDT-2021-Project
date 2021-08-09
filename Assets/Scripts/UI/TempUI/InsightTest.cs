using Managers;
using TMPro;
using Turn;
using UnityEngine;

namespace UI.TempUI
{
    // TODO: Can be deleted once proper insight UI is implemented 
    public class InsightTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private TurnManager turnManager;

        private void Awake() => turnManager = ManagerLocator.Get<TurnManager>();

        private void Update() => text.text = "Insight: " + turnManager.Insight.Value;
    }
}
