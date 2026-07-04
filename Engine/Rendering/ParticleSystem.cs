// ====================================================================================================
//  FILE: ParticleSystem.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: ParticleSystem.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

/*
File:    ParticleSystem.cs
Purpose: Subsystem for managing particle effects and simulation.
Features: Particle pooling, emission, lifetime management, physics simulation, rendering integration.

P11-03-04-B: Subsystem maintains particle pool, spawns particles based on emitter settings,
updates particle properties, removes expired particles, and handles rendering data.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Resources;
//Alias directives to resolve ambiguity
using Components_TransformComponent = SASZombieAssaultTD.Engine.Components.TransformComponent;
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Represents a single particle in the simulation.
    ///</summary>
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public Color Color { get; set; }
        public float Lifetime { get; set; }
        public float Age { get; set; }
        public bool IsAlive { get; set; } = true;

        public Particle(Vector3 position, Vector3 velocity, float rotation, float scale, Color color, float lifetime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            Lifetime = lifetime;
            Age = 0.0f;
            IsAlive = true;
        }

        ///<summary>
        ///Updates the particle's state based on elapsed time.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since the last update.</param>
        public void Update(float deltaTime)
        {
            if (!IsAlive) return;

            Age += deltaTime;

            //Update position based on velocity
            Position += Velocity * deltaTime;

            //Check if the particle has expired
            if (Age >= Lifetime)
                IsAlive = false;
        }

        public override string ToString()
        {
            return $"Particle(Pos: {Position}, Age: {Age:F2}/{Lifetime:F2}, Alive: {IsAlive})";
        }
    }

    ///<summary>
    ///Subsystem for managing particle effects and simulation.
    ///</summary>
    public class ParticleSystem
    {
        private readonly EntityManager _entityManager;
        private readonly RSManager _assetManager;
        private readonly EventRouter _eventBus;
        private readonly List<Particle> _particles = new();
        private readonly Queue<Particle> _particlePool = new();
        private readonly string _particleAssetId = "particle_default"; //Default particle asset

        private bool _initialized;
        private readonly bool _debugOutput = true;
        private bool _isPaused = false;
        private readonly Random _random = new Random();

        private static ParticleSystem _instance;
        public static ParticleSystem Instance => _instance ??= new ParticleSystem(null, null, null);

        ///<summary>
        ///Creates a new ParticleSystem with required dependencies.
        ///</summary>
        ///<param name="entityManager">Entity manager for component access.</param>
        ///<param name="assetManager">Asset manager for particle textures.</param>
        ///<param name="eventBus">Event bus for system communication.</param>
        public ParticleSystem(EntityManager entityManager, RSManager assetManager, EventRouter eventBus)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            DebugLog("ParticleSystem: Constructed with required dependencies");
        }

        ///<summary>
        ///Initializes the particle system.
        ///</summary>
        public void Initialize()
        {
            if (_initialized) return;

            try
            {
                DebugLog("ParticleSystem: Starting initialization...");

                //Pre-populate particle pool
                for (int i = 0; i < 200; i++)
                {
                    _particlePool.Enqueue(new Particle(Vector3.Zero, Vector3.Zero, 0.0f, 1.0f, Color.White, 1.0f));
                }

                _initialized = true;
                DebugLog("ParticleSystem: Initialization complete - Particle pool initialized with 200 particles");
            }
            catch (Exception ex)
            {
                DebugLog($"ParticleSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize ParticleSystem", ex);
            }
        }

        ///<summary>
        ///Updates all particles and emitters based on elapsed time.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since the last update.</param>
        public void Update(float deltaTime)
        {
            if (!_initialized)
            {
                DebugLog("ParticleSystem: Update failed - Not initialized");
                return;
            }

            try
            {
                //Update existing particles
                for (int i = _particles.Count - 1; i >= 0; i--)
                {
                    var particle = _particles[i];
                    particle.Update(deltaTime);

                    if (!particle.IsAlive)
                    {
                        //Remove dead particle and return to pool
                        _particles.RemoveAt(i);
                        ReturnParticleToPool(particle);
                    }
                }

                //Process emitters and spawn new particles
                ProcessEmitters(deltaTime);

                DebugLog($"ParticleSystem: Update complete - Active particles: {_particles.Count}");
            }
            catch (Exception ex)
            {
                DebugLog($"ParticleSystem: Update failed - {ex.Message}");
            }
        }

        ///<summary>
        ///Processes all particle emitters and spawns new particles.
        ///</summary>
        private void ProcessEmitters(float deltaTime)
        {
            var emitters = GetEntitiesWithParticleEmitterAndTransform();

            foreach (var entity in emitters)
            {
                var emitter = _entityManager.GetComponent<ParticleEmitterComponent>((uint)entity);
                var transform = _entityManager.GetComponent<Components_TransformComponent>((uint)entity);

                if (emitter == null || transform == null || !emitter.IsActive)
                    continue;

                //Update emission accumulator
                emitter.EmissionAccumulator += deltaTime;

                //Calculate how many particles to emit this frame
                var particlesToEmit = (int)(emitter.EmissionAccumulator * emitter.EmissionRate);
                emitter.EmissionAccumulator -= particlesToEmit / emitter.EmissionRate;

                //Spawn particles
                for (int i = 0; i < particlesToEmit && _particles.Count < emitter.MaxParticles; i++)
                {
                    SpawnParticle(emitter, transform);
                }
            }
        }

        ///<summary>
        ///Spawns a single particle from an emitter.
        ///</summary>
        private void SpawnParticle(ParticleEmitterComponent emitter, Components_TransformComponent transform)
        {
            var particle = GetParticleFromPool();
            if (particle == null) return;

            //Set initial particle properties from emitter ranges
            particle.Position = transform.Position;

            var velocity = emitter.InitialVelocityRange.GetRandom();
            particle.Velocity = new Vector3(velocity.X, velocity.Y, 0);

            var rotation = emitter.InitialRotationRange.GetRandom();
            particle.Rotation = rotation;
            var scale = emitter.InitialScaleRange.GetRandom();
            particle.Scale = scale;
            var sysColor = emitter.ColorTintRange.GetRandom();
            particle.Color = new Color(sysColor.R, sysColor.G, sysColor.B, sysColor.A);
            particle.Lifetime = emitter.LifetimeRange.GetRandom();
            particle.Age = 0.0f;
            particle.IsAlive = true;

            _particles.Add(particle);
        }

        ///<summary>
        ///Gets a particle from the pool or creates a new one if the pool is empty.
        ///</summary>
        private Particle? GetParticleFromPool()
        {
            return _particlePool.Count > 0 ? _particlePool.Dequeue() : new Particle(Vector3.Zero, Vector3.Zero, 0.0f, 1.0f, Color.White, 1.0f);
        }

        ///<summary>
        ///Returns a particle to the pool for reuse.
        ///</summary>
        private void ReturnParticleToPool(Particle particle)
        {
            if (_particlePool.Count < 200) //Limit pool size
            {
                particle.Age = 0.0f;
                particle.IsAlive = true;
                _particlePool.Enqueue(particle);
            }
        }

        ///<summary>
        ///Gets all entities that have both ParticleEmitterComponent and TransformComponent.
        ///</summary>
        private IReadOnlyList<object> GetEntitiesWithParticleEmitterAndTransform()
        {
            var result = new List<object>();
            var entities = _entityManager.Entities;

            foreach (var entity in entities)
            {
                if (_entityManager.HasComponent<ParticleEmitterComponent>((uint)entity) &&
                    _entityManager.HasComponent<Components_TransformComponent>((uint)entity))
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        ///<summary>
        ///Stops the particle system and clears all particles.
        ///</summary>
        public void Stop()
        {
            if (!_initialized) return;

            _particles.Clear();
            DebugLog("ParticleSystem: Stopped - All particles cleared");
        }

        ///<summary>
        ///Pauses the particle system.
        ///</summary>
        public void Pause()
        {
            _isPaused = true;
            DebugLog("ParticleSystem: Paused");
        }

        ///<summary>
        ///Resumes the particle system.
        ///</summary>
        public void Resume()
        {
            _isPaused = false;
            DebugLog("ParticleSystem: Resumed");
        }

        ///<summary>
        ///Starts confetti particle effects.
        ///</summary>
        public void StartConfetti()
        {
            if (!_initialized) return;

            //Create confetti particles
            for (int i = 0; i < 50; i++)
            {
                var particle = GetParticleFromPool();
                if (particle != null)
                {
                    particle.Position = new Vector3(
                        (float)_random.NextDouble() * 800 - 400,
                        (float)_random.NextDouble() * 600 - 300,
                        0
                    );
                    particle.Velocity = new Vector3(
                        (float)_random.NextDouble() * 200 - 100,
                        (float)_random.NextDouble() * 300 + 100,
                        0
                    );
                    particle.Color = new Color(
                        (byte)_random.Next(256),
                        (byte)_random.Next(256),
                        (byte)_random.Next(256),
                        255
                    );
                    particle.Lifetime = 3.0f;
                    particle.IsAlive = true;
                    _particles.Add(particle);
                }
            }
            DebugLog("ParticleSystem: Started confetti effects");
        }

        ///<summary>
        ///Stops confetti particle effects.
        ///</summary>
        public void StopConfetti()
        {
            if (!_initialized) return;

            //Remove confetti particles (simplified - just clear all)
            _particles.Clear();
            DebugLog("ParticleSystem: Stopped confetti effects");
        }

        ///<summary>
        ///Renders all particles.
        ///</summary>
        ///<param name="context">Render context</param>
        public void Render(IRenderContext context)
        {
            if (!_initialized || _isPaused) return;

            foreach (var particle in _particles)
            {
                if (particle.IsAlive)
                {
                    //Simple particle rendering - would integrate with actual rendering system
                    RenderSystem renderSystem = new RenderSystem();
                    renderSystem.DrawRectangle(
                        particle.Position.X - particle.Scale * 0.5f,
                        particle.Position.Y - particle.Scale * 0.5f,
                        particle.Scale,
                        particle.Scale,
                        particle.Color
                    );
                }
            }
        }

        ///<summary>
        ///Logs debug messages if debug output is enabled.
        ///</summary>
        private void DebugLog(string message)
        {
            if (_debugOutput)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }
}




