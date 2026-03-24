namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Defines the contract for game systems in the engine.
    /// </summary>
    public interface IGameSystem
    {
        /// <summary>
        /// Initializes the game system.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Updates the game system each frame.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// Shuts down the game system.
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// Gets or sets whether the system is active.
        /// </summary>
        bool IsActive { get; set; }
    }
}




