using UI.Core;

namespace UI
{
    public class TutorialDialogue : Dialogue
    {
        protected override bool ShouldDemoteOtherDialogues => false;

        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
    }
}
