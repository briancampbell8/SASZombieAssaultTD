// =====================================================================================================
//  FILE: RendererControl.cs
//  PATH: Engine/Towers/TowerPlacement/RendererControl.cs
//  SUBSYSTEM: Towers TowerPlacement
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    public sealed class RendererControl
    {
        private readonly State _state;
        private readonly PlacementRenderer _renderer;

        public RendererControl(State state)
        {
            _state = state;
            _renderer = new PlacementRenderer();
        }

        public void Initialize(TowerData data)
        {
            try
            {
                _renderer.Initialize(data);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"RendererControl.Initialize error: {ex.Message}");
            }
        }

        public void UpdatePosition(Vector3 worldPos, Vector3Int gridPos, bool canPlace)
        {
            try
            {
                _renderer.UpdatePosition(worldPos, gridPos, canPlace);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"RendererControl.UpdatePosition error: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            try
            {
                // PlacementRenderer has no Cleanup() method — safe no-op.
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"RendererControl.Cleanup error: {ex.Message}");
            }
        }
        public void Render()
        {
            if (!_state.IsActive)
                return;

            try
            {
                _renderer.Render();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"RendererControl.Render error: {ex.Message}");
            }
        }

        public void Update(float deltaTime)
        {
            if (!_state.IsActive)
                return;

            try
            {
                _renderer.Update(deltaTime);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, $"RendererControl.Update error: {ex.Message}");
            }
        }

    }
}
