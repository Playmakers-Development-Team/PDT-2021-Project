using System.Collections;
using System.Collections.Generic;
using UI.Core;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoseDialogue : Dialogue
{
    // Start is called before the first frame update

    // Update is called once per frame


    protected override void OnClose()
    {
    }

    protected override void OnPromote() 
    {
    }

    protected override void OnDemote()
    {
    }

    public void MainMenu() 
    {
        SceneManager.LoadScene(0); 
    }
    
}
