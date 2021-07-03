using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Managers;
using UI;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace Managers
{
    public class UIManager : Manager
    {
        public TileBase movementHighlightTile { get; set; }
        public TileBase abilityHighlightTile { get; set; }
        public Tilemap highlightTilemap { get; set; }
        
        public Tilemap movementHighlightTilemap { get; set; }

        
        public LineRenderer abilityLineRenderer { get; set; }

        public override void ManagerStart() {}

        /// <summary>
        /// Initialising the UIManagers to have access to the highlighted tiles
        /// </summary>
        public void Initialise(TileBase abilityBase, TileBase moveBase,Tilemap abilityMap,Tilemap
         moveMap)
        {
            abilityHighlightTile = abilityBase;
            highlightTilemap = abilityMap;
            movementHighlightTile = moveBase;
            movementHighlightTilemap = moveMap;

        }

        /// <summary>
        /// Clears the current ability highlighted cells
        /// </summary>
        public void ClearAbilityHighlight()
        {
            highlightTilemap.ClearAllTiles();
            
            if (abilityLineRenderer)
                abilityLineRenderer.positionCount = 0;
        }
        

        /// <summary>
        /// Highlights the current selected ability
        /// </summary>
        public void HighlightAbility(Vector2Int originCoordinate, Vector2 targetVector,
                                     Ability ability)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Vector3 originPosition = gridManager.ConvertCoordinateToPosition(originCoordinate);
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.Add(originPosition);
            
            ClearAbilityHighlight();
            var highlightedCoordinates =
                ability.Shape.GetHighlightedCoordinates(originCoordinate, targetVector);

            foreach (Vector2Int highlightedCoordinate in highlightedCoordinates)
            {
                highlightTilemap.SetTile((Vector3Int) highlightedCoordinate, abilityHighlightTile);
                linePositions.Add(gridManager.ConvertCoordinateToPosition(highlightedCoordinate));
            }

            if (ability.Shape.ShouldShowLine && abilityLineRenderer)
            {
                abilityLineRenderer.positionCount = linePositions.Count;
                abilityLineRenderer.SetPositions(linePositions.ToArray());
            }
        }
    }
}
