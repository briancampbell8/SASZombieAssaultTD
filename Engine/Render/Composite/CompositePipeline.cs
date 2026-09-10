// ====================================================================================================
//  FILE: CompositePipeline.cs
//  PATH: Engine/Render/Composite
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide Create() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.Render.Composite
{
    public static class CompositePipeline

    {
        private static ICompositeCreator creator;

        public static CompositeBuilder Create()

        {
            var loader = new CompositeImageLoader();

            // var creator = new CompositeCreator();


            var merger = new CompositeMergeTool();

            return new CompositeBuilder(
                loader,
                (ICompositeCreator)creator,
                merger);
        }
    }
}
