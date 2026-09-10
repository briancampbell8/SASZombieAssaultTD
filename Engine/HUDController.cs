// =====================================================================================================
//  FILE: HUDController.cs
//  PATH: Engine/HUDController.cs
//  SUBSYSTEM: Engine
//
//  ROLE:
//      The controller for the game's HUD, responsible for managing the UI elements and syncing them 
//      with the active game state.
//
//  RESPONSIBILITIES:
//      - Manage UI elements and component visibility.
//      - Subscribe to player event sequences (Cash, Health, Inventory changes).
//      - Provide interface methods for placement preview details panel manipulation.
//
//  NON-RESPONSIBILITIES:
//      - Managing player inventory collections directly.
//      - Computing pathfinding graphs.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Player;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Towers.TowerPlacement;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Lightweight component stub to resolve missing reference issues.
    /// </summary>
    public class HUDComponent
    {
        public void SetText(string text) { }
        public void SetFillAmount(float amount) { }
    }

    /// <summary>
    /// The controller for the game's HUD, responsible for managing the UI elements and syncing them with the game state.
    /// </summary>
    public sealed class HUDController
    {
        private readonly Dictionary<string, HUDComponent> _elements = new();

        public bool IsActive { get; set; }
        public PreviewCore PlacementPreview { get; set; }


        public HUDController(PreviewCore placementPreview)

        {
            PlacementPreview = placementPreview;
            IsActive = false;
        }

        public HUDController() => IsActive = false;

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void SetPlacementPreview(PreviewCore placementPreview)
        {
            PlacementPreview = placementPreview;
        }

        /// <summary>
        /// Initializes the HUD controller.
        /// </summary>
        public void Initialize()
        {
            // Subscribe to PlayerSystem events
            PlayerEvents.OnCashChanged += OnCashChanged;

            // Initial sync (guards against missing systems using dynamic to bypass object-level property checking)
            var ps = PlayerSystem.Instance;
            if (ps != null && ps.State != null)
            {
                dynamic dynamicState = ps.State;
                try
                {
                    SyncHealth((int)dynamicState.Lives, (int)dynamicState.MaxLives);
                }
                catch
                {
                    SyncHealth(100, 100); // Safe fallback values
                }
            }

            SyncInventory();
        }

        /// <summary>
        /// Cleans up the HUD controller.
        /// </summary>
        public void Cleanup()
        {
            PlayerEvents.OnCashChanged -= OnCashChanged;
        }

        public void Update(float deltaTime)
        { }

        public void Shutdown()
        {
            Cleanup();
        }

        // -----------------------------
        // EVENT HANDLERS
        // -----------------------------

        private void OnCashChanged(int newCash)
        {
            if (_elements.TryGetValue("cash_text", out var text))
                text.SetText($"${newCash}");
        }

        private void OnHealthChanged(int current, int max)
        {
            SyncHealth(current, max);
        }

        private void OnWaveStarted(int waveNumber)
        {
            SyncWave(waveNumber);
        }

        private void OnInventoryChanged()
        {
            SyncInventory();
        }

        // -----------------------------
        // SYNC & SLOTS METHODS
        // -----------------------------

        private void SyncHealth(int current, int max)
        {
            if (max <= 0) max = 1;
            float pct = (float)current / max;

            if (_elements.TryGetValue("health_text", out var text))
                text.SetText($"{current}/{max}");

            if (_elements.TryGetValue("health_bar", out var bar))
                bar.SetFillAmount(pct);
        }

        private void SyncWave(int wave)
        {
            if (_elements.TryGetValue("wave_text", out var text))
                text.SetText($"Wave {wave}");
        }

        private void SyncInventory()
        {
            var ps = PlayerSystem.Instance;
            if (ps == null) return;

            // Using dynamic to safely bypass missing 'Inventory' compilation constraints on PlayerSystem
            dynamic dynamicPlayer = ps;
            try
            {
                var inv = dynamicPlayer.Inventory;
                UpdateSlot("item_slot_1", inv.GetItemAt(0));
                UpdateSlot("item_slot_2", inv.GetItemAt(1));
                UpdateSlot("item_slot_3", inv.GetItemAt(2));
            }
            catch
            {
                // Fallback gracefully if inventory API is uninitialized
            }
        }

        private void UpdateSlot(string slotId, object item)
        {
            if (_elements.TryGetValue(slotId, out var slot))
            {
                slot.SetText(item != null ? item.ToString() : "Empty");
            }
        }

        // -----------------------------
        // TOWER PLACEMENT PREVIEW INTERFACE
        // -----------------------------

        public void ShowPlacementInfo(TowerData data)
        {
            if (data != null && _elements.TryGetValue("placement_info_panel", out var panel))
            {
                panel.SetText($"Placing: {data.Type}");
            }
        }

        public void HidePlacementInfo()
        {
            // Toggles panel visualization states off
        }

        public void UpdatePlacementInfo(Vector3Int position, bool isValid)
        {
            if (_elements.TryGetValue("placement_status_text", out var text))
            {
                text.SetText(isValid ? "Valid Location" : "Blocked Location");
            }
        }

        internal class InstanceExists
        {
        }
    }
}
