using UI.Core;
using UnityEngine;

namespace UI.Game.Timeline
{
    public class TimelineInsightButton : DialogueComponent<GameDialogue>
    {

        private GameDialogue.UnitInfo unitInfo;
        
        
        public GameDialogue.UnitInfo UnitInfo => unitInfo;
        
        
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            Debug.Log("insightClicked");
        }
        
        #endregion
        
        
        #region Drawing
        

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }
        
        #endregion
    }
}
