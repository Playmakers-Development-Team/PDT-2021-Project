using System;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.AbilityLoadout.Unit
{
    public class UnitCard : DialogueComponent<AbilityRewardDialogue>
    {
        protected internal LoadoutUnitInfo loadoutUnitInfo;
        
        [SerializeField] protected RawImage renderImage;
        
        private RectTransform rectTransform;
        internal bool isSliding = false;

        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Monobehavior Events
        
        private void Update()
        {
            if (isSliding)
            {
                // Move our position a step closer to the target.
                float step =  dialogue.selectedUnitSlideSpeed * Time.deltaTime; // calculate distance to move
                rectTransform.anchoredPosition -= new Vector2(step, 0);

                // Check if the position of the unit card and final position are approximately equal.
                if (rectTransform.anchoredPosition.x - dialogue.selectedUnitPosition < 0.001f)
                {
                    // Set the final position
                    rectTransform.anchoredPosition = new Vector2(dialogue.selectedUnitPosition, rectTransform.anchoredPosition.y);
                    
                    isSliding = false;
                    ShowAbilitySelectPanel();
                }
            }
        }

        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.fadeOtherUnits.Invoke(loadoutUnitInfo);
            Invoke(nameof(SlideIntoPosition), dialogue.fadeOutTime);
        }
        
        #endregion
        
        #region Utility Functions

        private void SlideIntoPosition()
        {
            dialogue.slideActiveUnit.Invoke();
        }
        
        private void ShowAbilitySelectPanel()
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

        internal void FadeOut(float fadeOutTime)
        {
            renderImage.CrossFadeAlpha(0, fadeOutTime, true);
        }

        #endregion
    }
}
