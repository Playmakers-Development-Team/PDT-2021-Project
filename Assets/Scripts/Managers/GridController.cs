using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        private Tilemap tilemap;
        private GridManager gridManager;

        private void Awake()
        {
            tilemap = GetComponentInChildren<Tilemap>();
            
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.Controller = this;
            gridManager.Initialise(tilemap);
        }
    }
}
