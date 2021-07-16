using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    public class TimelinePortrait : UIComponent<GameDialogue>
    {
        [SerializeField] private Image image;

        private GameDialogue.UnitInfo unitInfo;
        
        
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
        
        internal void Assign(IUnit unit)
        {
            unitInfo = dialogue.GetInfo(unit);
            
            image.sprite = unitInfo.Render;
            image.color = unitInfo.Color;
        }

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }
        
        #endregion
    }
}
