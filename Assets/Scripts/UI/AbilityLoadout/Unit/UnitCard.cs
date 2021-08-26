using System.Collections.Generic;
using Abilities;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Unit
{
    public class UnitCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        protected internal AbilityLoadoutDialogue.UnitInfo unitInfo;
        
        [SerializeField] protected RawImage renderImage;
        [SerializeField] protected List<Image> abilityRender = new List<Image>();

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.panelSwap.Invoke(AbilityLoadoutPanelType.AbilitySelect);
        }
        
        #endregion
        
        #region Drawing
        
        internal void Redraw(AbilityLoadoutDialogue.UnitInfo newUnitInfo)
        {
            // Assign unit info
            unitInfo = newUnitInfo;
            
            if (unitInfo == null)
                return;
            
            renderImage.texture = unitInfo.ProfileCropInfo.Image;
            
            // Assign ability images
            for (int i = 0; i < unitInfo.AbilityInfo.Count; ++i)
            {
                abilityRender[i].sprite = unitInfo.AbilityInfo[i].Render;
            }
        }

        #endregion
    }
}
