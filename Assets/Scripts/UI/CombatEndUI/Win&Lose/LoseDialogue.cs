using UI.Core;
using UnityEngine.SceneManagement;

namespace UI.CombatEndUI
{
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

        public void MainMenu() =>SceneManager.LoadScene(0); //TODO: Change to appropriate scene instead of zero
    
    
    }
}
