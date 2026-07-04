using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.Waves;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Economy;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Dictionary
{
    public static class RenderSystemExtensions
    {
        //Extension methods go here
        public static void DrawSprite(this object renderSystem, object sprite, object position, object color) { }
    }

    public static class PlacementInfoExtensions
    {
        //Extension methods go here
        public static void HandleInput(this UIElement element, object inputEvent) { }
    }

    public static class WaveDirectorExtensions
    {
        //Extension methods go here
        public static void Reset(this WaveDirector director) { }
    }

    public static class AudioSystemExtensions
    {
        //Extension methods go here
        public static void PlaySound() { /* Static AudioSystem method */ }
    }

    public static class EconomyManagerExtensions
    {
        //Extension methods go here
        public static void Reset() { EconomyManager.Reset(); }
    }

    public static class AssetRegistryExtensions
    {
        //Extension methods go here
        public static object GetAssetRegistryAll() => new object[0];
    }
}
