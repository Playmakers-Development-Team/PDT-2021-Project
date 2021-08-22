using Commands;

namespace Grid.Commands
{
    /// <summary>
    /// Executed when the grid has been populated with tile data from the scene's tilemap.
    /// </summary>
    public class GridReadyCommand : Command {}

    /// <summary>
    /// Executed when the grid has been populated with grid objects.
    /// </summary>
    public class GridObjectsReadyCommand : Command {}
}
