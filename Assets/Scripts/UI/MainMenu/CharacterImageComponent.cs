using System.Collections.Generic;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.MainMenu
{

    public enum CharacterTypes
    {
        NILES,
        HELENA,
        ESTELLE,
        EYE,
        STAG
    }
    
    public class CharacterImageComponent : DialogueComponent<MainMenuDialogue>
    {

        [SerializeField] private Image characterImage;

        [SerializeField] private List<Character> characters = new List<Character>();

        internal int characterIndex;
        
        protected override void Subscribe() {}
        
        protected override void Unsubscribe() {}
        

        public Character GetCharacter()
        {
            return characters[characterIndex];
        }

        public void RandomizeCharacterSprite()
        {
            int random = Random.Range(0, characters.Count - 1);
            characterImage.sprite = characters[random].characterSprite;
            characterIndex = random;
        }
        
        
        
        
    }
    
    
}
