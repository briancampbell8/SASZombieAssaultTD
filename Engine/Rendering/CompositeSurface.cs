////Program Name: CompositeSurface.cs
////File Path: Engine/Rendering/CompositeSurface.cs
////Program Purpose: The program provides a pixel data buffer for storing composite texture data.
////Program Features:
////- Stores width and height dimensions
////- Stores pixel data as an integer array
////- Initializes pixel array with specified dimensions in constructor
//

using Engine.Rendering.Interfaces;

public sealed class CompositeSurface : ICompositeSurface
{
    public int Width { get; }
    public int Height { get; }

    public int[] Pixels { get; }

    public CompositeSurface(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new int[width * height];
    }
}
