namespace SASZombieAssaultTD.Engine.Gameplay
{
    /// <summary>
    /// Interface for game systems that can be initialized, updated, and managed.
    /// </summary>
    public interface IGameSystem
    {
        /// <summary>
        /// Initialize the game system.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Update the game system.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        void Update(float deltaTime);
    }
}
