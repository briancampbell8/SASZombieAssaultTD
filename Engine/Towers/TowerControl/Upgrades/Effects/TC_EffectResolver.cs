// =====================================================================================================
//  FILE: TC_EffectResolver.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Effects/TC_EffectResolver.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Effects Subsystem
//
//  ROLE:
//      Provides deterministic resolution of upgrade effect definitions into concrete effect
//      instances using the TowerControl effect subsystem. Acts as the bridge between static
//      effect definitions and runtime effect construction.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize resolver state and bind to definition and builder subsystems.
//      - Resolve effect definitions by key and construct corresponding effect instances.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and resolution operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing effect definitions (handled by TC_EffectDefinitions).
//      - Building effect instances (handled by TC_EffectBuilder).
//      - Applying effects, triggering animations, or performing rendering operations.
//      - Managing persistence, loading, or saving operations.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly as a resolver; does not mutate external engine state.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Definitions;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Effects
{
    internal sealed class TC_EffectResolver
    {
        private TC_EffectDefinitions _definitions;
        private TC_EffectBuilder _builder;

        public void Initialize(TC_EffectDefinitions definitions, TC_EffectBuilder builder)
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_EffectResolver: Initialize - Binding definition and builder subsystems.");

            // Inline comment: store subsystem references for resolution operations
            _definitions = definitions;
            _builder = builder;
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_EffectResolver: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_EffectResolver: Shutdown - Clearing subsystem references.");

            // Inline comment: clear internal references for deterministic teardown
            _definitions = null;
            _builder = null;
        }

        public BuiltEffect ResolveEffect(string key)
        {
            // Trace resolution request
            Trace.WriteLine($"TC_EffectResolver: ResolveEffect - Resolving effect for key '{key}'.");

            // Guard against uninitialized state
            if (_definitions == null || _builder == null)
            {
                Trace.WriteLine("TC_EffectResolver: ResolveEffect - Resolver not initialized; returning null.");
                return null;
            }

            // Inline comment: retrieve effect definition from registry
            var definition = _definitions.GetDefinition(key);

            if (definition == null)
            {
                // Trace missing definition
                Trace.WriteLine($"TC_EffectResolver: ResolveEffect - No definition found for key '{key}'.");
                return null;
            }

            // Inline comment: build effect instance using builder subsystem
            var builtEffect = _builder.BuildEffect(definition);

            // Trace successful resolution
            Trace.WriteLine($"TC_EffectResolver: ResolveEffect - Effect '{key}' resolved successfully.");

            return builtEffect;
        }
    }
}
