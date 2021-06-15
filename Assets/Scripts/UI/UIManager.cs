using Managers;
using Unit.Abilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI
{
    public class UIManager : Manager
    {
        public TileBase abilityHighlightTile { get; set; }
        public Tilemap highlightTilemap { get; set; }

        public override void ManagerStart() {}

        /// <summary>
        /// Initialising the UIManagers to have access to the highlighted tiles
        /// </summary>
        public void Initialise(TileBase tileBase, Tilemap tilemap)
        {
            abilityHighlightTile = tileBase;
            highlightTilemap = tilemap;
        }

        /// <summary>
        /// Clears the current ability highlighted cells
        /// </summary>
        public void ClearAbilityHighlight()
        {
            highlightTilemap.ClearAllTiles();
        }

        /// <summary>
        /// Highlights the current selected ability
        /// </summary>
        public void HighlightAbility(Vector2Int originCoordinate, Vector2 targetVector,
                                     Ability ability)
        {
            ClearAbilityHighlight();
            var highlightedCoordinates =
                ability.Shape.GetHighlightedCoordinates(originCoordinate, targetVector);

            foreach (Vector2Int highlightedCoordinate in highlightedCoordinates)
            {
                highlightTilemap.SetTile((Vector3Int) highlightedCoordinate, abilityHighlightTile);
            }
        }
    }
}
