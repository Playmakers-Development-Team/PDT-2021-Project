using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Unit
{
    public class StatDisplay : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI text;
        
        private List<Image> childImages;
        private Color originalTextColor;

        private void Awake()
        {
            childImages = GetComponentsInChildren<Image>().ToList();
            originalTextColor = text.color;
        }

        public void Assign(int value)
        {
            text.text = value.ToString();
        }

        public void SetProjectedTint()
        {
            Color projectedColor = new Color(0.7f, 0.7f, 1, 0.8f);
            
            childImages.ForEach(i => i.color = projectedColor);
            text.color = projectedColor;
        }

        public void ResetTint()
        {
            childImages.ForEach(i => i.color = Color.white);
            text.color = originalTextColor;
        }
    }
}
