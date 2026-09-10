## Implementation Plan

### Consolidate Pipeline Coordination
- [ ] HUDPanelFinalizer_Manager.cs should be the single coordinator.
- [ ] Remove or repurpose HUDManager.FinalizeHUDColors() so it does not duplicate the coordinator’s responsibilities.

### Render() Should Not Perform Pipeline Logic
- [ ] Remove color-selection or pipeline calls from HUDManager.Render().
- [ ] Render() should only render.

### Strengthen Comment Headers
- [ ] Replace minimal comment headers with robust, detailed // comment headers.
- [ ] No TODOs.
- [ ] No NotImplementedException.
- [ ] No /* */ block comments.

### Clarify Lifecycle Timing
- [ ] In HUDPanelFinalizer_Manager.cs, specify exactly when InitializeHUDColors() runs:
  - After HUD panels are instantiated
  - Before rendering begins
  - Only once per HUD initialization cycle

### HUDPanel_Finalizer.Apply()
- [ ] Add a detailed comment header describing its purpose, responsibilities, and constraints.
- [ ] Confirm it uses this.ManualFillColor and this.ManualTextColor as the source of truth.