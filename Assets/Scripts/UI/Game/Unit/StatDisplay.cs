using TMPro;
using UnityEngine;

namespace UI.Game.Unit
{
    public class StatDisplay : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI text;
        
        public void Assign(int value)
        {
            text.text = value.ToString();
        }
    }
}
