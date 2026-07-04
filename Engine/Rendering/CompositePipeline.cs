////Program Name: CompositePipeline.cs

////File Path: Engine/Rendering/CompositePipeline.cs

////Program Purpose: The program provides a factory method for creating composite pipeline builders.

////Program Features:

////- Creates CompositeBuilder with loader, creator, and merger components

////- Provides centralized wiring for composite pipeline

///

//

using Engine.Rendering;
using SASZombieAssaultTD.Engine.Rendering;

public static class CompositePipeline

{
    public static CompositeBuilder Create()

    {
        var loader = new CompositeImageLoader();

        var creator = new CompositeCreator();

        var merger = new CompositeMergeTool();

        return new CompositeBuilder(loader, creator, merger);
    }
}
