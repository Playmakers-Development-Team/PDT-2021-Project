using UI.Core;
using UI.Game;
using UnityEngine;

namespace UI.AbilityLoadout
{
    /// <summary>
    /// NOTE: Attaching this script to the GameDialogue will add the AbilityLoadoutDialogue to the
    /// top of the stack, meaning you can draft abilities before an encounter.
    ///
    /// To disable this, remove this script from the Prefab
    /// </summary>
    
    public class AbilityDraft : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameObject AbilityLoadoutDialogue;
        
        protected override void OnComponentStart()
        {
            Instantiate(AbilityLoadoutDialogue, transform.parent);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
