// ====================================================================================================
//  FILE: VisualEffect.cs
//  PATH: ./Engine/Animation/Visualization/Effects/VisualEffect.cs
//  MODULE: Animation Visualization
//
//  ROLE:
//      Encapsulate core engine behavior for the VisualEffect base module.
//
//  RESPONSIBILITIES:
//      - Provide base effect lifecycle behavior for the Visualization subsystem.
//      - Provide IsExpired() behavior for the Visualization subsystem.
//      - Provide Update() behavior contract for the Visualization subsystem.
//      - Provide Render() behavior contract for the Visualization subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    VisualEffect.cs
Path:    Engine/Animation/Visualization/Effects/VisualEffect.cs
Purpose: P11-16-05 - Base class for animation visualization effects.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.VectorMath;

//FORCE System.Drawing.Color
using DrawingColor = System.Drawing.Color;

namespace SASZombieAssaultTD.Engine.Animation.Visualization.Effects
{
    public abstract class VisualEffect
    {
        public float Duration { get; set; }
        public float Intensity { get; set; }

        //FORCE System.Drawing.Color
        public DrawingColor Color { get; set; }

        public bool IsExpired => Duration > 0f && CalculateElapsedTime() >= Duration;

        private float CalculateElapsedTime()
        {
            var startTime = DateTime.Now.AddSeconds(-Duration);
            var elapsed = DateTime.Now - startTime;
            return (float)elapsed.TotalSeconds;
        }

        public abstract void Update(float animationTime);
        public abstract void Render(IDebugRenderer debugRenderer, Vector3 position, float animationTime);
    }
}
