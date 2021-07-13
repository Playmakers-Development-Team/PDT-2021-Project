using UnityEngine;

namespace UI.Test
{
    public class TestDialogue : Dialogue
    {
        internal readonly Event testEvent = new Event();

        protected override void OnHide()
        {
            Debug.Log("Hidden...");
            Destroy(gameObject);
        }

        protected override void OnPromote()
        {
            Debug.Log("Promoted...");
        }

        protected override void OnDemote()
        {
            Debug.Log("Demoted...");
        }
    }
}
