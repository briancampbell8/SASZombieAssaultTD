namespace SASZombieAssaultTD.Engine.Rendering
{
    public interface IRenderContext
    {
        void DrawTexture(Texture2D texture, int x, int y);
        void DrawText(string text, int x, int y);
        void ClearScreen();
    }
}