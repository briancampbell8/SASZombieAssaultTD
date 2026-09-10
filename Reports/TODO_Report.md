# TODO Report (Engine Only)
Generated: 07/14/2026 09:18:06

## Grand Total TODOs: 106

### AnimationECSIntegration.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Animation\Integration\AnimationECSIntegration.cs

#### Line 73
```
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world), "ECSWorld cannot be null.");

            //TODO: Fix missing constructor arguments - AnimationTriggerSystem requires EntityManager and EventRouter
            //world.AddSystem(new AnimationTriggerSystem());
            //world.AddSystem(new AnimationUpdateSystem());
            DLogger.Log(LogSubsystems.ResourcesPipeline, "AnimationECSIntegration: Systems not registered - missing constructor arguments");

```

#### Line 93
```
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world), "ECSWorld cannot be null.");

            //TODO: Fix generic type constraint - AnimationTriggerSystem and AnimationUpdateSystem don't implement IECSSystem
            //world.RemoveSystem<AnimationTriggerSystem>();
            //world.RemoveSystem<AnimationUpdateSystem>();

            //Example: Unsubscribe from ECS events
```


### AnimationUpdateSystem.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Animation\Systems\AnimationUpdateSystem.cs

#### Line 231
```
        ///</summary>
        ///<param name="message">The message to log.</param>
        private void DebugLog(string message)
        {
            //TODO: Replace with proper logging system when available
            DLogger.Log($"[AnimationUpdateSystem] {message}");
        }
    }
}
```


### EnemySystem.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Enemies\EnemySystem.cs

#### Line 130
```
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            if (_enemies.Contains(enemy)) return;

            _enemies.Add(enemy);
            //TODO: Fix EntityManager integration when methods are available
            //_entityManager.AddEntity(enemy.Entity);

            _eventBus.Publish(new EnemySpawnedEvent(enemy));
        }
```

#### Line 157
```
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            if (!_enemies.Remove(enemy)) return;

            //TODO: Fix EntityManager integration when methods are available
            //_entityManager.RemoveEntity(enemy.Entity);

        //   _eventBus.Publish(
          //     new EnemyDeathEvent(
```


### EngineBootstrap.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\EngineBootstrap.cs

#### Line 70
```
        ///</summary>
        public void CreateAndInitializeGameRoot()
        {
            //Initialize all systems directly
            //TODO: Verify if these systems need explicit initialization or if constructors handle it
            //_systemRegistry?.Initialize();
            //_systemManager?.Initialize();
            //_updateManager?.Initialize();
            //_renderManager?.Initialize();
```


### BattlefieldFlowManager.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Gameplay\BattlefieldFlowManager.cs

#### Line 176
```
        ///In a real implementation, this would load actual assets.
        ///</summary>
        private void SimulateLoading()
        {
            //TODO: Implement actual asset loading
            //For now, simulate immediate completion
            CompleteLoading();
        }

```


### NavigationDebugRenderer.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Navigation\NavigationDebugRenderer.cs

#### Line 207
```
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderAgentPaths(IRenderContext context)
        {
            //TODO: Fix GetEntitiesWith method call - ECSWorld may not have this method signature
            //var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
            var agents = Array.Empty<Entity>(); //Placeholder to prevent compilation error

            foreach (var entity in agents)
```

#### Line 250
```
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderAgentTargets(IRenderContext context)
        {
            //TODO: Fix GetEntitiesWith method call - ECSWorld may not have this method signature
            //var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
            var agents = Array.Empty<Entity>(); //Placeholder to prevent compilation error

            foreach (var entity in agents)
```


### BGFXDeviceCore.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXDeviceCore.cs

#### Line 61
```
///      Width = width;
///      Height = height;

//BGFX initialization - placeholder for Phase 10+
//TODO: Implement BGFX initialization using BGFXNative.bgfx_init when ready
//This is currently handled by BGFXGraphicsDevice.InitializeBGFX()
///           DLogger.Log(LogSubsystems.ResourcesPipeline, "[BGFX] BGFXDeviceCore.Initialize() - placeholder");

///          DLogger.Log(LogSubsystems.ResourcesPipeline, "[BGFX] BGFXDeviceCore.Initialize() - platform handle: {0}, size: {1}x{2}", windowHandle, width, height);
```


### BGFXFramebuffer.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXFramebuffer.cs

#### Line 76
```

        public void Dispose()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "[DIAG] BGFXFramebuffer.Dispose - STUB PROCESSED");
            //TODO BGFX: Dispose framebuffer resources once BGFX bindings are available.
            //This method intentionally left blank.
        }
    }
}
```


### BGFXGraphicsDeviceCore.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXGraphicsDeviceCore.cs

#### Line 159
```
    ///   }

    ///    public BGFXGraphicsDevice(IntPtr hwnd, int width, int height)
    ///   {
    //TODO BGFX: Initialize BGFX device once BGFX bindings are available.
    //This constructor intentionally left empty.
    ///        DLogger.Log(LogSubsystems.ResourcesPipeline, "[BGFXGraphicsDevice] Constructor called - hwnd: {0}, size: {1}x{2}", hwnd, width, height);
    ///    }

```

#### Line 559
```
///     }

///    public void PresentFramebuffer(Framebuffer fb, ID3D11DeviceContext context)
///   {
//TODO BGFX: Present framebuffer once BGFX bindings are available.
//- Call bgfx.frame() to advance to next frame
//- Handle context parameter for interface compatibility
//- Present the current backbuffer to display
//This method intentionally left blank.
```


### BGFXInitializationHarness.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXInitializationHarness.cs

#### Line 61
```
        ///Whether BGFX shutdown has been called.
        ///</summary>
        public bool ShutdownCalled;

        //TODO: Add frame tracking for future phases
        //- FrameCount: Track number of frames submitted
        //- LastFrameTime: Track timing for performance analysis
        //- FrameErrors: Track frame submission failures
    }
```


### BGFXNoOp.cs — 12 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXNoOp.cs

#### Line 42
```
namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX No-Op wrappers - inert, non-executing wrappers for BGFX API calls.
    ///This class contains only placeholder methods with TODO comments.
    ///</summary>
    internal class BGFXNoOp
    {
        ///<summary>
```

#### Line 51
```
        ///Initialize BGFX - No-Op wrapper.
        ///</summary>
        public static void InitNoOp(object initData)
        {
            //TODO: Call bgfx.init(initData) once activation is enabled.
        }

        ///<summary>
        ///Reset BGFX - No-Op wrapper.
```

#### Line 59
```
        ///Reset BGFX - No-Op wrapper.
        ///</summary>
        public static void Reset(int width, int height, object resetFlags)
        {
            //TODO: Call bgfx.reset(width, height, resetFlags) once activation is enabled.
        }

        ///<summary>
        ///Set view rectangle - No-Op wrapper.
```

#### Line 67
```
        ///Set view rectangle - No-Op wrapper.
        ///</summary>
        public static void SetViewRect(ushort viewId, ushort x, ushort y, ushort width, ushort height)
        {
            //TODO: Call bgfx.setViewRect(viewId, x, y, width, height) once activation is enabled.
        }

        ///<summary>
        ///Set view clear state - No-Op wrapper.
```

#### Line 75
```
        ///Set view clear state - No-Op wrapper.
        ///</summary>
        public static void SetViewClear(ushort viewId, uint clearFlags, uint clearColor, float depth, byte stencil)
        {
            //TODO: Call bgfx.setViewClear(viewId, clearFlags, clearColor, depth, stencil) once activation is enabled.
        }

        ///<summary>
        ///Set vertex buffer - No-Op wrapper.
```

#### Line 83
```
        ///Set vertex buffer - No-Op wrapper.
        ///</summary>
        public static void SetVertexBuffer(byte stream, object vertexBufferHandle, uint startVertex, uint numVertices)
        {
            //TODO: Call bgfx.setVertexBuffer(stream, vertexBufferHandle, startVertex, numVertices) once activation is enabled.
        }

        ///<summary>
        ///Set index buffer - No-Op wrapper.
```

#### Line 91
```
        ///Set index buffer - No-Op wrapper.
        ///</summary>
        public static void SetIndexBuffer(object indexBufferHandle, uint firstIndex, uint numIndices)
        {
            //TODO: Call bgfx.setIndexBuffer(indexBufferHandle, firstIndex, numIndices) once activation is enabled.
        }

        ///<summary>
        ///Set texture - No-Op wrapper.
```

#### Line 99
```
        ///Set texture - No-Op wrapper.
        ///</summary>
        public static void SetTexture(byte stage, object uniformHandle, object textureHandle)
        {
            //TODO: Call bgfx.setTexture(stage, uniformHandle, textureHandle) once activation is enabled.
        }

        ///<summary>
        ///Set uniform - No-Op wrapper.
```

#### Line 107
```
        ///Set uniform - No-Op wrapper.
        ///</summary>
        public static void SetUniform(object uniformHandle, object value, ushort num)
        {
            //TODO: Call bgfx.setUniform(uniformHandle, value, num) once activation is enabled.
        }

        ///<summary>
        ///Submit draw call - No-Op wrapper.
```

#### Line 115
```
        ///Submit draw call - No-Op wrapper.
        ///</summary>
        public static void Submit(ushort viewId, object programHandle, int depth, byte flags)
        {
            //TODO: Call bgfx.submit(viewId, programHandle, depth, flags) once activation is enabled.
        }

        ///<summary>
        ///Advance to next frame - No-Op wrapper.
```

#### Line 123
```
        ///Advance to next frame - No-Op wrapper.
        ///</summary>
        public static void Frame()
        {
            //TODO: Call bgfx.frame() once activation is enabled.
        }

        ///<summary>
        ///Shutdown BGFX - No-Op wrapper.
```

#### Line 131
```
        ///Shutdown BGFX - No-Op wrapper.
        ///</summary>
        public static void Shutdown()
        {
            //TODO: Call bgfx.shutdown() once activation is enabled.
        }
    }
}

```


### BGFXResourceCache.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\BGFX\BGFXResourceCache.cs

#### Line 63
```
        ///</summary>
        public object GetAlphaBlend()
        {
            //BGFX placeholder - return blend state handle
            //TODO: Implement actual BGFX blend state creation
            DLogger.Log(LogSubsystems.ResourcesPipeline, "[BGFX] GetAlphaBlend() - placeholder implementation");
            return null;
        }

```

#### Line 74
```
        ///</summary>
        public object GetSampler()
        {
            //BGFX placeholder - return sampler handle
            //TODO: Implement actual BGFX sampler creation
            DLogger.Log(LogSubsystems.ResourcesPipeline, "[BGFX] GetSampler() - placeholder implementation");
            return null;
        }

```


### D3D11DeviceCoreFullscreenQuad.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\D3D11\D3D11DeviceCoreFullscreenQuad.cs

#### Line 60
```
                LogSubsystems.D3D11,
                LogLevel.Info,
                "D3D11DeviceCore.InitializeFullscreenQuadPipeline: (placeholder) creating quad pipeline.");

            //TODO (future surgical pass):
            //1. Create vertex buffer for fullscreen quad:
            //   [-1,-1], [1,-1], [-1,1], [1,1]
            //
            //2. Compile/load vertex shader:
```

#### Line 104
```
                LogSubsystems.D3D11,
                LogLevel.Info,
                "D3D11DeviceCore.DrawFullscreenTexturedQuad: (placeholder) drawing fullscreen quad.");

            //TODO (future surgical pass):
            //1. Bind RTV (already bound by ClearRenderTarget).
            //2. Bind vertex buffer:
            //   _context.IASetVertexBuffers(...)
            //
```


### RSInitializer.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\RSInitializer.cs

#### Line 117
```
            foreach (var discovered in discoveredAssets)
            {
                //NEW API: ValidateMetadata returns bool
                //bool isValid = AssetValidation.ValidateMetadata(discovered.Metadata);
                bool isValid = true; //TODO: Implement proper validation when AssetValidation exists

                if (!isValid)
                {
                    //NEW API: Key is now a value object; use ToString()
```


### EnemySaveData.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Save\SAS\EnemySaveData.cs

#### Line 599
```
                    return new Enemy();
                case ZombieType.Devastator:
                    return new Enemy();
                case ZombieType.RobotClown:
                    return new Enemy(); //TODO: Implement RobotClownZombie
                default:
                    return new Enemy();
            }
        }
```


### SASGameSave.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Save\SAS\SASGameSave.cs

#### Line 387
```
                TotalWaves = waveDirector?.TotalWaves ?? 10,
                WaveProgress = waveDirector?.WaveProgress ?? 0f,
                OverallProgress = waveDirector?.OverallProgress ?? 0f,
                IsWaveActive = waveDirector?.IsWaveActive ?? false,
                CompletedWaves = new List<int>() //TODO: WaveDirector doesn't have CompletedWaves property
            };
        }

        static void CaptureTowerState(SASGameSave save)
```

#### Line 393
```
        }

        static void CaptureTowerState(SASGameSave save)
        {
            //TODO: TowerUpgradeManager class doesn't exist - using placeholder
            save.Towers = new TowerSaveData
            {
                TowerCount = 0, //Placeholder
                TotalValue = 0, //Placeholder
```

#### Line 421
```
        {
            //Implementation would capture game state
            save.GameState = new GameStateSaveData
            {
                CurrentState = "Unknown", //TODO: Implement proper state machine access
                IsPaused = false,
                GameSpeed = 1.0f,
                AutoSaveEnabled = true,
                LastSaveTime = DateTime.Now
```


### BattlefieldScene.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Scenes\Battlefields\BattlefieldScene.cs

#### Line 201
```
        protected virtual void LoadTilemap()
        {
            DLogger.Log(LogSubsystems.Save,
                LogLevel.Info, $"BattlefieldScene: Loading tilemap from {TilemapPath}");
            //TODO: Implement tilemap loading through asset system
        }

        ///<summary>
        ///Sets up spawn nodes for this battlefield.
```

#### Line 277
```

            //P100 Integration: Apply champion spawn rate
            ApplyChampionSpawnRate(difficultyScaling.ChampionSpawnRate);

            //TODO: Spawn enemies using WaveSpawnGroup with difficulty scaling
            //TODO: Apply DifficultyScaling stat multipliers to spawned enemies
        }

        ///<summary>
```

#### Line 278
```
            //P100 Integration: Apply champion spawn rate
            ApplyChampionSpawnRate(difficultyScaling.ChampionSpawnRate);

            //TODO: Spawn enemies using WaveSpawnGroup with difficulty scaling
            //TODO: Apply DifficultyScaling stat multipliers to spawned enemies
        }

        ///<summary>
        ///Sets the difficulty level for this battlefield.
```


### MainMenuScene.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Scenes\MainMenuScene.cs

#### Line 104
```

            object v = InputRouter.GetMenuInput();
            var input = v;

            //TODO: Fix input property access - MenuInput may not have these properties
            //if (input.IsUpPressed)
            //{
            //    _selectedOption = (_selectedOption - 1 + _menuElements.Count) % _menuElements.Count;
            //    DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", $"MainMenuScene: Selected option {_selectedOption}");
```


### SnapshotCommands.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Snapshot\SnapshotCommands.cs

#### Line 106
```
                //Placeholder values — real game systems will supply these
                int waveNumber = 1;
                float gameTime = 0f;

                //TODO: Wire real player state once PlayerSystem API is confirmed.
                var snapshot = _snapshotManager.CreateSnapshot(
                    playerState: null,
                    waveNumber: waveNumber,
                    gameTime: gameTime,
```


### SnapshotIntegration.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Snapshot\SnapshotIntegration.cs

#### Line 115
```
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error", "[SNAPSHOT] Player system not available");
                return null;
            }

            //TODO: Get current wave number and game time from game systems
            int waveNumber = 1; //Placeholder
            float gameTime = 0f; //Placeholder

            var snapshot = _snapshotManager.CreateSnapshot(
```

#### Line 244
```
        //---------------------------------------------------------------------

        private static void RegisterCommands()
        {
            //TODO: Register with console command system
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Info", "[SNAPSHOT] Commands registered with console system");
        }
    }
}
```


### PlacementValidator.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Towers\PlacementValidator.cs

#### Line 265
```
        ///<param name="towerData">Tower data for the tower being placed.</param>
        ///<returns>True if too close to other towers.</returns>
        public bool IsTooCloseToOtherTowers(Vector3Int gridPosition, TowerData towerData)
        {
            //TODO: Implement tower registry when available
            //if (_towerRegistry == null)
            //    return false;
            return false;

```

#### Line 273
```

            var minDistance = GetMinDistanceFromTowers(towerData.Type);
            var worldPosition = _navigationGrid.GridToWorld(gridPosition);

            //TODO: Implement tower registry when available
            //foreach (var tower in _towerRegistry.GetAllTowers())
            foreach (var tower in new List<Tower>()) //Empty placeholder
            {
                var distance = Vector3.Distance(new Vector3(worldPosition.X, worldPosition.Y, 0), tower.Position);
```

#### Line 339
```
        ///</summary>
        private void Initialize()
        {
            _navigationGrid = NavigationGrid.Instance;
            TowerRegistry = null; //TODO: Implement TowerRegistry when available
            _isInitialized = true;
        }

        ///<summary>
```


### NeuralManager.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Towers\TowerControl\NeuralManager.cs

#### Line 534
```
            foreach (var kvp in _upgradeDatabases)
            {
                var towerType = kvp.Key;
                var database = kvp.Value;
                //TODO: Fix GetUpgradePath method - doesn't exist on upgrade database
                //var upgradePath = database.GetUpgradePath();
                var upgradePath = new List<TowerUpgrade>(); //Placeholder
                _upgradePaths[towerType] = upgradePath;
            }
```

#### Line 596
```
        ///Play success sound.
        ///</summary>
        private void PlaySuccessSound(string soundName)
        {
            //TODO: Implement ModernAudioSubsystem instance
            //ModernAudioSubsystem.PlaySound(soundName);
        }

        ///<summary>
```

#### Line 605
```
        ///Play error sound.
        ///</summary>
        private void PlayErrorSound(string soundName)
        {
            //TODO: Implement ModernAudioSubsystem instance
            //ModernAudioSubsystem.PlaySound(soundName);
        }

        private string GetDebuggerDisplay()
```


### NeuralNet.cs — 10 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Towers\TowerControl\NeuralNet.cs

#### Line 148
```
            Level = 1;
            Name = "Basic Upgrade";
            Description = "Basic tower upgrade";
            Cost = 100;
            TowerType = TowerType.Basic; //TODO: VickersTurret doesn't exist in enum
            UpgradeType = UpgradeType.Damage;

            DamageMultiplier = 1.0f;
            RangeMultiplier = 1.0f;
```

#### Line 228
```
                    CriticalChance = 0.15f;
                    break;

                case UpgradeType.Special:
                    //TODO: Complete upgrade type doesn't exist, using Special instead
                    //InitializeCompleteUpgrade();
                    break;

                }
```

#### Line 353
```
        public void ApplyToTower(Tower tower)
        {
            if (tower == null) return;

            //TODO: Fix tower method calls - these methods don't exist on Tower class
            //Apply stat modifiers
            //foreach (var modifier in _statModifiers)
            //{
            //    tower.SetStatModifier(modifier.Key, modifier.Value);
```

#### Line 360
```
            //{
            //    tower.SetStatModifier(modifier.Key, modifier.Value);
            //}

            //TODO: Fix tower method calls
            //Apply special abilities
            //foreach (var ability in _specialAbilities)
            //{
            //    tower.AddSpecialAbility(ability);
```

#### Line 367
```
            //{
            //    tower.AddSpecialAbility(ability);
            //}

            //TODO: Fix tower method calls
            //Apply visual effects
            //foreach (var effect in _visualEffects)
            //{
            //    tower.AddVisualEffect(effect);
```

#### Line 377
```

            //Update tower level
            tower.Level = Level;

            //TODO: Fix tower method calls
            //Update tower visual properties
            //if (UpgradeSprite != null)
            //{
            //    tower.SetSprite(UpgradeSprite);
```

#### Line 401
```
        public void RemoveFromTower(Tower tower)
        {
            if (tower == null) return;

            //TODO: Fix tower method calls
            //Remove stat modifiers
            //foreach (var modifier in _statModifiers)
            //{
            //    tower.RemoveStatModifier(modifier.Key);
```

#### Line 408
```
            //{
            //    tower.RemoveStatModifier(modifier.Key);
            //}

            //TODO: Fix tower method calls
            //Remove special abilities
            //foreach (var ability in _specialAbilities)
            //{
            //    tower.RemoveSpecialAbility(ability);
```

#### Line 415
```
            //{
            //    tower.RemoveSpecialAbility(ability);
            //}

            //TODO: Fix tower method calls
            //Remove visual effects
            //foreach (var effect in _visualEffects)
            //{
            //    tower.RemoveVisualEffect(effect);
```

#### Line 560
```
        ///Apply upgrade effects.
        ///</summary>
        private void ApplyUpgradeEffects()
        {
            //TODO: Fix ParticleSystem.CreateEffect - method doesn't exist
            //Apply visual effects
            //if (UpgradeEffect != null)
            //{
            //    ParticleSystem.Instance?.CreateEffect(UpgradeEffect);
```


### TowerManager.cs — 5 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Towers\TowerManager.cs

#### Line 236
```
            }

            try
            {
                //TODO: Fix GetEntityWithComponent call - ECSWorld doesn't have this method signature
                //var entity = _ecsWorld.GetEntityWithComponent<TowerComponent>(c => c.Tower.Id == towerId);
                //if (entity != null)
                //{
                //    _ecsWorld.DestroyEntity(entity.Id);
```

#### Line 247
```
                //Placeholder: Find tower by ID and destroy it
                var towerToDestroy = GetTower(towerId);
                if (towerToDestroy != null)
                {
                    //TODO: Destroy tower entity
                }

                //Mark grid as unoccupied
                var gridPos = _navigationGrid.WorldToGrid(tower.Position);
```

#### Line 380
```
        ///<param name="towerType">Type of tower being placed.</param>
        ///<returns>True if blocks enemy path.</returns>
        private bool BlocksEnemyPath(Vector3Int position, TowerType towerType)
        {
            //TODO: Implement path blocking check
            //This would require integration with the pathfinding system
            return false;
        }

```

#### Line 392
```
        ///<param name="towerType">Type of tower.</param>
        ///<returns>Minimum distance in world units.</returns>
        private float GetMinDistanceFromTowers(TowerType towerType)
        {
            //TODO: Implement tower-specific distance requirements
            return 2.0f; //Default minimum distance
        }

        ///
```

#### Line 428
```
        ///<param name="towerType">Type of tower.</param>
        ///<returns>Tower cost.</returns>
        public int GetTowerCost(TowerType towerType)
        {
            //TODO: Implement tower cost database
            return towerType switch
            {
                TowerType.Basic => 100,
                TowerType.Sniper => 200,
```


### Label.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Components\Label.cs

#### Line 209
```
            _wordWrap = false;
            _maxWidth = 0;

            InvalidateTextCache();
            //TODO: SizeF type not found - using Vector3 instead
            Size = new System.Drawing.SizeF(TextSize.X, TextSize.Y);

            DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"Label: Created '{Id}' with text '{_text}'");
        }
```

#### Line 248
```
        ///</summary>
        ///<param name="renderer">The renderer to use.</param>
        public virtual void Render(Renderer renderer)
        {
            //TODO: base.Render() doesn't take parameters
            //base.Render(renderer);

            if (string.IsNullOrEmpty(_text) || _font == null)
                return;
```

#### Line 264
```

        ///<param name="isClicked">Whether input was clicked this frame.</param>
        public virtual void HandleInput(Vector3 inputPosition, bool isClicked)
        {
            //TODO: base.HandleInput() doesn't take these parameters
            //base.HandleInput(inputPosition, isClicked);

            //Labels typically don't handle input unless specifically enabled
            //This can be overridden for interactive labels
```


### ModernCashDisplay.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\HUD\ModernCashDisplay.cs

#### Line 359
```
            if (_cashTextWidget != null)
            {
                var text = $"{_prefix}{string.Format(_format, _currentCash)}";
                _cashTextWidget.Text = text;
                //TODO: Cannot assign Engine.Core.Color to System.Drawing.Color
                //_cashTextWidget.Color = _currentColor;
            }
        }

```

#### Line 371
```
        private void UpdateWidgetPositions()
        {
            if (_backgroundPanel != null)
            {
                //TODO: PointF doesn't have a 2-argument constructor
                //_backgroundPanel.Position = new PointF(_position.X, _position.Y);
            }
            if (_cashTextWidget != null)
            {
```

#### Line 376
```
                //_backgroundPanel.Position = new PointF(_position.X, _position.Y);
            }
            if (_cashTextWidget != null)
            {
                //TODO: PointF doesn't have a 2-argument constructor
                //_cashTextWidget.Position = new PointF(_position.X, _position.Y);
            }
        }

```


### HUDTextElement.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\HUDTextElement.cs

#### Line 149
```
        ///          SetSystemColor(System.Drawing.Color.FromArgb(255, 128, 0))
        ///</summary>
        public void SetSystemColor(System.Drawing.Color color)
{
    //TODO: Cannot assign System.Drawing.Color to Engine.Core.Color
    //ManualTextColor = color;
}


```


### HUDRenderAdapter.cs — 6 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Rendering\HUDRenderAdapter.cs

#### Line 50
```

        public void RenderRectangle(RectangleF rect, Color color)
        {
            DLogger.Log($"LegacyRenderer: RenderRectangle → {rect} Color={color}");
            // TODO: hook into actual GPU backend; for now this is the contract.
        }

        public void RenderRectangle(RectangleF rect, Color color, string textureName)
        {
```

#### Line 56
```

        public void RenderRectangle(RectangleF rect, Color color, string textureName)
        {
            DLogger.Log($"LegacyRenderer: RenderRectangle (textured) → {rect} Color={color} Texture={textureName}");
            // TODO: use textureName to select texture in backend.
        }

        // --------------------------------------------------------------------
        // Text
```

#### Line 70
```
                return;

            Font font = _defaultFont;
            DLogger.Log($"LegacyRenderer: RenderText → \"{text}\" at {position} Color={color} Font={font.Name},{font.Size}");
            // TODO: actual text rendering via backend.
        }

        // --------------------------------------------------------------------
        // Clipping
```

#### Line 80
```

        public void SetClipRect(RectangleF rect)
        {
            DLogger.Log($"LegacyRenderer: SetClipRect → {rect}");
            // TODO: apply clip rect in backend.
        }

        public void ClearClipRect()
        {
```

#### Line 86
```

        public void ClearClipRect()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: ClearClipRect");
            // TODO: clear clip rect in backend.
        }

        // --------------------------------------------------------------------
        // Frame lifecycle
```

#### Line 96
```

        public void EndFrame()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: EndFrame");
            // TODO: submit any buffered commands if needed.
        }
    }
}

```


### StatisticsDisplay_Main.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Statistics\StatisticsDisplay_Main.cs

#### Line 160
```
        ///<param name="victoryStats">Victory statistics to display.</param>
        public void Initialize(VictoryStatistics victoryStats)
        {
            IsVisible = false;
            //TODO: Fix VictoryStatistics properties - TotalScore and TotalKills don't exist
            //Score = victoryStats?.TotalScore ?? 0;
            //Kills = victoryStats?.TotalKills ?? 0;
            Score = 0;
            Kills = 0;
```

#### Line 175
```
        ///<param name="gameStats">Game statistics to display.</param>
        public void Initialize(GameStatistics gameStats)
        {
            IsVisible = false;
            //TODO: Fix GameStatistics properties - TotalScore and TotalKills don't exist
            //Score = gameStats?.TotalScore ?? 0;
            //Kills = gameStats?.TotalKills ?? 0;
            Score = 0;
            Kills = 0;
```


### ScoreDisplaySystem.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Systems\ScoreDisplaySystem.cs

#### Line 187
```
        {
            foreach (var popup in _activePopups)
            {
                var popupColor = Color.FromArgb((int)(popup.Alpha * 255), (byte)TextColor.R, (byte)TextColor.G, (byte)TextColor.B);
                //TODO: Fix DrawText method signature
                //context.DrawText($"+{popup.Score}", popup.Position.X, popup.Position.Y, popupColor, FontSize - 4);
            }
        }

```


### UIManager.cs — 10 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Systems\UIManager.cs

#### Line 154
```
                DLogger.Log(LogSubsystems.UI, LogLevel.Error, $"UIManager: Failed to render - {ex.Message}");
            }
        }

        //TODO: Implement UIManager with proper UIElement interface
        public bool AddElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, IsEnabled, etc.)
            return false;
```

#### Line 157
```

        //TODO: Implement UIManager with proper UIElement interface
        public bool AddElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, IsEnabled, etc.)
            return false;
        }

        //TODO: Implement UIManager with proper UIElement interface
```

#### Line 161
```
            //TODO: Implement when UIElement has required properties (Id, IsEnabled, etc.)
            return false;
        }

        //TODO: Implement UIManager with proper UIElement interface
        public bool RemoveElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, etc.)
            return false;
```

#### Line 164
```

        //TODO: Implement UIManager with proper UIElement interface
        public bool RemoveElement(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (Id, etc.)
            return false;
        }

        public bool RemoveElement(string id) =>
```

#### Line 177
```

        public List<T> GetElements<T>() where T : UIElement =>
            _uiElements.OfType<T>().ToList();

        //TODO: Implement UIManager with proper UIElement interface
        public void SetFocus(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, SetFocus, etc.)
        }
```

#### Line 180
```

        //TODO: Implement UIManager with proper UIElement interface
        public void SetFocus(UIElement element)
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, SetFocus, etc.)
        }

        //TODO: Implement UIManager with proper UIElement interface
        public void ClearFocus()
```

#### Line 183
```
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, SetFocus, etc.)
        }

        //TODO: Implement UIManager with proper UIElement interface
        public void ClearFocus()
        {
            //TODO: Implement when UIElement has required properties (RemoveFocus, etc.)
        }
```

#### Line 186
```

        //TODO: Implement UIManager with proper UIElement interface
        public void ClearFocus()
        {
            //TODO: Implement when UIElement has required properties (RemoveFocus, etc.)
        }

        public void SetViewport(int width, int height)
        {
```

#### Line 195
```
            _viewportSize = new Vector3(width, height, 0);
            DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"UIManager: Set viewport to {width}x{height}");
        }

        //TODO: Implement UIManager with proper UIElement interface
        private void HandleInput()
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, Bounds, HandleInput, etc.)
        }
```

#### Line 198
```

        //TODO: Implement UIManager with proper UIElement interface
        private void HandleInput()
        {
            //TODO: Implement when UIElement has required properties (IsEnabled, Bounds, HandleInput, etc.)
        }

        public void ClearElements()
        {
```


### UISystem.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Systems\UISystem.cs

#### Line 360
```

            //Cleanup all panels
            foreach (var panel in _panels.Values)
            {
                //TODO: Implement Cleanup method for UIPanel
                //panel.Cleanup();
            }

            _panels.Clear();
```


### SpawnPattern.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\SpawnPattern.cs

#### Line 800
```

            if (spawnPoints.Count == 0)
                return positions;

            //TODO: Fix type mismatch - Bounds is Rect?, can't use ?? with Vector3
            //var bounds = parameters.Bounds ?? new Vector3(10f, 10f, 0f);
            var bounds = new Vector3(10f, 10f, 0f);
            var center = parameters.CenterPoint ?? spawnPoints[0];

```


### WaveDirector.cs — 6 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveDirector.cs

#### Line 527
```
        Enemy SpawnEnemy(SASZombieAssaultTD.Engine.Enemies.ZombieType zombieType)
        {
            try
            {
                //TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
                EnemyManager enemyManager = null; //EnemyManager.Instance;

                if (enemyManager == null)
                    return null;
```

#### Line 754
```
        ///Get enemies spawned in current wave.
        ///</summary>
        int GetEnemiesSpawnedInWave()
        {
            //TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
            EnemyManager enemyManager = null; //EnemyManager.Instance;

            if (enemyManager == null)
                return 0;
```

#### Line 766
```

            foreach (var enemy in allEnemies)
            {
                NotImplementedGuard.Hit("NOT_IMPLEMENTED");
                //TODO: check if this is correct
                //TODO: enemy.SourceWave is object type, need cast
                //if (enemy.SourceWave == _currentWaveNumber)
                //{
                //    count++;
```

#### Line 767
```
            foreach (var enemy in allEnemies)
            {
                NotImplementedGuard.Hit("NOT_IMPLEMENTED");
                //TODO: check if this is correct
                //TODO: enemy.SourceWave is object type, need cast
                //if (enemy.SourceWave == _currentWaveNumber)
                //{
                //    count++;
                //}
```

#### Line 782
```
        ///Get enemies defeated in current wave.
        ///</summary>
        int GetEnemiesDefeatedInWave()
        {
            //TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
            EnemyManager enemyManager = null; //EnemyManager.Instance;

            if (enemyManager == null)
                return 0;
```

#### Line 793
```
            var allEnemies = enemyManager.GetAllEnemies();

            foreach (var enemy in allEnemies)
            {
                //TODO: enemy.SourceWave is object type, need cast
                //if (enemy.SourceWave == _currentWaveNumber && !enemy.IsActive)
                //{
                //    count++;
                //}
```


### WaveDirectorAudioIntegration.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveDirectorAudioIntegration.cs

#### Line 102
```
            //Play special sound for champion enemies
            if (enemy.IsChampion)
            {
                ModernPlaySound.Play("success_level_up", 1.2f);
                //TODO: Enemy.ChampionLevel doesn't exist - need to add this property or use different approach
                DLogger.Log($"WaveDirectorAudioIntegration: Champion enemy spawned");
            }
        }

```


### WaveDirector_Notifications.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveManagement\WaveDirector_Notifications.cs

#### Line 130
```
            try
            {
                int bonus = 25 + (_currentWaveNumber * 5);

                //TODO: Wire to PlayerStats when available.
                //PlayerStats.Instance.AddCash(bonus);

                NotificationBanner.Show(
                    title: "Wave Bonus",
```


### WaveDirector_Spawning.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveManagement\WaveDirector_Spawning.cs

#### Line 102
```
                enemy.SourceWave = script.WaveNumber;

                ApplyWaveModifications_Internal(enemy, script);

                //TODO: Wire to EnemyManager when available.
                //EnemyManager.Instance.AddEnemy(enemy);

                InvokeEnemySpawned(enemy);
            }
```


### WaveDirector_Stats.cs — 1 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveManagement\WaveDirector_Stats.cs

#### Line 142
```
        ///Returns the number of alive enemies belonging to a specific wave.
        ///</summary>
        internal int GetAliveEnemiesForWave_Internal(int waveNumber)
        {
            //TODO: Wire to EnemyManager when available.
            EnemyManager enemyManager = null; //EnemyManager.Instance;

            if (enemyManager == null)
                return 0;
```


### WaveDirector_WaveFlow.cs — 2 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveManagement\WaveDirector_WaveFlow.cs

#### Line 221
```

            _upcomingWaves.Clear();
            InitializeWaveQueue_Internal();

            //TODO: Integrate with EnemyManager when available.
            //EnemyManager.Instance?.ClearAllEnemies();
        }

        //===============================================================================================
```

#### Line 311
```
        {
            if (_currentState != WaveState.InProgress)
                return true;

            //TODO: Wire to EnemyManager when available.
            EnemyManager enemyManager = null; //EnemyManager.Instance;

            if (enemyManager == null)
                return true;
```


### WaveSpawnGroup.cs — 3 TODOs
**Path:** E:\BDC\Projects\SASZombieAssaultTD\Engine\Waves\WaveSpawnGroup.cs

#### Line 194
```
        {
            var count = Count;

            //Apply difficulty-based count increase
            var difficulty = "Normal"; //TODO: Implement proper difficulty system
            var multiplier = difficulty switch
            {
                "Hard" => 1.2f,
                "Elite" => 1.5f,
```

#### Line 591
```

        private void ApplyChampionProperties(Enemy enemy)
        {
            enemy.IsChampion = true;
            //TODO: Add ChampionLevel property to Enemy class
            //enemy.ChampionLevel = ChampionLevel;

            //Champion bonuses
            var championBonus = 1f + (ChampionLevel * 0.2f);
```

#### Line 600
```
            enemy.MaxHealth = (int)(enemy.MaxHealth * championBonus);
            enemy.Damage = (int)(enemy.Damage * championBonus);

            //Visual champion effects
            //enemy.SetChampionVisuals(); //TODO: implement champion visuals
        }

        ///
    }
```


