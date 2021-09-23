using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class GameTitleComponent : DialogueComponent<MainMenuDialogue>
    {

        [SerializeField] private Image gameTitle;
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

   
        public void UpdateTitle(Character chosenCharacter)
        {
            switch (chosenCharacter.characterType)
            {
                case CharacterTypes.ESTELLE:
                    gameTitle.color = new Color32(255, 165, 0, 255);
                    break;
                case CharacterTypes.HELENA:
                    gameTitle.color = new Color32(195, 171, 18, 255);
                    break;
                case CharacterTypes.NILES:
                    gameTitle.color = new Color32(140, 0, 255, 255);
                    break;
                case CharacterTypes.STAG:
                    gameTitle.color = new Color32(130, 30,210, 255);
                    break;
                case CharacterTypes.EYE:
                    gameTitle.color = new Color32(110, 170, 250, 255);
                    break;
            }
            
        }
    }
}
