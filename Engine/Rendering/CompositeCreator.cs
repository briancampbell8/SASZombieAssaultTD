////Program Name: CompositeCreator.cs
////File Path: Engine/Rendering/CompositeCreator.cs
////Program Purpose: The program creates composite surfaces and converts them into engine textures.
////Program Features:
////- Creates composite surfaces with specified dimensions
////- Converts composite surfaces to engine texture format

//

using Engine.Rendering.Interfaces;
using SASZombieAssaultTD.Engine.Diagnostics;

public sealed class CompositeCreator : ICompositeCreator
{
    public ICompositeSurface CreateSurface(int width, int height)
    {
        return new CompositeSurface(width, height);
    }

    public IEngineTexture FinalizeComposite(ICompositeSurface surface)
    {
        CompositeSurface surf = (CompositeSurface)surface;
        return new EngineTexture(surf.Width, surf.Height, surf.Pixels);
    }

    internal EngineTexture CreateTexture(CompositeSurface surface)
    {
        return NI.Hit<EngineTexture>();
    }

    internal EngineTexture CreateTexture(ICompositeSurface surface)
    {
        return NI.Hit<EngineTexture>();
    }
}

public interface IEngineTexture
{
}
