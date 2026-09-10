// =====================================================================================================
//  FILE: ParticleSystem.cs
//  PATH: Engine/Render/Particles/ParticleSystem.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Top‑level orchestrator for all particle subprograms.
//      Delegates emission, simulation, pooling, and rendering to:
//          • ParticleCore
//          • ParticleEmitter
//          • ParticleSimulation
//          • ParticleRender
//          • ParticlePool
//
//  RESPONSIBILITIES:
//      - Initialize particle subprograms.
//      - Maintain deterministic update sequencing.
//      - Provide a unified API for particle operations.
//      - Route calls to ParticleCore and its registered subprograms.
//
//  NON-RESPONSIBILITIES:
//      - Performing particle simulation directly.
//      - Rendering particles.
//      - Managing emitter logic.
//      - Handling pooling or randomization.
//
//  ARCHITECTURAL NOTES:
//      - Manager node; not a worker.
//      - All particle behavior resides in subprograms.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    public sealed class ParticleSystem
    {
        private readonly ParticleCore _core;
        private readonly ParticleEmitter _emitter;
        private readonly ParticleSimulation _simulation;
        private readonly ParticleRender _render;
        private readonly ParticlePool _pool;

        // Track emitter components so we can call the emitter micro‑program correctly.
        private readonly List<ParticleEmitterComponent> _emitters = new();

        private bool _initialized;

        public ParticleSystem()
        {
            _core = new ParticleCore();
            _emitter = new ParticleEmitter(_core);
            _simulation = new ParticleSimulation(_core);
            _render = new ParticleRender(_core);
            _pool = new ParticlePool(_core);
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _pool.Initialize();
            _initialized = true;
        }

        // Register an emitter component so the system can update it each frame.
        public void RegisterEmitter(ParticleEmitterComponent emitter)
        {
            if (emitter == null)
                return;

            if (!_emitters.Contains(emitter))
                _emitters.Add(emitter);
        }

        public void UnregisterEmitter(ParticleEmitterComponent emitter)
        {
            if (emitter == null)
                return;

            _emitters.Remove(emitter);
        }

        public void Update(float deltaTime)
        {
            if (!_initialized)
                return;

            // Update each registered emitter component (calls ParticleEmitter.Update(component, deltaTime))
            for (int i = 0; i < _emitters.Count; i++)
                _emitter.Update(_emitters[i], deltaTime);

            _simulation.Update(deltaTime);
        }

        public void Render(D3D11Adapter_Core context)
        {
            if (!_initialized)
                return;

            _render.Render(context);
        }

        public void Stop()
        {
            if (!_initialized)
                return;

            _core.ClearAll();
        }

        public void Pause()
        {
            _core.IsPaused = true;
        }

        public void Resume()
        {
            _core.IsPaused = false;
        }
    }
}