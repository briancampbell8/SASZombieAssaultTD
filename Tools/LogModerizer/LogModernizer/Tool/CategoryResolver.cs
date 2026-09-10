// ====================================================================================================
//  FILE: CategoryResolver.cs
//  PATH: Tool\CategoryResolver.cs
//  PROGRAM: CategoryResolver.cs
//  MODULE: Diagnostics & Engine Pipeline (CategoryResolver)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for LogModernizer.Tool.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide public interface and handling execution for Resolve().
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

namespace LogModernizer.Tool
{
    internal sealed class CategoryResolver
    {
        public string Resolve(LegacyCallMatch match, string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string message = match.Message;

            if (message.Contains("error", System.StringComparison.OrdinalIgnoreCase))
                return "LogCategory.Error";

            if (message.Contains("warn", System.StringComparison.OrdinalIgnoreCase))
                return "LogCategory.Warning";

            if (fileName.Contains("Pathfinding"))
                return "LogCategory.Pathfinding";

            if (fileName.Contains("Spawner"))
                return "LogCategory.Spawner";

            return "LogCategory.General";
        }
    }
}
