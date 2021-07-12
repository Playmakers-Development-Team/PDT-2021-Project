using System;
using UnityEngine;

namespace UI
{
    [Serializable]
    public struct GridSelection
    {
        [SerializeField] private Vector2Int[] spaces;
        [SerializeField] private GridSelectionType type;

        
        public Vector2Int[] Spaces => spaces;
        public GridSelectionType Type => type;


        public GridSelection(Vector2Int[] spaces, GridSelectionType type)
        {
            this.spaces = spaces;
            this.type = type;
        }
    }
}
