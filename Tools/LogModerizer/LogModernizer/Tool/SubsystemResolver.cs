// ====================================================================================================
//  FILE: SubsystemResolver.cs
//  PATH: Tool\SubsystemResolver.cs
//  PROGRAM: SubsystemResolver.cs
//  MODULE: Diagnostics & Engine Pipeline (SubsystemResolver)
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
    internal sealed class SubsystemResolver
    {
        private static readonly Dictionary<string, string> FolderToSubsystem = new()
        {
            { "\\Economy\\", "LogSubsystems.Economy" },
            { "\\GameLoop\\", "LogSubsystems.GameLoop" },
            { "\\GameRoot\\", "LogSubsystems.GameRoot" },
            { "\\Memory\\", "LogSubsystems.Memory" },
            { "\\Navigation\\", "LogSubsystems.Navigation" },
            { "\\Performance\\", "LogSubsystems.Performance" },
            { "\\Rendering\\", "LogSubsystems.Rendering" },
            { "\\Save\\", "LogSubsystems.Save" },
            { "\\Scenes\\", "LogSubsystems.Scenes" },
            { "\\State\\", "LogSubsystems.State" },
            { "\\Timing\\", "LogSubsystems.Timing" },
            { "\\UI\\", "LogSubsystems.UI" },
            { "\\Window\\", "LogSubsystems.Window" },

            // Diagnostics stays General
            { "\\Diagnostics\\", "LogSubsystems.General" }
        };

        public string Resolve(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return "LogSubsystems.General";

            foreach (var kvp in FolderToSubsystem)
            {
                if (filePath.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }

            return "LogSubsystems.General";
        }
    }
}
