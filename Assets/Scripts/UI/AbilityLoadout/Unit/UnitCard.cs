using System.Collections.Generic;
using Abilities;
using UI.AbilityLoadout.Panel_Scripts;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Unit
{
    public class UnitCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        protected internal AbilityLoadoutDialogue.UnitInfo unitInfo;
        
        [SerializeField] protected RawImage renderImage;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.showAbilitySelectPanel.Invoke(unitInfo);
        }
        
        #endregion
        
        #region Drawing
        
        internal void Redraw(AbilityLoadoutDialogue.UnitInfo newUnitInfo)
        {
            // Assign unit info
            unitInfo = newUnitInfo;
            
            if (unitInfo == null)
                return;
            
            // Render image
            renderImage.texture = unitInfo.ProfileCropInfo.Image;
            renderImage.color = unitInfo.ProfileCropInfo.Colour;
            renderImage.uvRect = unitInfo.ProfileCropInfo.UVRect;
        }

        #endregion
    }
}
