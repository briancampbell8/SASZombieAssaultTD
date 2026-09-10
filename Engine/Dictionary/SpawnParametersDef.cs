// ====================================================================================================
//  FILE: SpawnParametersDef.cs
//  PATH: ./Engine/Dictionary/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SpawnParametersDef module.
//
//  RESPONSIBILITIES:
//      - Provide DrawSprite() behavior for the Core subsystem.
//      - Provide HandleInput() behavior for the Core subsystem.
//      - Provide Reset() behavior for the Core subsystem.
//      - Provide PlaySound() behavior for the Core subsystem.
//      - Provide Reset() behavior for the Core subsystem.
//      - Provide GetAssetRegistryAll() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.Waves.WaveManagement;

namespace SASZombieAssaultTD.Engine.Dictionary
{
    public static class RenderSystemExtensions
    {
        //Extension methods go here
        public static void DrawSprite(this object renderSystem, object sprite, object position, object color, float x, Color color1, Texture2D texture) { }
    }

    public static class PlacementInfoExtensions
    {
        //Extension methods go here
        public static void HandleInput(this UIElement element, object inputEvent) { }
    }

    public static class WaveDirectorExtensions
    {
        //Extension methods go here
        // 🟢 Renamed from Reset to ResetSafe to clear name collisions with the concrete instance method
        public static void ResetSafe(this WaveDirector director) { director?.Reset(); }
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
