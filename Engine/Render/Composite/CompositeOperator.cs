// =====================================================================================================
//  FILE: CompositeOperator.cs
//  PATH: Engine/Render/Composite/CompositeOperator.cs
//  SUBSYSTEM: Render Composite
//
//  ROLE:
//      Deterministic operator wrapper for composite-surface operations.
//      Provides explicit conversion from ITextureSurface into a CompositeOperator instance.
//
//  RESPONSIBILITIES:
//      - Hold a reference to the source ITextureSurface.
//      - Provide deterministic operator behavior for composite pipelines.
//      - Serve as a stable identity for composite operations.
//
//  NON-RESPONSIBILITIES:
//      - Texture decoding.
//      - GPU upload.
//      - Resource lifetime management.
//
//  ARCHITECTURAL NOTES:
//      - Pure micro-class.
//      - Deterministic, immutable surface reference.
//      - No assumptions or hidden responsibilities.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.Render.Composite
{
    public sealed class CompositeOperator
    {
        public ITextureSurface Surface { get; }

        public CompositeOperator(ITextureSurface surface)
        {
            Surface = surface;
        }

        // LEGAL replacement for the forbidden operator
        public static CompositeOperator FromSurface(ITextureSurface surface)
        {
            return new CompositeOperator(surface);
        }
    }

}
