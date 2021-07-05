using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimelinePortrait : Element
    {
        [SerializeField] private Image image;

        private IUnit unit;
        
        
        internal void Assign(IUnit newUnit)
        {
            image.sprite = newUnit.Render;
            unit = newUnit;
        }

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }

        public void OnClick()
        {
            manager.unitSelected.Invoke(unit);
        }
    }
}
