using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Assets
{
    public static class PlaceholderTexture
    {
        private static Texture2D _instance;

        public static Texture2D Instance => _instance;

        public static void Initialize()
        {
            if (_instance != null)
                return;

            // Create a 1x1 magenta texture (BGRA format: B=0, G=0, R=255, A=255)
            var magentaPixel = new byte[] { 0, 0, 255, 255 };
            _instance = new Texture2D("Placeholder", 1, 1, magentaPixel);
        }
    }
}
