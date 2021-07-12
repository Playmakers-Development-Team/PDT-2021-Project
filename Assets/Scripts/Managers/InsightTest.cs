using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class InsightTest : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TextMeshProUGUI text;
    private UnitManager unitManager;
    void Awake()
    {
        unitManager = ManagerLocator.Get<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Insight: " + unitManager.Insight;
    }
}
