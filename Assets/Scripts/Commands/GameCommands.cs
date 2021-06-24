namespace Commands
{
    public class GameCommands
    {
        /// <summary>
        /// Executed when the player has won the game (a.k.a combat encounter).
        /// </summary>
        public class WinCommand : Command {}

        /// <summary>
        /// Executed when the player has lost the game (a.k.a combat encounter).
        /// </summary>
        public class LoseCommand : Command {}
    }
}
