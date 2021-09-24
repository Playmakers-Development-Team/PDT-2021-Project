using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UI.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class LoseMenuDialogue : Dialogue
{

    [SerializeField] private float loadTime;
    
    [Tooltip("Time should be in miliseconds and is a countdown i.e. 3000 for 3 seconds, 10000 for 10 seconds")]
    [SerializeField] private int animationSpeed;

    [SerializeField] private TextMeshProUGUI restartLevelText;
    [SerializeField] private TextMeshProUGUI tipText;

    
    [SerializeField] private List<string> LoseScreenTips = new List<string>();
    
    private UIManager uiManager;

    protected override void OnDialogueAwake()
    {
        
        uiManager = ManagerLocator.Get<UIManager>();
        uiManager.Add(this);    
        
        
        RunAnimation();
        tipText.text = "Tip: ";
        tipText.text += GetRandomTip();
    }

    protected override void OnClose() {}

    protected override void OnPromote() {}

    protected override void OnDemote() {}


    internal async UniTask RunAnimation()
    {
        while (loadTime > 0)
        {
            restartLevelText.text = "Restarting Level";
            for (int i = 0; i < 3; i++)
            {
                restartLevelText.text += ".";
                await UniTask.Delay(animationSpeed);
            }
        }
    }
  
    
    internal string GetRandomTip() => LoseScreenTips[UnityEngine.Random.Range(0, LoseScreenTips.Count - 1)];

    private void Update()
    {
        loadTime -= Time.deltaTime;

        if (loadTime < 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    } 
    

    
    
}
