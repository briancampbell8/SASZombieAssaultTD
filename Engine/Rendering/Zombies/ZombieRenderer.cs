using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Core;
using System.Diagnostics;
using System.Reflection;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering.Zombies
//
{
    ///<summary>
    ///Renders a zombie using a simple texture and animation frame.
    ///</summary>
    public sealed class ZombieRenderer
    {
        private readonly Texture2D _texture;

        public ZombieRenderer(Texture2D texture)
        {
            DLogger.Log("BREAKPOINT", "Execution reached here");
            DLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, " +
            $"Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
            _texture = texture;
        }

        public void Render(EnemyBase zombie, AnimationPlayer animation)
        {
            if (zombie is null || animation is null)
                return;

            //Placeholder: real rendering backend would draw the frame here.
            //For now, this is a no-op to keep the engine compiling cleanly.
        }
    }

    public class EnemyBase { }
}



