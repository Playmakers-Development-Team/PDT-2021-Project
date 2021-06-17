using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI.Refactored
{
    public class Grid : Element
    {
        private GridManager gridManager;
        
        protected override void OnAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            manager.gridSpacesSelected.AddListener(OnGridSelected);
            manager.gridSpacesDeselected.AddListener(OnGridDeselected);
        }

        protected override void Refresh()
        {
        }

        private void OnGridSelected(IEnumerable<Vector2Int> selected)
        {
            
        }

        private void OnGridDeselected()
        {
            
        }
    }
}
