// =====================================================================================================
//  FILE: UpgradePanel-Transitions.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Transitions.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
//
//  ROLE:
//      Provides deterministic transition‑animation behavior for UpgradePanel.
//      This partial isolates all timing and transition‑state logic from rendering,
//      layout, upgrade logic, and font/color management.
// =====================================================================================================

// Fixed Namespace to align perfectly with the driver file UpgradePanel.cs
namespace SASZombieAssaultTD.Engine.UI.HUD
{
    // Fixed access modifier from 'internal' to 'public' to match driver declaration
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Transition Control
        // ---------------------------------------------------------------------------------------------

        private void StartTransition()
        {
            _transitionTimer = 0f;
            _isTransitioning = true;
        }

        private void UpdateTransition(float deltaTime)
        {
            _transitionTimer += deltaTime;

            if (_transitionTimer >= _transitionDuration)
            {
                _isTransitioning = false;
                _transitionTimer = 0f;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Transition Progress
        // ---------------------------------------------------------------------------------------------

        private float GetTransitionProgress()
        {
            if (!_isTransitioning)
                return 1f;

            return System.Math.Clamp(_transitionTimer / _transitionDuration, 0f, 1f);
        }
    }
}
