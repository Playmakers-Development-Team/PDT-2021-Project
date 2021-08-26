using System.Collections.Generic;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Abilities
{
    public class UnitAbilitiesCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        protected internal List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos;
        
        [SerializeField] protected List<Image> abilityRenders = new List<Image>();

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion

        #region Drawing
        
        internal void Redraw(List<AbilityLoadoutDialogue.AbilityInfo> newAbilityInfo)
        {
            // Assign unit info
            abilityInfos = newAbilityInfo;
            
            if (abilityInfos == null)
                return;

            // Assign ability images
            for (int i = 0; i < abilityInfos.Count; ++i)
            {
                abilityRenders[i].sprite = abilityInfos[i].Render;
            }
        }

        #endregion
    }
}
