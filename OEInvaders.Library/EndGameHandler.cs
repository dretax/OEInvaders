namespace OEInvaders.Library
{
    /// <summary>
    /// Handles the game ending event.
    /// </summary>
    public static class EndGameHandler
    {
        /// <summary>
        /// Delegate of the game over event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="points">Points.</param>
        public delegate void GameOverDelegate(string name, int points);

        /// <summary>
        /// Runs when the game is over.
        /// </summary>
        public static event GameOverDelegate OnGameOver;

        /// <summary>
        /// Handles the game over event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="points">Points.</param>
        public static void RunGameOver(string name, int points)
        {
            if (OnGameOver != null)
            {
                OnGameOver(name, points);
            }
        }
    }
}