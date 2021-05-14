using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    [CreateAssetMenu(menuName = "Settings/Color Drop Setting")]
    public class ColorDropSettings : ScriptableObject
    {
        public ColorSelection[] colorSelections;
        public SDFSelection[] sdfSelections;
    }
}
