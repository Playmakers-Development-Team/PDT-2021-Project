using System;
using UnityEngine;

namespace UI.Game.Grid
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
        
        public GridSelection(Vector2Int space, GridSelectionType type)
        {
            spaces = new Vector2Int[1];
            spaces[0] = space;
            this.type = type;
        }
    }
}
