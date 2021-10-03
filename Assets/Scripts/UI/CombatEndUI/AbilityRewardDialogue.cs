using System;
using System.Collections.Generic;
using Abilities;
using TenetStatuses;
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
        
        internal readonly Event<AbilitySelectedCommand> abilityButtonPress = new Event<AbilitySelectedCommand>();
        internal readonly Event<AbilityButton> drawOldAbilityDetails = new Event<AbilityButton>();
        internal readonly Event<AbilityButton> drawNewAbilityDetails = new Event<AbilityButton>();
        internal readonly Event clearOldAbilityDetails = new Event();
        internal readonly Event clearNewAbilityDetails = new Event();
        
        [SerializeField] protected Canvas unitSelectCanvas;
        [SerializeField] protected Canvas abilitySelectCanvas;
        [SerializeField] protected internal UnitSelectCanvasScript unitSelectCanvasScript;
        [SerializeField] protected AbilitySelectCanvasScript abilitySelectCanvasScript;

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
            
            unitSelectCanvasScript.Redraw(units);
        }

        #endregion

        #region Utility Functions

        private void OnUnitFade(LoadoutUnitInfo selectedUnit)
        {
            List<LoadoutUnitInfo> fadeOutUnits = new List<LoadoutUnitInfo>();
            fadeOutUnits.AddRange(units);
            fadeOutUnits.Remove(selectedUnit);
            
            unitSelectCanvasScript.Redraw(units, fadeOutUnits);
        }

        
        protected void OnAbilitySelect(AbilitySelectedCommand cmd)
        {
            abilityButtonPress.Invoke(cmd);
        }

        // Overriden in inherited classes
        protected virtual void OnAbilitySelectPanel (LoadoutUnitInfo loadoutUnitInfo){}

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
