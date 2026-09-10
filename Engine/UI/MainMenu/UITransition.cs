// ====================================================================================================
//  FILE: UITransition.cs
//  PATH: ./Engine/UI/MainMenu/
//  MODULE: UI.MainMenu
//
//  ROLE:
//      Deterministic transition object used by Main Menu transitions. Handles fade‑in, fade‑out,
//      and other time‑based visual state changes for UIElement instances.
//
//  RESPONSIBILITIES:
//      - Store transition timing and interpolation metadata.
//      - Update UIElement opacity deterministically over time.
//      - Provide stable, reproducible animation curves for the Option‑B UI pipeline.
//
//  NON‑RESPONSIBILITIES:
//      - Scene switching logic.
//      - Input dispatch.
//      - Asset loading.
//
// ====================================================================================================

using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public class UITransition
    {
        public UIElement Target { get; set; }
        public float Duration { get; set; }
        public float StartOpacity { get; set; }
        public float EndOpacity { get; set; }

        private float _time;
        internal System.Action<object> Apply;
        internal float Elapsed;

        public void Update(float deltaTime)
        {
            _time += deltaTime;

            float t = System.Math.Clamp(_time / Duration, 0f, 1f);

            Target.Opacity = StartOpacity + (EndOpacity - StartOpacity) * t;
        }

        public bool IsComplete => _time >= Duration;
    }
}
