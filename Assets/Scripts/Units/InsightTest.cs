using System;
using Managers;
using TMPro;
using Units.Players;
using UnityEngine;

namespace Units
{
    // TODO delete this
    [Obsolete]
    public class InsightTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private PlayerManager playerManager;

        private void Awake() => playerManager = ManagerLocator.Get<PlayerManager>();

        private void Update() => text.text = "Insight: " + playerManager.Insight.Value;
    }
}
