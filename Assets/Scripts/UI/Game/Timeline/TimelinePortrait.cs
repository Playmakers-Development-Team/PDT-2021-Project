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

        protected override void Subscribe()
        {
            dialogue.unitSelected.AddListener(SelectUnit);
            dialogue.unitDeselected.AddListener(DeselectUnit);
            dialogue.turnManipulationStarted.AddListener(TurnManipulationStart);
            dialogue.turnManipulationEnded.AddListener(TurnManipulationEnd);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSelected.RemoveListener(SelectUnit);
            dialogue.unitDeselected.RemoveListener(DeselectUnit);
            dialogue.turnManipulationStarted.RemoveListener(TurnManipulationStart);
            dialogue.turnManipulationEnded.RemoveListener(TurnManipulationEnd);
        }
        

        #endregion


        #region Listeners

        public void OnClick()
        {
            if (turnManager.turnManipulating)
            {
                TurnManipulate();
                TurnManipulationEnd();
            }else
                dialogue.unitSelected.Invoke(unitInfo);
        }

        private void SelectUnit(GameDialogue.UnitInfo info)
        {
            btn.interactable = !(info == unitInfo);
            selectedUnit = info;
        }

        private void DeselectUnit()
        {
            btn.interactable = true;
            selectedUnit = null;
        }

        private void TurnManipulate()
        {
            int selectedIndex = turnManager.FindTurnIndexFromCurrentQueue(selectedUnit.Unit);
            int thisIndex = turnManager.FindTurnIndexFromCurrentQueue(unitInfo.Unit);

            if(selectedIndex == 0)
                turnManager.MoveTargetBeforeCurrent(selectedIndex);
            else
                turnManager.ShiftTurnQueue(thisIndex,selectedIndex);
            
        }

        private void TurnManipulationStart(GameDialogue.UnitInfo info)
        {
            btn.interactable = !(info == unitInfo);
            if(info != unitInfo)
                prepareForManipulation();
        }
        
        private void TurnManipulationEnd()
        {
           exitManipulation();
        }
        private void prepareForManipulation()
        {
            turnManager.turnManipulating = true;
        }

        private void exitManipulation()
        {
            turnManager.turnManipulating = false;
        }

        #endregion

        #region Utility

        public bool isSelected()
        {
            return unitInfo == selectedUnit;
        }

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
