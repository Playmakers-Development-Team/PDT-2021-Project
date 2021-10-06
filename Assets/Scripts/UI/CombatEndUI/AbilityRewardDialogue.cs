using System;
using System.Collections.Generic;
using Abilities;
using TenetStatuses;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.CombatEndUI.AbilityLoadout.Unit;
using UI.CombatEndUI.PanelScripts;
using UI.Commands;
using UI.Core;
using Units.Players;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.CombatEndUI
{
    public abstract class AbilityRewardDialogue : Dialogue
    {
        internal readonly Event<LoadoutUnitInfo> showAbilitySelectPanel = new Event<LoadoutUnitInfo>();
        internal readonly Event<LoadoutUnitInfo> unitSpawned = new Event<LoadoutUnitInfo>();
        internal readonly Event showUnitSelectPanel = new Event();
        internal readonly Event<LoadoutUnitInfo> fadeOtherUnits = new Event<LoadoutUnitInfo>();
        internal readonly Event slideActiveUnit = new Event();
        
        internal readonly Event<AbilitySelectedCommand> abilityButtonPress = new Event<AbilitySelectedCommand>();
        internal readonly Event<AbilityButton> drawOldAbilityDetails = new Event<AbilityButton>();
        internal readonly Event<AbilityButton> drawNewAbilityDetails = new Event<AbilityButton>();
        internal readonly Event clearOldAbilityDetails = new Event();
        internal readonly Event clearNewAbilityDetails = new Event();
        
        [SerializeField] protected Canvas unitSelectCanvas;
        [SerializeField] protected Canvas abilitySelectCanvas;
        [SerializeField] protected Canvas finalAbilitiesCanvas;
        [SerializeField] protected internal UnitSelectCanvasScript unitSelectCanvasScript;
        [SerializeField] protected AbilitySelectCanvasScript abilitySelectCanvasScript;

        [SerializeField] protected internal float fadeOutTime = 0.5f;
        [SerializeField] protected internal float selectedUnitSlideSpeed = 1000f;
        [SerializeField] protected internal float selectedUnitPosition = -180f;
        
        protected internal UnitCard activeUnitCard;
        protected internal UnitAbilitiesCard activeAbilitiesCard;
        protected readonly List<LoadoutUnitInfo> units = new List<LoadoutUnitInfo>();
        public List<Sprite> abilityImages = new List<Sprite>();

        #region Monobehaviour Events

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();

            showUnitSelectPanel.AddListener(() =>
            {
                OnUnitSelectPanel();
            });

            showAbilitySelectPanel.AddListener(unitInfo =>
            {
                OnAbilitySelectPanel(unitInfo);
            });
            
            fadeOtherUnits.AddListener(unitInfo =>
            {
                OnUnitFade(unitInfo);
            });
            
            slideActiveUnit.AddListener(() =>
            {
                OnUnitSlide();
            });
            
            unitSpawned.AddListener(info =>
            {
                if (info.Unit is PlayerUnit)
                    units.Add(info);
            });
        }
        
        #endregion

        #region Dialogue Functions

        protected override void OnClose() {}

        protected override void OnPromote()
        {
            canvasGroup.interactable = true;
        }

        protected override void OnDemote()
        {
            canvasGroup.interactable = false;
        }

        #endregion
        
        #region Panel Switching

        private void OnUnitSelectPanel()
        {
            unitSelectCanvas.enabled = true;
            abilitySelectCanvas.enabled = false;
            finalAbilitiesCanvas.enabled = false;
            
            unitSelectCanvasScript.Redraw(units);
        }

        #endregion

        #region Utility Functions

        private void OnUnitFade(LoadoutUnitInfo selectedUnit)
        {
            unitSelectCanvasScript.SetActiveUnit(selectedUnit);
            
            unitSelectCanvasScript.FadeOutUnits(fadeOutTime);
            unitSelectCanvasScript.FadeOutText();
        }

        private void OnUnitSlide()
        {
            unitSelectCanvasScript.SlideActiveUnit();
        }
        
        protected void OnAbilitySelect(AbilitySelectedCommand cmd)
        {
            abilityButtonPress.Invoke(cmd);
        }

        // Overriden in inherited classes
        protected virtual void OnAbilitySelectPanel(LoadoutUnitInfo loadoutUnitInfo)
        {
            abilitySelectCanvas.enabled = true;
            finalAbilitiesCanvas.enabled = true;
            units.Clear();
        }

        protected internal void SetActiveUnitCard(UnitCard unitCard)
        {
            activeUnitCard = unitCard;
        }
        
        // Search through the selected unit's abilities to find the NonUpgraded ability
        // Only useful for ability upgrading (obviously)
        protected internal AbilityButton GetNonUpgradedAbility(string abilityName)
        {
            List<AbilityButton> currentAbilityButtons = activeAbilitiesCard.abilityButtons;
            
            foreach (AbilityButton nonUpgradedAbility in currentAbilityButtons)
            {
                if (abilityName.Equals(nonUpgradedAbility.AbilityName + "+"))
                {
                    return nonUpgradedAbility;
                }
            }
            
            Debug.LogError("Could not find NonUpgraded ability for " + abilityName);

            return null;
        }

        #endregion

        #region Querying
        
        internal LoadoutAbilityInfo GetInfo(Ability ability)
        {
            LoadoutAbilityInfo loadoutAbilityInfo = new LoadoutAbilityInfo();
            loadoutAbilityInfo.Ability = ability;
            
            switch (ability.RepresentedTenet)
            {
                case TenetType.Apathy:
                    loadoutAbilityInfo.Render = abilityImages[0];
                    break;
                case TenetType.Humility:
                    loadoutAbilityInfo.Render = abilityImages[1];
                    break;
                case TenetType.Joy:
                    loadoutAbilityInfo.Render = abilityImages[2];
                    break;
                case TenetType.Passion:
                    loadoutAbilityInfo.Render = abilityImages[3];
                    break;
                case TenetType.Pride:
                    loadoutAbilityInfo.Render = abilityImages[4];
                    break;
                case TenetType.Sorrow:
                    loadoutAbilityInfo.Render = abilityImages[5];
                    break;
                default:
                    throw new Exception($"Could not get {nameof(LoadoutAbilityInfo)} for {ability}.");
            }
            
            return loadoutAbilityInfo;
        }

        #endregion
    }
}
