using System.Collections;
using System.Collections.Generic;
using Audio;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Event = UI.Core.Event;
using UI.Core;
using UI.Game;
using UnityEngine.UI;

namespace UI.PauseScreen
{
public class PauseScreenDialogue : Dialogue
{

    [SerializeField] private GameObject exitConfirmationPrefab;
    private GameObject exitConfirmedPrefabInstance;
    [SerializeField] private GameObject pauseScreenInstance;

    public Dialogue GameDialgoue { get; set; }

    internal readonly Event buttonSelected = new Event();
    internal readonly Event continueGame = new Event();
    internal readonly Event settingsOpened = new Event();
    internal readonly Event exitGame = new Event();


    public void Update()
    {
        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
        //     pauseScreenInstance.SetActive(!pauseScreenInstance.activeSelf);
        //     Promote();
        // }
    }

    protected override void OnDialogueAwake()
    {
        base.OnDialogueAwake();
       // Promote();
        
        continueGame.AddListener(() =>
        {
            Resume();
        });
        
        
    }

    protected override void OnClose()
    {
        
    }

    protected override void OnPromote()
    {
        
    }

    protected override void OnDemote()
    {
        
    }    
    
    public void Resume()
    {
        ManagerLocator.Get<AudioManager>().UpdateMusic("CombatState","In_Combat");
        GameDialgoue.Promote();
        Destroy(this.gameObject);

    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        pauseScreenInstance.SetActive(false);
        exitConfirmedPrefabInstance = Instantiate(exitConfirmationPrefab, transform);
        exitConfirmedPrefabInstance.GetComponentsInChildren<Button>()[0].onClick.AddListener(CancelExit);
        exitConfirmedPrefabInstance.GetComponentsInChildren<Button>()[1].onClick.AddListener(ConfirmExit);
    }

    public void CancelExit()
    {
        pauseScreenInstance.SetActive(true);
        Destroy(exitConfirmedPrefabInstance);
    }
    public void ConfirmExit()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Settings()
    {
        //fuck if i know
    }
}
}