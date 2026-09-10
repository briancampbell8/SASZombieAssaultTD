// =====================================================================================================
//  FILE: HUDControllerExtensions.cs
//  PATH: Engine/UI/HUD/HUDControllerExtensions.cs
//  MODULE: UI / HUD
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for HUDManager to simplify visibility control,
//      asset‑load verification, and safe access to HUD texture handles.
//
//  RESPONSIBILITIES:
//      - Provide ToggleHUD(), ShowHUD(), HideHUD() behavior for the HUD subsystem.
//      - Provide EnsureHUDTextures() behavior for the HUD subsystem.
//      - Provide SafeLeftTexture() and SafeRightTexture() behavior for the HUD subsystem.
//      - Improve readability and maintainability of HUDManager usage without modifying core code.
//      - Remain pure: no side effects outside HUDManager’s own state fields.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or GPU operations (handled by HUDRenderer / ModernUIRenderer).
//      - Managing HUD layout, positioning, or widget hierarchies (HUDManager does not expose these).
//      - Allocating UI elements or mutating global UI registries.
//      - Replacing or overriding HUDManager’s deterministic behavior.
//
//  NOTES:
//      Relocated from Engine/Extensions to Engine/UI/HUD.
//      HUDManager is intentionally minimal; extensions provide convenience without expanding subsystem
//      responsibilities.
// =====================================================================================================

using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Deterministic helper extensions for HUDManager.
    /// </summary>
    public static class HUDControllerExtensions
    {
        /// <summary>
        /// Toggles the global HUD visibility flag.
        /// </summary>
        public static void ToggleHUD(this HUDManager manager)
        {
            if (manager == null)
                return;

            manager.IsVisible = !manager.IsVisible;
        }

        /// <summary>
        /// Forces the HUD to be visible.
        /// </summary>
        public static void ShowHUD(this HUDManager manager)
        {
            if (manager == null)
                return;

            manager.IsVisible = true;
        }

        /// <summary>
        /// Forces the HUD to be hidden.
        /// </summary>
        public static void HideHUD(this HUDManager manager)
        {
            if (manager == null)
                return;

            manager.IsVisible = false;
        }

        /// <summary>
        /// Ensures HUD textures are loaded deterministically.
        /// </summary>
        public static void EnsureHUDTextures(this HUDManager manager)
        {
            if (manager == null)
                return;

            manager.EnsureAssetsLoaded();
        }

        /// <summary>
        /// Retrieves the left HUD texture safely.
        /// </summary>
        public static Texture2D? SafeLeftTexture(this HUDManager manager)
        {
            if (manager == null)
                return null;

            manager.EnsureAssetsLoaded();
            return manager.GetLeftTexture();
        }

        /// <summary>
        /// Retrieves the right HUD texture safely.
        /// </summary>
        public static Texture2D? SafeRightTexture(this HUDManager manager)
        {
            if (manager == null)
                return null;

            manager.EnsureAssetsLoaded();
            return manager.GetRightTexture();
        }
    }
}
