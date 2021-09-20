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

        private GameDialogue.UnitInfo unitInfo;

        public GameDialogue.UnitInfo UnitInfo => unitInfo;


        #region UIComponent

        protected override void Subscribe()
        {
            dialogue.unitSelected.AddListener(SelectUnit);
            dialogue.unitDeselected.AddListener(DeselectUnit);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSelected.RemoveListener(SelectUnit);
            dialogue.unitDeselected.RemoveListener(DeselectUnit);
        }

        #endregion


        #region Listeners

        public void OnClick()
        {
            dialogue.unitSelected.Invoke(unitInfo);
        }

        private void SelectUnit(GameDialogue.UnitInfo info)
        {
            btn.interactable = !(info == unitInfo);
        }

        private void DeselectUnit()
        {
            btn.interactable = true;
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
