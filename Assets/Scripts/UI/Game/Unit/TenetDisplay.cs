using TenetStatuses;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Unit
{
    public class TenetDisplay : StatDisplay
    {
        [SerializeField] private Image image;
        
        public void SetTenet(TenetType type)
        {
            image.sprite = Settings.GetTenetIcon(type);
            text.fontMaterial.color = Settings.GetTenetColour(type);
        }
    }
}
