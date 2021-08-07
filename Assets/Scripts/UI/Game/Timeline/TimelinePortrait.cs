using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    public class TimelinePortrait : DialogueComponent<GameDialogue>
    {
        [SerializeField] private RawImage image;

        private GameDialogue.UnitInfo unitInfo;
        
        
        public GameDialogue.UnitInfo UnitInfo => unitInfo;
        
        
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            dialogue.unitSelected.Invoke(unitInfo);
        }
        
        #endregion
        
        
        #region Drawing
        
        internal void Assign(GameDialogue.UnitInfo unit)
        {
            unitInfo = unit;
            
            image.texture = unitInfo.TimelineCropInfo.Image;
            image.color = unitInfo.TimelineCropInfo.Colour;
            image.uvRect = unitInfo.TimelineCropInfo.UVRect;
        }

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }
        
        #endregion
    }
}
