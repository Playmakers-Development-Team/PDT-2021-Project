using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UI.Core;
using Units;
using Units.Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    [RequireComponent(typeof(Button))]
    public class TimelinePortrait : DialogueComponent<GameDialogue>
    {
        [SerializeField] private RawImage image;
        [SerializeField] private Sprite enemyBackground;
        [SerializeField] private Image selectableFrame;
        [SerializeField] private Button btn;
        private TurnManager turnManager;

        private GameDialogue.UnitInfo unitInfo;
        private GameDialogue.UnitInfo selectedUnit;

        public GameDialogue.UnitInfo UnitInfo => unitInfo;


        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void OnComponentStart()
        {
            // For some reason, sometimes we get an uninitialised portrait?
            if (selectableFrame != null)
                selectableFrame.gameObject.SetActive(false);
        }

        protected override void Subscribe()
        {
            dialogue.unitSelected.AddListener(SelectUnit);
            dialogue.unitDeselected.AddListener(DeselectUnit);
            dialogue.turnManipulationStarted.AddListener(TurnManipulationStarted);
            dialogue.turnManipulationChosen.AddListener(TurnManipulationChosen);
            dialogue.turnManipulationEnded.AddListener(TurnManipulationEnd);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSelected.RemoveListener(SelectUnit);
            dialogue.unitDeselected.RemoveListener(DeselectUnit);
            dialogue.turnManipulationStarted.AddListener(TurnManipulationStarted);
            dialogue.turnManipulationChosen.RemoveListener(TurnManipulationChosen);
            dialogue.turnManipulationEnded.RemoveListener(TurnManipulationEnd);
        }
        

        #endregion


        #region Listeners

        public void OnClick()
        {
            if (turnManager.turnManipulating && selectedUnit != null)
            {
                TurnManipulate();
                TurnManipulationEnd();
            }
            else
            {
                dialogue.unitSelected.Invoke(unitInfo);
            }
        }

        private void SelectUnit(GameDialogue.UnitInfo info)
        {
            // btn.interactable = !(info == unitInfo);
            // selectedUnit = info;

            if (turnManager.turnManipulating && (selectedUnit == null || selectedUnit.Unit != info.Unit))
            {
                dialogue.turnManipulationChosen.Invoke(info);
                selectedUnit = info;
                
                // For some reason, sometimes we get an uninitialised portrait?
                if (selectableFrame)
                    selectableFrame.gameObject.SetActive(false);
            }
        }

        private void DeselectUnit()
        {
            // btn.interactable = true;
            selectedUnit = null;
        }

        private void TurnManipulate()
        {
            int selectedIndex = turnManager.FindTurnIndexFromCurrentQueue(selectedUnit.Unit);
            int thisIndex = turnManager.FindTurnIndexFromCurrentQueue(unitInfo.Unit);

            if (thisIndex == 0)
                turnManager.MoveTargetBeforeCurrent(selectedIndex);
            else
                turnManager.MoveTargetAfterCurrent(selectedIndex);
        }

        private void TurnManipulationStarted()
        {
            btn.interactable = unitInfo != null && unitInfo.Unit != turnManager.ActingUnit;
            
            // For some reason, sometimes we get an uninitialised portrait?
            if (selectableFrame)
                selectableFrame.gameObject.SetActive(btn.interactable);
        }

        private void TurnManipulationChosen(GameDialogue.UnitInfo info)
        {
            // btn.interactable = !(info == unitInfo);
            // if(info != unitInfo)
            //     PrepareForManipulation();

            // For some reason, unit info can be null
            if (unitInfo == null) return;

            int actingUnitIndex = turnManager.FindTurnIndexFromCurrentQueue(turnManager.ActingUnit);
            int unitIndex = turnManager.FindTurnIndexFromCurrentQueue(unitInfo.Unit);

            btn.interactable = Mathf.Abs(unitIndex) - Mathf.Abs(actingUnitIndex) <= 1;
        }
        
        private void TurnManipulationEnd()
        {
            turnManager.turnManipulating = false;
            btn.interactable = true;
            selectedUnit = null;
            
            // For some reason, sometimes we get an uninitialised portrait?
            if (selectableFrame)
                selectableFrame.gameObject.SetActive(false);
        }

        #endregion

        #region Utility

        public bool IsSelected() => unitInfo == selectedUnit;

        #endregion

        #region Drawing

        internal void Assign(GameDialogue.UnitInfo unit)
        {
            unitInfo = unit;
            if (unit.Unit.GetType().Equals(typeof(EnemyUnit)))
            {
                GetComponent<Image>().sprite = enemyBackground;
            }


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
