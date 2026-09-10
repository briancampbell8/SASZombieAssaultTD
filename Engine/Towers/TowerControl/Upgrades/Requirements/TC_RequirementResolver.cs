// =====================================================================================================
//  FILE: TC_RequirementResolver.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Requirements/TC_RequirementResolver.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Requirements Subsystem
//
//  ROLE:
//      Provides deterministic resolution of upgrade requirement definitions into concrete
//      requirement instances using the TowerControl requirement subsystem. Acts as the bridge
//      between static requirement definitions and runtime requirement construction.
//      Supports initialization, execution entry, and shutdown to align with engine lifecycle flow.
//
//  RESPONSIBILITIES:
//      - Initialize resolver state and bind to definition and builder subsystems.
//      - Resolve requirement definitions by key and construct corresponding requirement instances.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//      - Emit diagnostic trace statements for lifecycle and resolution operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing requirement definitions (handled by TC_RequirementDefinitions).
//      - Building requirement instances (handled by TC_RequirementBuilder).
//      - Evaluating requirement logic or determining whether requirements are met.
//      - Managing persistence, loading, or saving operations.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly as a resolver; does not mutate external engine state.
//      - Follows the engine-hosted lifecycle pattern for consistency across TowerControl subsystems.
//      - Shutdown clears internal references to maintain deterministic teardown behavior.
// =====================================================================================================

using System.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Definitions;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Requirements
{
    internal sealed class TC_RequirementResolver
    {
        private TC_RequirementDefinitions _definitions;
        private TC_RequirementBuilder _builder;

        public void Initialize(TC_RequirementDefinitions definitions, TC_RequirementBuilder builder)
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_RequirementResolver: Initialize - Binding definition and builder subsystems.");

            // Store subsystem references for resolution operations
            _definitions = definitions;
            _builder = builder;
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_RequirementResolver: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_RequirementResolver: Shutdown - Clearing subsystem references.");

            // Clear internal references for deterministic teardown
            _definitions = null;
            _builder = null;
        }

        public BuiltRequirement ResolveRequirement(string key)
        {
            // Trace resolution request
            Trace.WriteLine($"TC_RequirementResolver: ResolveRequirement - Resolving requirement for key '{key}'.");

            // Guard against uninitialized state
            if (_definitions == null || _builder == null)
            {
                Trace.WriteLine("TC_RequirementResolver: ResolveRequirement - Resolver not initialized; returning null.");
                return null;
            }

            // Retrieve requirement definition from registry
            var definition = _definitions.GetDefinition(key);

            if (definition == null)
            {
                // Trace missing definition
                Trace.WriteLine($"TC_RequirementResolver: ResolveRequirement - No definition found for key '{key}'.");
                return null;
            }

            // Build requirement instance using builder subsystem
            var builtRequirement = _builder.BuildRequirement(definition);

            // Trace successful resolution
            Trace.WriteLine($"TC_RequirementResolver: ResolveRequirement - Requirement '{key}' resolved successfully.");

            return builtRequirement;
        }
    }
}
