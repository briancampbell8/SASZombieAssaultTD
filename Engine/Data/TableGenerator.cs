using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;

namespace SASZombieAssaultTD.Engine.Data
{
    public static class TableGenerator
    {
        public static void BuildAll()
        {
            ModernLoggingSystem.LogInfo("TableGenerator.BuildAll() started.");

            BuildHudTable();
            BuildSpriteTable();
            BuildAnimationTable();
            BuildMapTable();

            ModernLoggingSystem.LogInfo("TableGenerator.BuildAll() completed.");
        }

        private static void BuildHudTable()
        {
            // TODO: Implement HUD table generation
        }

        private static void BuildSpriteTable()
        {
            // TODO: Implement sprite placement table generation
        }

        private static void BuildAnimationTable()
        {
            // TODO: Implement animation metadata table generation
        }

        private static void BuildMapTable()
        {
            // TODO: Implement map data table generation
        }
    }
}
