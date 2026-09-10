// ====================================================================================================
//  FILE: ChampionVisuals.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ChampionVisuals module.
//
//  RESPONSIBILITIES:
//      - Provide GenerateForLevel() behavior for the Core subsystem.
//      - Provide ApplyTo() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide GetCurrentAuraIntensity() behavior for the Core subsystem.
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
Program Name: SASZombieAssaultTD
File Path: Engine\Enemies\ChampionVisuals.cs
Purpose: P100 Wave and Enemy Modernization - Champion visual effects system.
Features: Champion aura effects, color tinting, particle effects, scale modifications.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Champion visual effects for enemy champions.
    ///P100-05: Champion visuals system implementation
    ///</summary>
    public class ChampionVisuals
    {
        ///<summary>
        ///Aura color for the champion.
        ///</summary>
        public Vector4 AuraColor { get; set; }

        ///<summary>
        ///Aura intensity (0.0 to 1.0).
        ///</summary>
        public float AuraIntensity { get; set; }

        ///<summary>
        ///Scale multiplier for the champion.
        ///</summary>
        public float ScaleMultiplier { get; set; }

        ///<summary>
        ///Glow intensity (0.0 to 1.0).
        ///</summary>
        public float GlowIntensity { get; set; }

        ///<summary>
        ///Particle effects for the champion.
        ///</summary>
        public List<ParticleEffect> Particles { get; set; }

        ///<summary>
        ///Pulse animation speed.
        ///</summary>
        public float PulseSpeed { get; set; }

        ///<summary>
        ///Whether the champion has a halo effect.
        ///</summary>
        public bool HasHalo { get; set; }

        private float _pulseTime = 0f;

        public ChampionVisuals()
        {
            AuraColor = new Vector4(1f, 0.8f, 0f, 1f); //Gold default
            AuraIntensity = 0.5f;
            ScaleMultiplier = 1.2f;
            GlowIntensity = 0.3f;
            Particles = new List<ParticleEffect>();
            PulseSpeed = 2f;
            HasHalo = true;
        }

        ///<summary>
        ///Generate champion visuals for a specific champion level.
        ///</summary>
        ///<param name="championLevel">Champion level (1-10).</param>
        ///<returns>Champion visuals configuration.</returns>
        public static ChampionVisuals GenerateForLevel(int championLevel)
        {
            var visuals = new ChampionVisuals();

            //Scale increases with level
            visuals.ScaleMultiplier = 1.0f + (championLevel * 0.05f);

            //Aura color changes based on level tier
            var tier = (championLevel - 1) / 3; //0-2 tiers
            visuals.AuraColor = tier switch
            {
                0 => new Vector4(1f, 0.8f, 0f, 1f), //Gold
                1 => new Vector4(0.5f, 0.8f, 1f, 1f), //Blue
                2 => new Vector4(1f, 0.3f, 0.5f, 1f), //Red
                _ => new Vector4(1f, 1f, 1f, 1f) //White
            };

            //Intensity increases with level
            visuals.AuraIntensity = 0.3f + (championLevel * 0.05f);
            visuals.GlowIntensity = 0.2f + (championLevel * 0.03f);

            //Higher levels have halo
            visuals.HasHalo = championLevel >= 5;

            //Pulse speed increases with level
            visuals.PulseSpeed = 1.5f + (championLevel * 0.2f);

            return visuals;
        }

        ///<summary>
        ///Apply champion visuals to an enemy.
        ///</summary>
        ///<param name="enemy">Enemy to apply visuals to.</param>
        public void ApplyTo(Enemy enemy)
        {
            if (enemy == null) return;

            //Apply scale
            enemy.SetScale(ScaleMultiplier);

            //Apply tint color
            enemy.SetTintColor(AuraColor.X, AuraColor.Y, AuraColor.Z, AuraColor.W);

            //Store visual data in custom properties
            enemy.SetCustomProperty("ChampionAuraColor", AuraColor);
            enemy.SetCustomProperty("ChampionAuraIntensity", AuraIntensity);
            enemy.SetCustomProperty("ChampionGlowIntensity", GlowIntensity);
            enemy.SetCustomProperty("ChampionPulseSpeed", PulseSpeed);
            enemy.SetCustomProperty("ChampionHasHalo", HasHalo);

            DLogger.Log($"ChampionVisuals: Applied to enemy (Level {enemy.ChampionLevel})");
        }

        ///<summary>
        ///Update champion visual effects.
        ///</summary>
        ///<param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            _pulseTime += deltaTime * PulseSpeed;

            //Update pulse effect
            var pulseValue = (MathF.Sin(_pulseTime) + 1f) / 2f; //0 to 1
            var currentIntensity = AuraIntensity + (pulseValue * 0.2f);

            //Update particles
            foreach (var particle in Particles)
            {
                particle.Update(deltaTime);
            }
        }

        ///<summary>
        ///Get current aura intensity with pulse effect.
        ///</summary>
        ///<returns>Current aura intensity.</returns>
        public float GetCurrentAuraIntensity()
        {
            var pulseValue = (MathF.Sin(_pulseTime) + 1f) / 2f;
            return AuraIntensity + (pulseValue * 0.2f);
        }

        ///<summary>
        ///Clone champion visuals.
        ///</summary>
        ///<returns>Cloned visuals.</returns>
        public ChampionVisuals Clone()
        {
            return new ChampionVisuals
            {
                AuraColor = this.AuraColor,
                AuraIntensity = this.AuraIntensity,
                ScaleMultiplier = this.ScaleMultiplier,
                GlowIntensity = this.GlowIntensity,
                Particles = new List<ParticleEffect>(this.Particles),
                PulseSpeed = this.PulseSpeed,
                HasHalo = this.HasHalo
            };
        }
    }

    ///<summary>
    ///Particle effect for champion visuals.
    ///</summary>
    public class ParticleEffect
    {
        public string EffectType { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Lifetime { get; set; }
        public float Size { get; set; }
        public Vector4 Color { get; set; }

        private float _age = 0f;

        public ParticleEffect()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Lifetime = 1f;
            Size = 1f;
            Color = new Vector4(1f, 1f, 1f, 1f);
        }

        ///<summary>
        ///Update particle effect.
        ///</summary>
        ///<param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            _age += deltaTime;
            Position += Velocity * deltaTime;

            //Fade out near end of lifetime
            if (_age > Lifetime * 0.8f)
            {
                var fadeProgress = (_age - Lifetime * 0.8f) / (Lifetime * 0.2f);
                Color = new Vector4(Color.X, Color.Y, Color.Z, 1f - fadeProgress);
            }
        }

        ///<summary>
        ///Check if particle is still alive.
        ///</summary>
        ///<returns>True if alive.</returns>
        public bool IsAlive => _age < Lifetime;
    }
}

