using Managers;
using TMPro;
using Turn;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TempUI
{
    // TODO: Delete once proper turn manipulation UI has been added
    public class TurnManipulateTargetUI : MonoBehaviour
    {
        [SerializeField] private Button button;

        private TurnManager turnManager;
        
        private IUnit unit;
        private Transform parent;
        private bool spawningBeforeCurrentUnit;

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
     
        }

        public void InitialiseButton(IUnit unit, Transform parent, bool spawningBeforeCurrentUnit)
        {
            this.unit = unit;
            this.parent = parent;
            this.spawningBeforeCurrentUnit = spawningBeforeCurrentUnit;
            button.GetComponentInChildren<TextMeshProUGUI>().text = unit.Name;
        }


        public void ManipulateUnit()
        {
            
            if (spawningBeforeCurrentUnit)
                turnManager.MoveTargetBeforeCurrent(turnManager.FindTurnIndexFromCurrentQueue
                (unit));
            else
            {
                turnManager.MoveTargetAfterCurrent(turnManager.FindTurnIndexFromCurrentQueue
                    (unit));
            }
                
        }
        
        
        
    }
}
