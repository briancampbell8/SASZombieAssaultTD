// ====================================================================================================
//  FILE: CompositeBuilder.cs
//  PATH: Engine/Render/Composite
//  MODULE: Rendering Subsystem – Composition Infrastructure
//
//  ROLE:
//      Deterministic dependency builder for the composition infrastructure.
//      Assembles and exposes components required for CPU-side image composition.
//
//  RESPONSIBILITIES:
//      - Hold references to ICompositeImageLoader, ICompositeCreator, and ICompositeMergeTool.
//      - Provide a stable, deterministic assembly point for composition-related dependencies.
//      - Allow higher-level systems to retrieve the components they need for building composite flows.
//
//  NON-RESPONSIBILITIES:
//      - Creating or running the CompositePipeline.
//      - Performing composition or GPU rendering.
//      - Resource caching or lookup.
//      - Gameplay logic, UI layout, or diagnostics.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.Render.Composite
{
    public sealed class CompositeBuilder
    {
        public ICompositeImageLoader Loader { get; }

        private ICompositeCreator Creator;

        public ICompositeMergeTool Merger { get; }

        public CompositeBuilder(
            ICompositeImageLoader loader,
            ICompositeCreator creator,
            ICompositeMergeTool merger)
        {
            Loader = loader;
            Creator = creator;
            Merger = merger;
        }
    }
}
