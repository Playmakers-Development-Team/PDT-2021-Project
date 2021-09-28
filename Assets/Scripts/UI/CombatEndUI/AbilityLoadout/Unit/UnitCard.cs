using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.AbilityLoadout.Unit
{
    public class UnitCard : DialogueComponent<AbilityRewardDialogue>
    {
        protected internal LoadoutUnitInfo loadoutUnitInfo;
        
        [SerializeField] protected RawImage renderImage;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.showAbilitySelectPanel.Invoke(loadoutUnitInfo);
        }
        
        #endregion
        
        #region Drawing
        
        internal void Redraw(LoadoutUnitInfo newLoadoutUnitInfo)
        {
            // Assign unit info
            loadoutUnitInfo = newLoadoutUnitInfo;
            
            if (loadoutUnitInfo == null)
                return;
            
            // Render image
            renderImage.texture = loadoutUnitInfo.ProfileCropInfo.Image;
            renderImage.color = loadoutUnitInfo.ProfileCropInfo.Colour;
            renderImage.uvRect = loadoutUnitInfo.ProfileCropInfo.UVRect;
        }

        #endregion
    }
}
