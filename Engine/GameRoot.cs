using SASZombieAssaultTD.Engine.Systems.Enemies;

namespace GameEngine
{
    public static class GameRoot
    {
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized)
                return;

            // Initialize core engine systems here
            // Example:
            // InputSystem.Initialize();
            // AssetSystem.Initialize();
            // SceneSystem.Initialize();

            // Integrate EnemyDefinitionRegistry
            var allEnemyDefs = EnemyDefinitionRegistry.All;
            // Optionally: iterate or log loaded enemy definitions
            // foreach (var def in allEnemyDefs)
            //     DebugLogger.Log("Info", $"Loaded enemy: {def.Id}");

            _initialized = true;
        }

        public static void Update(float deltaTime)
        {
            if (!_initialized)
                Initialize();

            // Update core systems here
            // Example:
            // InputSystem.Update(deltaTime);
            // SceneSystem.Update(deltaTime);
        }

        public static void Shutdown()
        {
            if (!_initialized)
                return;

            // Shutdown / dispose systems here
            // Example:
            // SceneSystem.Shutdown();
            // AssetSystem.Shutdown();

            _initialized = false;
        }
    }
}