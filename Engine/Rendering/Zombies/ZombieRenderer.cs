using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Core;
using System.Diagnostics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Rendering.Zombies
{
    /// <summary>
    /// Renders a zombie using a simple texture and animation frame.
    /// </summary>
    public sealed class ZombieRenderer
    {
        private readonly Texture2D _texture;

        public ZombieRenderer(Texture2D texture)
        {
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Execution reached here");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Reached execution checkpoint");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, " +
            $"Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
            _texture = texture;
        }

        public void Render(EnemyBase zombie, AnimationPlayer animation)
        {
            if (zombie is null || animation is null)
                return;

            // Placeholder: real rendering backend would draw the frame here.
            // For now, this is a no-op to keep the engine compiling cleanly.
        }
    }

    public class EnemyBase { }
}



