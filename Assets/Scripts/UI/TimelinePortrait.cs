using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimelinePortrait : Element
    {
        [SerializeField] private Image image;
        
        
        internal void Assign(Sprite portrait)
        {
            image.sprite = portrait;
        }

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }
    }
}
