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
        [SerializeField] private List<Ability> abilities = new List<Ability>();

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
            unitInfo = newUnitInfo;
            
            if (unitInfo == null)
                return;
            
            renderImage.texture = unitInfo.ProfileCropInfo.Image;
            abilities = unitInfo.Unit.Abilities;
        }

        #endregion
    }
}
