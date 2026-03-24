## Type Resolution Analysis Report
Generated: 02/21/2026 21:25:31

### Type: EntityManager

Found definition(s):
- File: .\Engine\ECS\EntityManager.cs
  Namespace: SASZombieAssaultTD.Engine.ECS
- File: .\Engine\Managers\EntityManager.cs
  Namespace: SASZombieAssaultTD.Engine.Managers

Referenced in:
- File: .\Engine\ECS\ECSVerificationSuite.cs
  Line 22: private readonly EntityManager _entityManager;
  Line 37: _entityManager = new EntityManager(ecsWorld);
- File: .\Engine\ECS\EntityManager.cs
  Line 2: File:    EntityManager.cs
  Line 17: public sealed class EntityManager
  Line 22: /// Initializes a new EntityManager.
  Line 25: public EntityManager(ECSWorld ecsWorld)
  Line 28: DebugLogger.Log("INFO", "EntityManager: Initialized");
  Line 212: DebugLogger.Log("INFO", $"EntityManager: Destroyed {destroyedCount} dead entities");
  Line 237: DebugLogger.Log("INFO", $"EntityManager: Destroyed {destroyedCount} inactive entities");
  Line 262: DebugLogger.Log("INFO", $"EntityManager: Destroyed {destroyedCount} projectiles");
  Line 292: DebugLogger.Log("INFO", $"EntityManager: Respawned {respawnedCount} dead enemies");
  Line 330: var info = $"EntityManager Debug Info:\n";
- File: .\Engine\Managers\EntityManager.cs
  Line 3: File:    EntityManager.cs
  Line 16: public class EntityManager
- File: .\Engine\Scenes\BaseScene.cs
  Line 23: protected SASZombieAssaultTD.Engine.Managers.EntityManager? EntityManager => _gameRoot?.EntityManager;
- File: .\Engine\Scenes\TestScenes\MeanStreetsTest.cs
  Line 15: public EntityManager? EntityManager { get; set; }
  Line 29: if (EntityManager != null && SpawnPoints.Count > 0)
- File: .\Engine\Systems\AnimationSystem.cs
  Line 25: private readonly EntityManager _entityManager;
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 35: public AnimationSystem(EntityManager entityManager, EventBus eventBus)
  Line 37: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 106: // This is a limitation of the current EntityManager design
  Line 248: /// - AnimationSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\CollisionSystem.cs
  Line 119: private readonly SASZombieAssaultTD.Engine.Components.EntityManager _entityManager;
  Line 130: /// <param name="entityManager">Entity manager for component access</param>
  Line 132: public CollisionSystem(EntityManager entityManager, EventBus eventBus)
  Line 134: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 653: /// - CollisionSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\ParticleSystem.cs
  Line 84: private readonly EntityManager _entityManager;
  Line 97: /// <param name="entityManager">Entity manager for component access</param>
  Line 100: public ParticleSystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 102: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 533: // 2. Add it to the EntityManager
  Line 592: /// - ParticleSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\PhysicsSystem.cs
  Line 51: private readonly EntityManager _entityManager;
  Line 61: /// <param name="entityManager">Entity manager for component access</param>
  Line 63: public PhysicsSystem(EntityManager entityManager, EventBus eventBus)
  Line 65: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 355: /// - PhysicsSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\RenderingSystem.cs
  Line 43: /// P11-03-01-B: Constructor accepts AssetManager, EntityManager, EventBus, and platform renderer interface.
  Line 73: private readonly EntityManager _entityManager;
  Line 93: /// <param name="entityManager">Entity manager for accessing renderable entities</param>
  Line 100: EntityManager entityManager,
  Line 107: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 307: /// - RenderingSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\TriggerSystem.cs
  Line 28: private readonly EntityManager _entityManager;
  Line 39: /// <param name="entityManager">Entity manager for component access</param>
  Line 41: public TriggerSystem(EntityManager entityManager, EventBus eventBus)
  Line 43: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 417: /// - TriggerSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\UISystem.cs
  Line 34: private readonly EntityManager _entityManager;
  Line 71: /// <param name="entityManager">Entity manager for component access</param>
  Line 74: public UISystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 76: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 988: /// - UISystem queries EntityManager for entities with UIComponent
- File: .\Engine\Systems\WaveSystem.cs
  Line 1027: private readonly EntityManager _entityManager;
  Line 1330: public WaveSystem(EnemyManager enemyManager, TimingController timingController, EntityManager entityManager, HazardManager hazardManager)
  Line 1334: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Enemies\EnemySystem.cs
  Line 4: Purpose: Reference to EntityManager; AddEnemy, RemoveEnemy; enemy update loop in Update.
  Line 18: private readonly EntityManager _entityManager;
  Line 22: public EnemySystem(EntityManager entityManager, EventBus eventBus)
  Line 24: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\AnimationTriggerSystem.cs
  Line 25: private readonly EntityManager _entityManager;
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 35: public AnimationTriggerSystem(EntityManager entityManager, EventBus eventBus)
  Line 37: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\DamageSystem.cs
  Line 75: private readonly EntityManager _entityManager;
  Line 104: /// <param name="entityManager">Entity manager for component access</param>
  Line 106: public DamageSystem(EntityManager entityManager, EventBus eventBus)
  Line 108: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 331: // Remove entity from EntityManager
  Line 396: /// - Uses EntityManager to access ProjectileComponent, EnemyComponent, HealthComponent
- File: .\Engine\Systems\Gameplay\DeathEffectSystem.cs
  Line 28: private readonly EntityManager _entityManager;
  Line 41: /// <param name="entityManager">Entity manager for component access</param>
  Line 46: EntityManager entityManager,
  Line 51: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\DeathSystem.cs
  Line 27: private readonly EntityManager _entityManager;
  Line 45: /// <param name="entityManager">The entity manager.</param>
  Line 47: public DeathSystem(EntityManager entityManager, EventBus eventBus)
  Line 49: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\EntityRemovalSystem.cs
  Line 25: private readonly EntityManager _entityManager;
  Line 43: /// <param name="entityManager">The entity manager.</param>
  Line 45: public EntityRemovalSystem(EntityManager entityManager, EventBus eventBus)
  Line 47: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\GameOverSystem.cs
  Line 27: private readonly EntityManager _entityManager;
  Line 71: /// <param name="entityManager">Entity manager for component access</param>
  Line 73: public GameOverSystem(EntityManager entityManager, EventBus eventBus)
  Line 75: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\InventorySystem.cs
  Line 17: private readonly EntityManager _entityManager;
  Line 25: /// <param name="entityManager">Entity manager for component access</param>
  Line 28: public InventorySystem(EntityManager entityManager, EventManager eventManager, ItemDatabase itemDatabase)
  Line 30: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\KillAttributionSystem.cs
  Line 25: private readonly EntityManager _entityManager;
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 35: public KillAttributionSystem(EntityManager entityManager, EventBus eventBus)
  Line 37: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\PickupSystem.cs
  Line 74: private readonly EntityManager _entityManager;
  Line 103: /// <param name="entityManager">Entity manager for component access</param>
  Line 105: public PickupSystem(EntityManager entityManager, EventBus eventBus)
  Line 107: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 462: /// - Uses EntityManager to access PlayerComponent and PickupComponent
- File: .\Engine\Systems\Gameplay\ProjectileSystem.cs
  Line 17: private readonly EntityManager? _entityManager;
  Line 21: public ProjectileSystem(EntityManager? entityManager = null, EventBus? eventBus = null)
  Line 23: _entityManager = entityManager;
- File: .\Engine\Systems\Gameplay\ResourceSystem.cs
  Line 16: private readonly EntityManager _entityManager;
  Line 23: /// <param name="entityManager">Entity manager for component access</param>
  Line 25: public ResourceSystem(EntityManager entityManager, EventManager eventManager)
  Line 27: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 347: /// <param name="entityManager">Entity manager for validation</param>
  Line 350: public static ResourceSystem FromSaveData(ResourceSystemSaveData saveData, EntityManager entityManager, EventManager eventManager)
  Line 352: var system = new ResourceSystem(entityManager, eventManager);
- File: .\Engine\Systems\Gameplay\RespawnSystem.cs
  Line 27: private readonly EntityManager _entityManager;
  Line 51: /// <param name="entityManager">Entity manager for component access</param>
  Line 53: public RespawnSystem(EntityManager entityManager, EventBus eventBus)
  Line 55: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\RoundResetSystem.cs
  Line 27: private readonly EntityManager _entityManager;
  Line 66: /// <param name="entityManager">Entity manager for component access</param>
  Line 68: public RoundResetSystem(EntityManager entityManager, EventBus eventBus)
  Line 70: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\ScoreSystem.cs
  Line 24: private readonly EntityManager _entityManager;
  Line 47: /// <param name="entityManager">Entity manager for component access</param>
  Line 49: public ScoreSystem(EntityManager entityManager, EventBus eventBus)
  Line 51: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\StatsTrackerSystem.cs
  Line 25: private readonly EntityManager _entityManager;
  Line 36: /// <param name="entityManager">Entity manager for component access</param>
  Line 38: public StatsTrackerSystem(EntityManager entityManager, EventBus eventBus)
  Line 40: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\TowerSystem.cs
  Line 107: private readonly EntityManager _entityManager;
  Line 116: public TowerSystem(EntityManager entityManager, EventBus eventBus, AssetManager assetManager)
  Line 118: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Gameplay\ZoneTriggerSystem.cs
  Line 66: private readonly EntityManager _entityManager;
  Line 104: /// <param name="entityManager">Entity manager for component access</param>
  Line 106: public ZoneTriggerSystem(EntityManager entityManager, EventBus eventBus)
  Line 108: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 486: /// - Uses EntityManager to access ZoneComponent for zone identification
  Line 676: /// - Uses EntityManager to access ZoneComponent for zone identification
- File: .\Engine\Systems\Persistence\LoadSystem.cs
  Line 29: private readonly EntityManager _entityManager;
  Line 47: /// <param name="entityManager">Entity manager for component access</param>
  Line 49: public LoadSystem(EntityManager entityManager, EventBus eventBus)
  Line 51: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Persistence\SaveManager.cs
  Line 29: private readonly EntityManager _entityManager;
  Line 49: /// <param name="entityManager">Entity manager for component access</param>
  Line 52: public SaveManager(EntityManager entityManager, EventBus eventBus, string saveDirectory = null!)
  Line 54: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\Rendering\RenderSystem.cs
  Line 27: private readonly EntityManager _entityManager;
  Line 45: /// <param name="entityManager">The entity manager for accessing entities.</param>
  Line 46: public RenderSystem(EntityManager entityManager)
  Line 48: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
- File: .\Engine\Systems\UI\HealthBarRenderer.cs
  Line 30: private readonly EntityManager _entityManager;
  Line 76: /// <param name="entityManager">Entity manager for component access</param>
  Line 77: public HealthBarRenderer(EntityManager entityManager)
  Line 79: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 80: DebugLog("HealthBarRenderer: Initialized with EntityManager");
- File: .\Engine\Systems\UI\ScoreDisplaySystem.cs
  Line 31: private readonly EntityManager _entityManager;
  Line 89: /// <param name="entityManager">Entity manager for component access</param>
  Line 91: public ScoreDisplaySystem(EntityManager entityManager, EventBus eventBus)
  Line 93: _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
  Line 97: DebugLog("ScoreDisplaySystem: Initialized with EntityManager and EventBus");

---

### Type: DeathType

No definition found in project.

Referenced in:
- File: .\Engine\Components\StatsComponent.cs
  Line 60: /// Key: DeathType, Value: Number of kills of that type
  Line 62: public Dictionary<DeathType, int> KillByType { get; set; } = new Dictionary<DeathType, int>();
  Line 98: /// Initializes the KillByType dictionary with all DeathType values set to 0.
  Line 102: KillByType = new Dictionary<DeathType, int>
  Line 104: { DeathType.PlayerKill, 0 },
  Line 105: { DeathType.EnemyKill, 0 },
  Line 106: { DeathType.Environmental, 0 },
  Line 107: { DeathType.HealthDepletion, 0 },
  Line 108: { DeathType.Scripted, 0 },
  Line 109: { DeathType.Suicide, 0 },
  Line 110: { DeathType.Unknown, 0 }
  Line 118: /// <param name="deathType">The type of death for the kill</param>
  Line 119: public void AddKill(DeathType deathType)
  Line 131: if (KillByType.ContainsKey(deathType))
  Line 133: KillByType[deathType]++;
  Line 137: KillByType[deathType] = 1;
  Line 192: /// <param name="deathType">The death type to query</param>
  Line 194: public int GetKillsByType(DeathType deathType)
  Line 196: return KillByType.TryGetValue(deathType, out int count) ? count : 0;
  Line 269: if (Enum.TryParse<DeathType>(kvp.Key, out var deathType) && kvp.Value >= 0)
  Line 271: this.KillByType[deathType] = kvp.Value;
- File: .\Engine\Systems\Achievements\AchievementDefinition.cs
  Line 143: /// <param name="deathType">Optional death type filter</param>
  Line 145: public bool MatchesCriteria(AchievementRequirementType requirementType, string entityType = null!, string deathType = null!)
  Line 153: if (!string.IsNullOrEmpty(DeathTypeFilter) && DeathTypeFilter != deathType)
- File: .\Engine\Systems\Achievements\ChallengeDefinition.cs
  Line 152: /// <param name="deathType">Optional death type filter</param>
  Line 154: public bool MatchesCriteria(ChallengeRequirementType requirementType, string entityType = "", string deathType = "")
  Line 162: if (!string.IsNullOrEmpty(DeathTypeFilter) && DeathTypeFilter != deathType)
- File: .\Engine\Systems\Events\EntityDiedEvent.cs
  Line 66: public DeathType DeathType { get; set; }
  Line 83: $"Type={DeathType}";
  Line 90: public enum DeathType
- File: .\Engine\Systems\Events\KillAttributedEvent.cs
  Line 7: IsFriendlyFire, IsSelfInflicted, DeathType, and Timestamp. Event is serializable,
  Line 84: public DeathType DeathType { get; set; }
  Line 130: $"DeathType={DeathType}, " +
- File: .\Engine\Systems\Gameplay\AchievementSystem.cs
  Line 150: if (!definition.MatchesCriteria(definition.RequirementType, killEvent.VictimEntityType, killEvent.DeathType))
- File: .\Engine\Systems\Gameplay\AnimationTriggerSystem.cs
  Line 82: DebugLog($"AnimationTriggerSystem: Processing death for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 113: string animationName = GetDeathAnimationName(deathEvent.DeathType);
  Line 124: LogAnimationTrigger(deathEvent.EntityId, animationName, deathEvent.DeathType);
  Line 135: /// <param name="deathType">The type of death</param>
  Line 137: private string GetDeathAnimationName(DeathType deathType)
  Line 139: return deathType switch
  Line 141: DeathType.PlayerKill => "Death_PlayerKill",
  Line 142: DeathType.EnemyKill => "Death_EnemyKill",
  Line 143: DeathType.Environmental => "Death_Environmental",
  Line 144: DeathType.HealthDepletion => "Death_HealthDepletion",
  Line 145: DeathType.Scripted => "Death_Scripted",
  Line 146: DeathType.Suicide => "Death_Suicide",
  Line 156: /// <param name="deathType">The death type that triggered the animation</param>
  Line 157: private void LogAnimationTrigger(object entityId, string animationName, DeathType deathType)
  Line 159: string logMessage = $"ANIMATION_TRIGGER: Entity={entityId}, Animation={animationName}, DeathType={deathType}, Timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
- File: .\Engine\Systems\Gameplay\ChallengeSystem.cs
  Line 353: if (!definition.MatchesCriteria(definition.RequirementType, killEvent.VictimEntityType, killEvent.DeathType))
- File: .\Engine\Systems\Gameplay\DeathEffectSystem.cs
  Line 10: on DeathType or entity type with gameplay-agnostic design.
  Line 36: private readonly Dictionary<DeathType, DeathEffectConfiguration> _deathEffectConfigurations;
  Line 99: DebugLog($"DeathEffectSystem: Processing death effects for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 131: var effectConfig = GetDeathEffectConfiguration(deathEvent.DeathType);
  Line 140: LogEffectSpawn(deathEvent.EntityId, position, effectConfig, deathEvent.DeathType);
  Line 199: /// <param name="deathType">The death type to get configuration for</param>
  Line 201: private DeathEffectConfiguration GetDeathEffectConfiguration(DeathType deathType)
  Line 203: return _deathEffectConfigurations.TryGetValue(deathType, out var config)
  Line 205: : _deathEffectConfigurations[DeathType.Unknown];
  Line 213: _deathEffectConfigurations = new Dictionary<DeathType, DeathEffectConfiguration>
  Line 216: DeathType.PlayerKill,
  Line 224: DeathType.EnemyKill,
  Line 232: DeathType.Environmental,
  Line 240: DeathType.HealthDepletion,
  Line 248: DeathType.Scripted,
  Line 256: DeathType.Suicide,
  Line 264: DeathType.Unknown,
  Line 282: /// <param name="deathType">The death type that triggered the effects</param>
  Line 283: private void LogEffectSpawn(object entityId, PointF position, DeathEffectConfiguration effectConfig, DeathType deathType)
  Line 288: string logMessage = $"DEATH_EFFECTS: Entity={entityId}, Position={position}, DeathType={deathType}, Particles=[{particleEffects}], Sounds=[{soundEffects}], Timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
- File: .\Engine\Systems\Gameplay\KillAttributionSystem.cs
  Line 201: DeathType = deathEvent.DeathType,
- File: .\Engine\Systems\Gameplay\MetaProgressionSystem.cs
  Line 206: int xpAward = CalculateKillXP(killEvent.VictimEntityType, killEvent.DeathType);
  Line 247: /// <param name="deathType">Type of death</param>
  Line 249: private int CalculateKillXP(string entityType, string deathType)
  Line 262: var multiplier = deathType?.ToLowerInvariant() switch
- File: .\Engine\Systems\Gameplay\ScoreSystem.cs
  Line 8: supports score multipliers based on DeathType or entity type, tracks total and per-entity score.
  Line 34: private readonly Dictionary<DeathType, float> _deathTypeMultipliers;
  Line 97: DebugLog($"ScoreSystem: Processing score for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 130: float deathTypeMultiplier = GetDeathTypeMultiplier(deathEvent.DeathType);
  Line 177: /// <param name="deathType">The death type</param>
  Line 179: private float GetDeathTypeMultiplier(DeathType deathType)
  Line 181: return _deathTypeMultipliers.TryGetValue(deathType, out float multiplier)
  Line 278: _deathTypeMultipliers = new Dictionary<DeathType, float>
  Line 280: { DeathType.PlayerKill, 1.0f },      // Normal player kills
  Line 281: { DeathType.EnemyKill, 0.8f },       // Slightly less for enemy kills
  Line 282: { DeathType.Environmental, 0.5f },     // Half for environmental kills
  Line 283: { DeathType.HealthDepletion, 0.3f },   // Less for health depletion
  Line 284: { DeathType.Scripted, 1.0f },         // Full score for scripted kills
  Line 285: { DeathType.Suicide, 0.0f },          // No score for suicides
  Line 286: { DeathType.Unknown, 0.5f }           // Default for unknown deaths
  Line 311: $"DeathType={deathEvent.DeathType}, " +
- File: .\Engine\Systems\Gameplay\StatsTrackerSystem.cs
  Line 85: DebugLog($"StatsTrackerSystem: Processing stats for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 93: UpdateKillerStats(deathEvent.KillerEntityId, deathEvent.DeathType);
  Line 122: /// <param name="deathType">The type of death for categorization</param>
  Line 123: private void UpdateKillerStats(object killerEntityId, DeathType deathType)
  Line 128: killerStats.AddKill(deathType);
  Line 130: DebugLog($"StatsTrackerSystem: Updated kill stats for Entity {killerEntityId}, DeathType: {deathType}");
- File: .\Engine\Systems\Persistence\SaveManager.cs
  Line 659: if (Enum.TryParse<DeathType>(kvp.Key, out var deathType))
  Line 661: stats.KillByType[deathType] = kvp.Value;
- File: .\Engine\Systems\UI\KillFeedSystem.cs
  Line 125: DeathType = killEvent.DeathType,
  Line 131: DebugLog($"KillFeedSystem: Added kill entry - {killEvent.KillerName} killed {killEvent.VictimName} ({killEvent.DeathType})");
  Line 256: RenderDeathTypeIcon(entry.DeathType, currentX, yPosition, alpha, context);
  Line 263: private void RenderDeathTypeIcon(DeathType deathType, float x, float y, float alpha, IRenderContext context)
  Line 267: string iconText = GetDeathTypeIcon(deathType);
  Line 268: var iconColor = Color.FromArgb((int)(alpha * 255), GetDeathTypeColor(deathType));
  Line 281: private string GetDeathTypeIcon(DeathType deathType)
  Line 283: return deathType switch
  Line 285: DeathType.Bullet => "🔫",
  Line 286: DeathType.Explosion => "💥",
  Line 287: DeathType.Fire => "🔥",
  Line 288: DeathType.Melee => "⚔️",
  Line 289: DeathType.Poison => "☠️",
  Line 290: DeathType.Electric => "⚡",
  Line 291: DeathType.Fall => "📍",
  Line 292: DeathType.Drowning => "💧",
  Line 293: DeathType.Other => "💀",
  Line 301: private Color GetDeathTypeColor(DeathType deathType)
  Line 303: return deathType switch
  Line 305: DeathType.Bullet => Color.Yellow,
  Line 306: DeathType.Explosion => Color.Orange,
  Line 307: DeathType.Fire => Color.Red,
  Line 308: DeathType.Melee => Color.Gray,
  Line 309: DeathType.Poison => Color.Purple,
  Line 310: DeathType.Electric => Color.Cyan,
  Line 311: DeathType.Fall => Color.Brown,
  Line 312: DeathType.Drowning => Color.Blue,
  Line 313: DeathType.Other => Color.DarkGray,
  Line 374: public DeathType DeathType { get; set; }

---

### Type: PlayerStatsData

Found definition(s):
- File: .\Engine\Systems\Persistence\SaveGameData.cs
  Namespace: SASZombieAssaultTD.Engine.Persistence
- File: .\Engine\Systems\Player\PlayerStatsData.cs
  Namespace: SASZombieAssaultTD.Engine.Player

Referenced in:
- File: .\Engine\Components\StatsComponent.cs
  Line 213: /// <returns>PlayerStatsData containing current component state</returns>
  Line 214: public PlayerStatsData ToSaveData()
  Line 216: return new PlayerStatsData
  Line 234: public void FromSaveData(PlayerStatsData saveData)
- File: .\Engine\Systems\Persistence\SaveGameData.cs
  Line 43: public PlayerStatsData PlayerStats { get; set; } = new PlayerStatsData();
  Line 141: PlayerStats = new PlayerStatsData();
  Line 282: public class PlayerStatsData
  Line 320: /// Creates a new PlayerStatsData with default values.
  Line 322: public PlayerStatsData()
  Line 341: public PlayerStatsData Clone()
  Line 343: return new PlayerStatsData
- File: .\Engine\Systems\Persistence\SaveManager.cs
  Line 387: saveData.PlayerStats = new PlayerStatsData
- File: .\Engine\Systems\Player\PlayerStatsData.cs
  Line 3: public class PlayerStatsData { }

---

### Type: TransformComponent

Found definition(s):
- File: .\Engine\Components\TransformComponent.cs
  Namespace: SASZombieAssaultTD.Engine.Components
- File: .\Engine\ECS\EntityFactory.cs
  Namespace: SASZombieAssaultTD.Engine.ECS
- File: .\Engine\ECS\Components\TransformComponent.cs
  Namespace: SASZombieAssaultTD.Engine.ECS
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Namespace: SASZombieAssaultTD.Engine.Navigation

Referenced in:
- File: .\Engine\Components\TransformComponent.cs
  Line 2: File:    TransformComponent.cs
  Line 3: Path:    Engine/Components/TransformComponent.cs
  Line 35: public class TransformComponent
  Line 65: /// Creates a new TransformComponent with default values.
  Line 67: public TransformComponent()
  Line 72: /// Creates a new TransformComponent with specified position.
  Line 75: public TransformComponent(PointF position)
  Line 81: /// Creates a new TransformComponent with full configuration.
  Line 87: public TransformComponent(
  Line 104: return $"TransformComponent(Pos: {Position}, Rot: {Rotation:F2}, Scale: {Scale:F2})";
  Line 154: DebugLog("TransformComponent: FromSaveData called with null save data");
  Line 200: DebugLog($"TransformComponent: Restored transform state - Pos: {this.Position}, " +
  Line 205: DebugLog($"TransformComponent: Error restoring from save data - {ex.Message}");
  Line 215: Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] TransformComponent: {message}");
- File: .\Engine\ECS\ECSVerificationReport.cs
  Line 58: report.AppendLine("   ✓ TransformComponent - Position, rotation, scale, movement helpers");
  Line 85: report.AppendLine("   ✓ Legacy Entity base → TransformComponent");
  Line 170: entity.AddComponent(new TransformComponent());
  Line 182: var entities = world.GetEntitiesWith<TransformComponent>().ToList();
  Line 183: var renderables = world.GetEntitiesWith<TransformComponent, RenderableComponent>().ToList();
- File: .\Engine\ECS\ECSVerificationSuite.cs
  Line 149: var projectileTransform = projectile.GetComponent<TransformComponent>();
  Line 150: var enemyTransform = enemy.GetComponent<TransformComponent>();
  Line 215: var enemyTransform = enemy.GetComponent<TransformComponent>();
  Line 247: var enemy1Transform = enemy1.GetComponent<TransformComponent>();
  Line 248: var enemy2Transform = enemy2.GetComponent<TransformComponent>();
  Line 327: var projectileTransform = projectile.GetComponent<TransformComponent>();
  Line 328: var enemyTransform = enemy.GetComponent<TransformComponent>();
- File: .\Engine\ECS\EntityFactory.cs
  Line 41: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 77: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 104: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 126: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 268: public sealed class TransformComponent : BaseComponent
- File: .\Engine\ECS\EntityManager.cs
  Line 136: var transform = entity.GetComponent<TransformComponent>();
  Line 174: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
  Line 188: .Where(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position) <= maxRange)
  Line 189: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
- File: .\Engine\ECS\Components\NavAgentComponent.cs
  Line 349: // This would typically be called from the entity's TransformComponent
- File: .\Engine\ECS\Components\TransformComponent.cs
  Line 2: // FILE: Engine/ECS/Components/TransformComponent.cs
  Line 19: //     This is the authoritative TransformComponent definition.
  Line 30: public sealed class TransformComponent : BaseComponent
  Line 51: /// Creates a TransformComponent at the origin (0,0).
  Line 53: public TransformComponent()
  Line 60: /// Creates a TransformComponent at the specified coordinates.
  Line 62: public TransformComponent(float x, float y)
  Line 99: return $"TransformComponent (X={X}, Y={Y})";
- File: .\Engine\ECS\Systems\AISystem.cs
  Line 115: var transform = entity.GetComponent<TransformComponent>();
  Line 154: var transform = entity.GetComponent<TransformComponent>();
  Line 232: var transform = entity.GetComponent<TransformComponent>();
  Line 262: var transform = entity.GetComponent<TransformComponent>();
  Line 306: private void ExecuteWanderingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 320: private void ExecutePatrollingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 341: private void ExecuteAttackingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 354: private void PerformAttack(Entity entity, TransformComponent transform)
- File: .\Engine\ECS\Systems\AnimationSystem.cs
  Line 293: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
  Line 306: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
- File: .\Engine\ECS\Systems\CombatSystem.cs
  Line 139: var projectileTransform = projectile.GetComponent<TransformComponent>();
  Line 151: var targetTransform = target.GetComponent<TransformComponent>();
  Line 191: var transform = aoeEntity.GetComponent<TransformComponent>();
  Line 204: var distance = Vector2.Distance(transform.Position, target.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero);
  Line 315: var transform = entity.GetComponent<TransformComponent>();
  Line 336: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\ECS\Systems\NavigationSystem.cs
  Line 17: /// Queries entities with NavAgentComponent and TransformComponent, requests paths, and advances agents.
  Line 187: var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
  Line 192: var transform = entity.GetComponent<TransformComponent>();
  Line 286: var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
  Line 291: var transform = entity.GetComponent<TransformComponent>();
  Line 308: private void UpdateNavigationAgent(Entity entity, NavAgentComponent navAgent, TransformComponent transform, float deltaTime)
  Line 428: var transform = entity.GetComponent<TransformComponent>();
  Line 495: var affectedAgents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>()
  Line 523: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\ECS\Systems\RenderSystem.cs
  Line 15: /// P11-12-07: Simple system that renders entities with TransformComponent and RenderableComponent.
  Line 50: /// P11-12-07: Renders all entities with TransformComponent and RenderableComponent.
  Line 62: // Get all entities with both TransformComponent and RenderableComponent
  Line 63: var renderableEntities = _ecsWorld.GetEntitiesWith<TransformComponent, RenderableComponent>();
  Line 70: Transform = entity.GetComponent<TransformComponent>()!,
  Line 104: private void RenderEntity(IRenderContext context, Entity entity, TransformComponent transform, RenderableComponent renderable)
  Line 165: var entitiesWithTransform = _ecsWorld.GetEntitiesWith<TransformComponent>().Count();
  Line 167: var entitiesWithBoth = _ecsWorld.GetEntitiesWith<TransformComponent, RenderableComponent>().Count();
- File: .\Engine\ECS\Testing\ECSTestSuite.cs
  Line 113: e.AddComponent(new TransformComponent());
  Line 114: if (!e.HasComponent<TransformComponent>())
  Line 117: var t = e.GetComponent<TransformComponent>();
  Line 121: e.RemoveComponent<TransformComponent>();
  Line 122: return !e.HasComponent<TransformComponent>();
  Line 160: var t = new TransformComponent(5f, 10f);
- File: .\Engine\Entities\Enemy.cs
  Line 147: AddComponent(new TransformComponent());
  Line 220: var transform = GetComponent<TransformComponent>();
  Line 250: var transform = GetComponent<TransformComponent>();
  Line 251: var targetTransform = target.GetComponent<TransformComponent>();
  Line 317: var transform = GetComponent<TransformComponent>();
- File: .\Engine\Managers\EntityManager.cs
  Line 126: /// P11-03-02-B: Get all entities that have both SpriteComponent and TransformComponent.
  Line 136: if (HasComponent<SpriteComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 166: /// P11-03-04-C: Get all entities that have both ParticleEmitterComponent and TransformComponent.
  Line 176: if (HasComponent<ParticleEmitterComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 206: /// P11-04-01-C: Get all entities that have both CollisionComponent and TransformComponent.
  Line 216: if (HasComponent<CollisionComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 226: /// P11-04-02-D: Get all entities that have both PhysicsComponent and TransformComponent.
  Line 236: if (HasComponent<PhysicsComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 246: /// P11-04-03-E: Get all entities that have both TriggerComponent and TransformComponent.
  Line 256: if (HasComponent<TriggerComponent>(entity) && HasComponent<TransformComponent>(entity))
- File: .\Engine\Navigation\NavigationDebugRenderer.cs
  Line 219: var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
  Line 224: var transform = entity.GetComponent<TransformComponent>();
  Line 260: var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
  Line 265: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Line 47: var transform = entity.GetComponent<TransformComponent>();
  Line 53: DebugLogger.Log("MIGRATION", "Entity missing TransformComponent.");
  Line 136: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 212: if (!entity.HasComponent<TransformComponent>())
  Line 214: result.AddIssue(entity.Id, "Missing TransformComponent.");
  Line 322: public sealed class TransformComponent : BaseComponent
- File: .\Engine\Navigation\NavigationVerificationSuite.cs
  Line 160: var transform = agent.GetComponent<TransformComponent>();
  Line 332: var transform = agent.GetComponent<TransformComponent>();
  Line 428: legacyEntity.AddComponent(new TransformComponent(new Vector2(50, 50));
  Line 477: entity.AddComponent(new TransformComponent(position));
  Line 492: entity.AddComponent(new TransformComponent(position));
  Line 509: entity.AddComponent(new TransformComponent(position));
- File: .\Engine\Physics\ColliderComponent.cs
  Line 143: var transform = Owner?.GetComponent<TransformComponent>();
  Line 168: var transform = Owner?.GetComponent<TransformComponent>();
  Line 280: var transform = Owner?.GetComponent<TransformComponent>();
- File: .\Engine\Physics\CollisionDebugRenderer.cs
  Line 215: var collidableEntities = _ecsWorld.GetEntitiesWith<ColliderComponent, TransformComponent>();
  Line 220: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\Physics\CollisionVerificationSuite.cs
  Line 148: var projectileTransform = projectile.GetComponent<TransformComponent>();
  Line 149: var enemyTransform = enemy.GetComponent<TransformComponent>();
  Line 531: entity.AddComponent(new TransformComponent(position));
  Line 557: entity.AddComponent(new TransformComponent(position));
  Line 583: entity.AddComponent(new TransformComponent(position));
  Line 607: entity.AddComponent(new TransformComponent(position));
- File: .\Engine\Systems\CollisionSystem.cs
  Line 112: /// P11-04-01-D: Iterates over entities with CollisionComponent and TransformComponent,
  Line 164: /// - Iterates over all entities with CollisionComponent and TransformComponent
  Line 240: var transformA = _entityManager.GetComponent<TransformComponent>(entityA);
  Line 241: var transformB = _entityManager.GetComponent<TransformComponent>(entityB);
  Line 382: var transformA = _entityManager.GetComponent<TransformComponent>(collision.EntityA);
  Line 383: var transformB = _entityManager.GetComponent<TransformComponent>(collision.EntityB);
  Line 400: private void ResolveOverlap(CollisionResult collision, SASZombieAssaultTD.Engine.Components.TransformComponent physicsA, PhysicsComponent? physicsB, SASZombieAssaultTD.Engine.Components.TransformComponent transformA, SASZombieAssaultTD.Engine.Components.TransformComponent transformB)
  Line 654: /// - Component data (CollisionComponent + TransformComponent) determines all collision behavior
- File: .\Engine\Systems\ParticleSystem.cs
  Line 190: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 213: private void SpawnParticle(ParticleEmitterComponent emitter, TransformComponent transform)
  Line 254: /// Gets all entities that have both ParticleEmitterComponent and TransformComponent.
  Line 264: _entityManager.HasComponent<TransformComponent>(entity))
  Line 307: var particleTransform = new TransformComponent(
  Line 524: var transform = new TransformComponent(
  Line 549: private void SpawnParticleFromEffect(ParticleEmitterComponent emitter, TransformComponent transform)
- File: .\Engine\Systems\PhysicsSystem.cs
  Line 44: /// P11-04-02-B: Iterates over entities with PhysicsComponent and TransformComponent,
  Line 95: /// - Iterates over all entities with PhysicsComponent and TransformComponent
  Line 142: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 179: private void UpdateKinematicEntity(SASZombieAssaultTD.Engine.Components.PhysicsComponent physics, SASZombieAssaultTD.Engine.Components.TransformComponent transform, float deltaTime)
  Line 249: private void ApplyVelocityToPosition(PhysicsComponent physics, TransformComponent transform, float deltaTime)
  Line 356: /// - Component data (PhysicsComponent + TransformComponent) determines all physics behavior
- File: .\Engine\Systems\RenderingSystem.cs
  Line 44: /// P11-03-02-D: Now depends on SpriteComponent + TransformComponent for rendering.
  Line 50: /// - TransformComponent: Stores position, rotation, scale, and origin for spatial transforms
  Line 143: /// - Queries all entities that have both SpriteComponent and TransformComponent
  Line 161: // P11-03-02-C: Query all entities with both SpriteComponent and TransformComponent
  Line 195: var transformComponent = _entityManager.GetComponent<TransformComponent>(entity);
  Line 197: if (spriteComponent == null || transformComponent == null)
  Line 219: transformComponent.Position.X,
  Line 220: transformComponent.Position.Y
  Line 308: /// - Component data (SpriteComponent + TransformComponent) determines all rendering behavior
- File: .\Engine\Systems\TriggerSystem.cs
  Line 21: /// P11-04-03-C: Iterates over entities with TriggerComponent and TransformComponent,
  Line 22: /// checks for proximity to other entities with CollisionComponent and TransformComponent,
  Line 73: /// - Iterates over all entities with TriggerComponent and TransformComponent
  Line 74: /// - Checks for proximity to other entities with CollisionComponent and TransformComponent
  Line 128: var triggerTransform = _entityManager.GetComponent<TransformComponent>(triggerEntity);
  Line 183: var triggerTransform = _entityManager.GetComponent<TransformComponent>(triggerEntity);
  Line 185: var targetTransform = _entityManager.GetComponent<TransformComponent>(targetEntity);
  Line 418: /// - Component data (TriggerComponent + TransformComponent) determines all trigger behavior
  Line 429: /// - Iterates over all trigger entities with TriggerComponent and TransformComponent
  Line 430: /// - Checks proximity to target entities with CollisionComponent and TransformComponent
- File: .\Engine\Systems\Gameplay\DeathEffectSystem.cs
  Line 101: // Get the entity's position from TransformComponent
  Line 102: var transformComponent = _entityManager.GetComponent<TransformComponent>(deathEvent.EntityId);
  Line 103: if (transformComponent == null)
  Line 105: DebugLog($"DeathEffectSystem: Entity {deathEvent.EntityId} has no TransformComponent - using default position");
  Line 111: SpawnDeathEffectsAtPosition(transformComponent.Position, deathEvent);
  Line 113: DebugLog($"DeathEffectSystem: Death effects spawned for Entity {deathEvent.EntityId} at position {transformComponent.Position}");
- File: .\Engine\Systems\Gameplay\RespawnSystem.cs
  Line 8: Publishes EntityRespawnedEvent, resets HealthComponent, TransformComponent,
  Line 199: // Reset TransformComponent
  Line 200: if (_entityManager.HasComponent<TransformComponent>(entityId))
  Line 202: var transformComp = _entityManager.GetComponent<TransformComponent>(entityId);
  Line 259: if (_entityManager.HasComponent<TransformComponent>(entityId))
  Line 261: var transformComp = _entityManager.GetComponent<TransformComponent>(entityId);
- File: .\Engine\Systems\Persistence\LoadSystem.cs
  Line 252: var transform = _entityManager.GetComponent<TransformComponent>(playerEntity);
  Line 264: DebugLog("LoadSystem: Warning - Player entity missing TransformComponent");
  Line 638: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 651: var newTransform = new TransformComponent(entityData.Position, entityData.Rotation, entityData.Scale);
  Line 653: DebugLog($"LoadSystem: Created TransformComponent for entity {entityData.EntityId}");
- File: .\Engine\Systems\Persistence\SaveManager.cs
  Line 368: var transform = _entityManager.GetComponent<TransformComponent>(playerEntity);
  Line 535: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 630: var transform = _entityManager.GetComponent<TransformComponent>(playerEntity);
- File: .\Engine\Systems\Rendering\RenderSystem.cs
  Line 162: var transformComponent = entity.GetComponent<TransformComponent>();
  Line 163: if (transformComponent == null)
  Line 167: var position = transformComponent.Position;
- File: .\Engine\Systems\UI\HealthBarRenderer.cs
  Line 4: Purpose: Renders health bars above entities with HealthComponent and TransformComponent.
  Line 7: P11-04-09-B: Renders health bars above entities with HealthComponent and TransformComponent.
  Line 24: /// Renders health bars above entities that have HealthComponent and TransformComponent.
  Line 84: /// Renders health bars for all entities with HealthComponent and TransformComponent.
  Line 113: /// Gets all entities that have both HealthComponent and TransformComponent.
  Line 123: _entityManager.HasComponent<TransformComponent>(entity))
  Line 141: var transformComponent = _entityManager.GetComponent<TransformComponent>(entity);
  Line 143: if (healthComponent == null || transformComponent == null)
  Line 156: var entityPosition = transformComponent.Position;

---

### Type: ItemInstance

Found definition(s):
- File: .\Engine\Systems\Inventory\ItemInstance.cs
  Namespace: SASZombieAssaultTD.Engine.Inventory

Referenced in:
- File: .\Engine\Components\InventoryComponent.cs
  Line 74: /// <param name="itemInstance">Item instance to add</param>
  Line 77: public InventoryAddResult AddItem(ItemInstance itemInstance, ItemDatabase itemDatabase)
  Line 79: if (itemInstance == null)
  Line 85: var itemDef = itemDatabase.GetItemDefinition(itemInstance.ItemDefinitionId);
  Line 87: return new InventoryAddResult { Success = false, Reason = $"Item definition not found: {itemInstance.ItemDefinitionId}" };
  Line 90: var existingStack = Items.FirstOrDefault(item => item.CanMergeWith(itemInstance));
  Line 93: int remainingQuantity = existingStack.MergeFrom(itemInstance, itemDef.MaxStack);
  Line 97: return new InventoryAddResult { Success = true, AddedQuantity = itemInstance.Quantity };
  Line 104: var newStack = new ItemInstance(itemInstance.ItemDefinitionId, remainingQuantity, itemInstance.HasDurability)
  Line 106: Durability = itemInstance.Durability
  Line 113: AddedQuantity = itemInstance.Quantity,
  Line 120: itemInstance.SetQuantity(remainingQuantity);
  Line 125: AddedQuantity = itemInstance.Quantity - remainingQuantity,
  Line 140: int quantityToAdd = Math.Min(itemInstance.Quantity, itemDef.MaxStack);
  Line 141: int remainingQuantity = itemInstance.Quantity - quantityToAdd;
  Line 145: var newStack = new ItemInstance(itemInstance.ItemDefinitionId, quantityToAdd, itemInstance.HasDurability)
  Line 147: Durability = itemInstance.Durability
  Line 238: public List<ItemInstance> GetItems(string itemDefinitionId)
  Line 241: return new List<ItemInstance>();
  Line 251: public ItemInstance GetItemByInstanceId(string instanceId)
  Line 301: component.Items = saveData.Items.Select(ItemInstance.FromSaveData).ToList();
- File: .\Engine\Systems\Gameplay\InventorySystem.cs
  Line 102: var itemInstance = new ItemInstance(pickupEvent.ItemDefinitionId, pickupEvent.Quantity, pickupEvent.HasDurability);
  Line 105: itemInstance.SetDurability(pickupEvent.Durability);
  Line 113: itemInstance.SetCustomMetadata(kvp.Key, kvp.Value);
  Line 118: var addResult = inventory.AddItem(itemInstance, _itemDatabase);
  Line 239: ItemInstance itemToDrop = null;
  Line 273: itemToDrop = new ItemInstance(itemDefinitionId, removeResult.RemovedQuantity);
  Line 342: var transferItem = new ItemInstance(itemDefinitionId, removeResult.RemovedQuantity);
- File: .\Engine\Systems\Inventory\ItemInstance.cs
  Line 11: public class ItemInstance
  Line 64: public ItemInstance(string itemDefinitionId, int quantity = 1, bool hasDurability = false)
  Line 75: public ItemInstance() { }
  Line 153: public bool CanMergeWith(ItemInstance other)
  Line 167: public int MergeFrom(ItemInstance other, int maxStack)
  Line 186: public ItemInstance Split(int splitAmount)
  Line 191: var newInstance = new ItemInstance(ItemDefinitionId, splitAmount, HasDurability)
  Line 248: public static ItemInstance FromSaveData(ItemInstanceSaveData saveData)
  Line 250: var instance = new ItemInstance(saveData.ItemDefinitionId, saveData.Quantity, saveData.HasDurability)
  Line 267: /// Serializable save data structure for ItemInstance.

---

### Type: ItemDatabase

Found definition(s):
- File: .\Engine\Systems\Inventory\ItemDatabase.cs
  Namespace: SASZombieAssaultTD.Engine.Inventory

Referenced in:
- File: .\Engine\Components\InventoryComponent.cs
  Line 75: /// <param name="itemDatabase">Item database for stack limits</param>
  Line 77: public InventoryAddResult AddItem(ItemInstance itemInstance, ItemDatabase itemDatabase)
  Line 82: if (itemDatabase == null)
  Line 85: var itemDef = itemDatabase.GetItemDefinition(itemInstance.ItemDefinitionId);
- File: .\Engine\Systems\Gameplay\InventorySystem.cs
  Line 19: private readonly ItemDatabase _itemDatabase;
  Line 27: /// <param name="itemDatabase">Item database for item definitions</param>
  Line 28: public InventorySystem(EntityManager entityManager, EventManager eventManager, ItemDatabase itemDatabase)
  Line 32: _itemDatabase = itemDatabase ?? throw new ArgumentNullException(nameof(itemDatabase));
- File: .\Engine\Systems\Inventory\ItemDatabase.cs
  Line 14: public class ItemDatabase
  Line 28: public ItemDatabase(string dataFilePath = null!)
  Line 83: Console.WriteLine("[ItemDatabase] Cannot register null item definition");
  Line 89: Console.WriteLine("[ItemDatabase] Item definition must have a valid ID");
  Line 95: Console.WriteLine($"[ItemDatabase] Item with ID '{itemDefinition.Id}' already exists");
  Line 100: Console.WriteLine($"[ItemDatabase] Registered item: {itemDefinition.Id} - {itemDefinition.Name}");
  Line 113: Console.WriteLine("[ItemDatabase] Cannot update null item definition");
  Line 119: Console.WriteLine("[ItemDatabase] Item definition must have a valid ID");
  Line 125: Console.WriteLine($"[ItemDatabase] Item with ID '{itemDefinition.Id}' not found for update");
  Line 130: Console.WriteLine($"[ItemDatabase] Updated item: {itemDefinition.Id} - {itemDefinition.Name}");
  Line 143: Console.WriteLine("[ItemDatabase] Item ID cannot be null or empty");
  Line 149: Console.WriteLine($"[ItemDatabase] Removed item: {itemId}");
  Line 153: Console.WriteLine($"[ItemDatabase] Item with ID '{itemId}' not found for removal");
  Line 212: Console.WriteLine($"[ItemDatabase] Item data file not found: {path}");
  Line 231: Console.WriteLine($"[ItemDatabase] Loaded {_itemDefinitions.Count} items from {path}");
  Line 235: Console.WriteLine($"[ItemDatabase] No valid item data found in {path}");
  Line 240: Console.WriteLine($"[ItemDatabase] Error loading items from file: {ex.Message}");
  Line 274: Console.WriteLine($"[ItemDatabase] Saved {_itemDefinitions.Count} items to {path}");
  Line 279: Console.WriteLine($"[ItemDatabase] Error saving items to file: {ex.Message}");
  Line 291: Console.WriteLine($"[ItemDatabase] Cleared {count} item definitions");
  Line 388: Console.WriteLine($"[ItemDatabase] Loaded {_itemDefinitions.Count} default item definitions");
  Line 393: /// Serializable save data structure for ItemDatabase.

---

### Type: BlendParameters

Found definition(s):
- File: .\Engine\Animation\BlendTrees\BlendParameters.cs
  Namespace: Engine.Animation.BlendTrees

Referenced in:
- File: .\Engine\Animation\AnimationController.cs
  Line 30: private BlendParameters _currentBlendParameters;
  Line 52: public BlendParameters CurrentBlendParameters => _currentBlendParameters;
  Line 93: _currentBlendParameters = new BlendParameters();
- File: .\Engine\Animation\BlendTrees\BlendParameters.cs
  Line 9: public class BlendParameters
  Line 44: public BlendParameters()
  Line 55: public BlendParameters(BlendParameters other)
  Line 203: public BlendParameters Clone()
  Line 205: return new BlendParameters(this);
  Line 215: info.AppendLine("BlendParameters:");
- File: .\Engine\Animation\BlendTrees\BlendTree.cs
  Line 142: public BlendTreeValidationResult ValidateParameters(BlendParameters parameters)
  Line 177: public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
  Line 200: public string GetDebugInfo(BlendParameters parameters)
- File: .\Engine\Animation\BlendTrees\IBlendNode.cs
  Line 32: BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context);
  Line 39: string GetDebugInfo(BlendParameters parameters);
- File: .\Engine\Animation\BlendTrees\Nodes\LinearBlendNode.cs
  Line 73: public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
  Line 119: public string GetDebugInfo(BlendParameters parameters)
- File: .\Engine\Animation\BlendTrees\Nodes\SingleClipNode.cs
  Line 50: public BlendNodeResult Evaluate(BlendParameters parameters, BlendContext context)
  Line 61: public string GetDebugInfo(BlendParameters parameters)

---

### Type: AnimationEventDispatcher

Found definition(s):
- File: .\Engine\Animation\Events\AnimationEventDispatcher.cs
  Namespace: SASZombieAssaultTD.Engine.Animation.Events

Referenced in:
- File: .\Engine\Animation\AnimationController.cs
  Line 31: private readonly AnimationEventDispatcher _eventDispatcher;
  Line 53: public AnimationEventDispatcher EventDispatcher => _eventDispatcher;
  Line 94: _eventDispatcher = new AnimationEventDispatcher();
- File: .\Engine\Animation\Events\AnimationEventDispatcher.cs
  Line 12: public class AnimationEventDispatcher
  Line 54: public AnimationEventDispatcher()
  Line 62: DebugLogger.Log("DEBUG", "AnimationEventDispatcher: Initialized dispatcher");
  Line 74: DebugLogger.Log("ERROR", "AnimationEventDispatcher: Cannot register null receiver");
  Line 82: DebugLogger.Log("ERROR", $"AnimationEventDispatcher: Receiver '{receiver.ReceiverName}' validation failed: {string.Join(", ", validation.Errors)}");
  Line 90: DebugLogger.Log("ERROR", $"AnimationEventDispatcher: Receiver ID '{receiver.ReceiverId}' already registered");
  Line 101: DebugLogger.Log("DEBUG", $"AnimationEventDispatcher: Registered receiver '{receiver.ReceiverName}' ({receiver.ReceiverId}) with priority {receiver.Priority}");
  Line 114: DebugLogger.Log("ERROR", "AnimationEventDispatcher: Cannot deregister receiver with null or empty ID");
  Line 124: DebugLogger.Log("DEBUG", $"AnimationEventDispatcher: Deregistered receiver '{receiverToRemove.ReceiverName}' ({receiverId})");
  Line 129: DebugLogger.Log("WARNING", $"AnimationEventDispatcher: Receiver ID '{receiverId}' not found for deregistration");
  Line 142: DebugLogger.Log("ERROR", "AnimationEventDispatcher: Cannot register null event track");
  Line 150: DebugLogger.Log("ERROR", $"AnimationEventDispatcher: Event track '{eventTrack.TrackName}' validation failed: {string.Join(", ", validation.Errors)}");
  Line 157: DebugLogger.Log("ERROR", $"AnimationEventDispatcher: Event track for clip '{eventTrack.ClipId}' already registered");
  Line 165: DebugLogger.Log("DEBUG", $"AnimationEventDispatcher: Registered event track '{eventTrack.TrackName}' ({eventTrack.TrackId}) for clip '{eventTrack.ClipId}'");
  Line 178: DebugLogger.Log("ERROR", "AnimationEventDispatcher: Cannot deregister event track with null or empty clip ID");
  Line 186: DebugLogger.Log("DEBUG", $"AnimationEventDispatcher: Deregistered event track '{eventTrack.TrackName}' ({eventTrack.TrackId}) for clip '{clipId}'");
  Line 190: DebugLogger.Log("WARNING", $"AnimationEventDispatcher: Event track for clip '{clipId}' not found for deregistration");
  Line 245: DebugLogger.Log("DEBUG", $"AnimationEventDispatcher: Dispatched {dispatchedEvents.Count} events for clip '{currentClipId}' at {playbackTime:F3}s");
  Line 251: DebugLogger.Log("ERROR", $"AnimationEventDispatcher: Error during update: {ex.Message}");
  Line 319: DebugLogger.Log("DEBUG", "AnimationEventDispatcher: Reset all event tracks and statistics");
  Line 422: var info = $"AnimationEventDispatcher: Enabled={IsEnabled}";

---

### Type: IRenderContext

Found definition(s):
- File: .\Engine\Core\Interfaces\IGameStateMachine.cs
  Namespace: SASZombieAssaultTD.Engine.Core.Interfaces
- File: .\Engine\Rendering\IRenderContext.cs
  Namespace: SASZombieAssaultTD.Engine.Rendering

Referenced in:
- File: .\Engine\GameRoot.cs
  Line 55: private readonly IRenderContext _renderContext;
  Line 77: IRenderContext renderContext)
- File: .\Engine\Animation\AnimationDebugTools.cs
  Line 51: private readonly IRenderContext _renderContext;
  Line 89: IRenderContext renderContext,
- File: .\Engine\Core\IProgram.cs
  Line 35: /// Rendering is performed through the active IRenderContext.
- File: .\Engine\Core\Interfaces\IGameStateMachine.cs
  Line 76: void Render(IRenderContext renderContext);
  Line 152: void Render(IRenderContext renderContext);
  Line 164: public interface IRenderContext
- File: .\Engine\Core\Managers\RenderManager.cs
  Line 70: private IRenderContext? _currentContext;
  Line 164: public void RenderAll(IRenderContext context)
  Line 240: private void SetupRenderContext(IRenderContext context)
  Line 255: private void FinalizeRenderContext(IRenderContext context)
- File: .\Engine\ECS\Systems\AISystem.cs
  Line 104: public void Render(IRenderContext context)
- File: .\Engine\ECS\Systems\CollisionSystem.cs
  Line 118: public void Render(IRenderContext context)
- File: .\Engine\ECS\Systems\CombatSystem.cs
  Line 95: public void Render(IRenderContext context)
- File: .\Engine\ECS\Systems\ISystem.cs
  Line 24: void Render(IRenderContext context);
- File: .\Engine\ECS\Systems\NavigationSystem.cs
  Line 138: public void Render(IRenderContext context)
- File: .\Engine\ECS\Systems\RenderSystem.cs
  Line 53: public void Render(IRenderContext context)
  Line 104: private void RenderEntity(IRenderContext context, Entity entity, TransformComponent transform, RenderableComponent renderable)
- File: .\Engine\ECS\Systems\ScoringSystem.cs
  Line 125: public void Render(IRenderContext context)
- File: .\Engine\Entities\Enemy.cs
  Line 312: public override void Render(IRenderContext context)
- File: .\Engine\Entities\Entity.cs
  Line 19: public abstract void Render(IRenderContext context);
- File: .\Engine\Entities\Projectile.cs
  Line 33: public override void Render(IRenderContext context)
- File: .\Engine\Entities\Soldier.cs
  Line 29: public override void Render(IRenderContext context)
- File: .\Engine\Entities\Turret.cs
  Line 29: public override void Render(IRenderContext context)
- File: .\Engine\Managers\EntityManager.cs
  Line 275: public void RenderAll(IRenderContext context)
- File: .\Engine\Navigation\NavigationDebugRenderer.cs
  Line 133: public void Render(IRenderContext context)
  Line 180: private void RenderNavigationGrid(IRenderContext context)
  Line 217: private void RenderAgentPaths(IRenderContext context)
  Line 258: private void RenderAgentTargets(IRenderContext context)
  Line 297: private void RenderFlowFields(IRenderContext context)
  Line 321: private void RenderDebugStats(IRenderContext context)
- File: .\Engine\Physics\CollisionDebugRenderer.cs
  Line 129: public void Render(IRenderContext context)
  Line 164: private void RenderSpatialGrid(IRenderContext context)
  Line 213: private void RenderCollisionShapes(IRenderContext context)
  Line 257: private void RenderShape(IRenderContext context, CollisionShape shape, Vector2 position, bool isTrigger)
  Line 301: private void RenderCapsule(IRenderContext context, CapsuleShape capsule, Vector2 position, uint color)
  Line 340: private void RenderDebugStats(IRenderContext context)
  Line 375: private void RenderContacts(IRenderContext context, IEnumerable<ContactInfo> contacts)
- File: .\Engine\Rendering\DebugOverlay.cs
  Line 11: Uses IRenderContext for consistent text rendering.
  Line 27: private readonly IRenderContext _renderContext;
  Line 29: public DebugOverlay(FrameStats stats, IRenderContext renderContext)
- File: .\Engine\Rendering\Framebuffer.cs
  Line 9: Implements IRenderContext interface for compatibility with existing engine.
  Line 27: /// Implements IRenderContext for full compatibility with existing engine systems.
  Line 29: public sealed class Framebuffer : IRenderContext, IDisposable
  Line 89: /// Fills the entire framebuffer with a single color (IRenderContext).
  Line 135: // IRenderContext implementation - full compatibility
- File: .\Engine\Rendering\IRenderContext.cs
  Line 9: public interface IRenderContext
- File: .\Engine\Rendering\RenderInitValidator.cs
  Line 46: public bool ValidateInitialization(IRenderContext renderContext, RenderPipelineConfig config)
  Line 155: private void ValidateRenderContext(IRenderContext renderContext)
  Line 159: ComponentName = "IRenderContext",
  Line 166: result.Message = "IRenderContext is null";
- File: .\Engine\Rendering\RenderSurface.cs
  Line 35: public void Bind(IRenderContext context)
- File: .\Engine\Rendering\SpriteBatchRenderer.cs
  Line 14: private IRenderContext? _context;
  Line 23: public void Begin(IRenderContext context)
- File: .\Engine\Rendering\TextRenderer.cs
  Line 100: TextAlignment alignment = TextAlignment.Left, float? maxWidth = null, IRenderContext? context = null)
  Line 236: private void DrawTextWithContext(IRenderContext context, string text, Vector2 position, DrawingColor color, float scale)
- File: .\Engine\Rendering\WindowHost.cs
  Line 26: public IRenderContext CreateRenderContext()
  Line 29: // The engine's IRenderContext is implemented by Framebuffer.
- File: .\Engine\Scenes\BaseScene.cs
  Line 70: public virtual void OnRender(IRenderContext context) { }
  Line 94: public abstract void Render(IRenderContext context);
- File: .\Engine\Scenes\GameScene.cs
  Line 75: public override void OnRender(IRenderContext context)
  Line 95: public override void Render(IRenderContext context)
- File: .\Engine\Scenes\LoadingScene.cs
  Line 87: public override void OnRender(IRenderContext context)
  Line 114: public override void Render(IRenderContext context)
- File: .\Engine\Scenes\MainMenuScene.cs
  Line 78: public override void OnRender(IRenderContext context)
  Line 97: public override void Render(IRenderContext context)
- File: .\Engine\Scenes\PauseScene.cs
  Line 78: public override void OnRender(IRenderContext context)
  Line 98: public override void Render(IRenderContext context)
- File: .\Engine\Scenes\SceneManager.cs
  Line 137: public void Render(IRenderContext context)
- File: .\Engine\Scenes\TestScenes\MeanStreetsTest.cs
  Line 35: public void Render(IRenderContext context)
- File: .\Engine\Systems\DefaultPlatformRenderer.cs
  Line 45: IRenderContext context,
- File: .\Engine\Systems\RenderingSystem.cs
  Line 151: public void Render(IRenderContext context)
  Line 235: private void ExecuteRenderQueue(IRenderContext context)
  Line 254: private void DrawRenderItem(IRenderContext context, RenderItem item)
  Line 267: // Draw using IRenderContext
  Line 348: IRenderContext context,
- File: .\Engine\Systems\UISystem.cs
  Line 203: public void Render(IRenderContext context)
  Line 227: private void RenderUISubsystems(IRenderContext context)
  Line 312: private void RenderUIEntity(object entity, IRenderContext context)
- File: .\Engine\Systems\Rendering\RenderContext.cs
  Line 6: public class RenderContext : IRenderContext
- File: .\Engine\Systems\Rendering\RenderSystem.cs
  Line 28: private IRenderContext? _context;
  Line 35: public IRenderContext? Context => _context;
  Line 72: public void SetContext(IRenderContext context)
- File: .\Engine\Systems\UI\Button.cs
  Line 29: public override void Render(IRenderContext context)
- File: .\Engine\Systems\UI\HealthBarRenderer.cs
  Line 88: public void RenderHealthBars(IRenderContext context)
  Line 136: private void RenderEntityHealthBar(object entity, IRenderContext context)
- File: .\Engine\Systems\UI\HUD.cs
  Line 28: public void Render(IRenderContext context)
- File: .\Engine\Systems\UI\InventoryPanelRenderer.cs
  Line 154: public void Render(IRenderContext context)
- File: .\Engine\Systems\UI\ItemTooltipRenderer.cs
  Line 158: public void Render(IRenderContext context)
- File: .\Engine\Systems\UI\KillFeedSystem.cs
  Line 199: public void Render(IRenderContext context)
  Line 225: private void RenderKillEntry(KillFeedEntry entry, int index, IRenderContext context)
  Line 263: private void RenderDeathTypeIcon(DeathType deathType, float x, float y, float alpha, IRenderContext context)
- File: .\Engine\Systems\UI\LayoutSystem.cs
  Line 13: public void Render(UIElementBase element, RenderQueue queue, IRenderContext context)
  Line 31: public void Render(IRenderContext context)
  Line 43: public void Render(IRenderContext context, UIElementBase _, int __)
- File: .\Engine\Systems\UI\Menus.cs
  Line 28: public void Render(IRenderContext context)
- File: .\Engine\Systems\UI\Panel.cs
  Line 23: public override void Render(IRenderContext context)
- File: .\Engine\Systems\UI\ResourceDisplayRenderer.cs
  Line 129: public void Render(IRenderContext context)
- File: .\Engine\Systems\UI\ScoreDisplaySystem.cs
  Line 242: public void Render(IRenderContext context)
  Line 264: private void RenderScoreText(IRenderContext context)
  Line 274: private void RenderScorePopups(IRenderContext context)
- File: .\Engine\Systems\UI\UIElementBase.cs
  Line 190: public abstract void Render(IRenderContext context);

---

### Type: Entity

Found definition(s):
- File: .\Engine\ECS\Entity.cs
  Namespace: SASZombieAssaultTD.Engine.ECS
- File: .\Engine\ECS\EntityFactory.cs
  Namespace: SASZombieAssaultTD.Engine.ECS
- File: .\Engine\Entities\Entity.cs
  Namespace: SASZombieAssaultTD.Engine.Entities
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Namespace: SASZombieAssaultTD.Engine.Navigation
- File: .\Engine\Scene\Entity.cs
  Namespace: SASZombieAssaultTD.Engine.Scene

Referenced in:
- File: .\Engine\Animation\AnimationDebugTools.cs
  Line 242: /// Gets detailed animation state information for specified entity.
  Line 245: /// <param name="entityId">The entity ID to inspect.</param>
  Line 246: /// <returns>Detailed animation state information for the specified entity.</returns>
  Line 341: foreach (var entity in _animationController.ActiveEntities)
  Line 343: var skeleton = _animationController.GetEntitySkeleton(entity.Id);
  Line 366: foreach (var entity in _animationController.ActiveEntities)
  Line 368: var stateInfo = GetAnimationStateInfo(entity.Id);
  Line 372: var position = entity.Position + new Vector3(0, 2, 0);
  Line 522: /// Gets or sets the currently selected entity ID for detailed debugging.
  Line 523: /// When set, debug information focuses on this specific entity.
  Line 663: /// Comprehensive animation state information for entity inspection.
  Line 669: /// Gets or sets the entity ID this state information belongs to.
  Line 676: /// Represents the active animation state for the entity.
  Line 711: /// Gets or sets whether the entity is currently transitioning between states.
- File: .\Engine\Animation\AnimationStateInspector.cs
  Line 131: /// Gets detailed animation state information for specified entity.
  Line 133: /// <param name="entityId">The entity ID to inspect.</param>
  Line 177: DebugLogger.LogError($"Failed to get state info for entity {entityId}: {ex.Message}", ex);
  Line 184: /// Gets transition history for specified entity.
  Line 186: /// <param name="entityId">The entity ID to get history for.</param>
  Line 188: /// <returns>Transition history for the entity.</returns>
  Line 207: /// <returns>List of inspected entity IDs.</returns>
  Line 219: /// <returns>Dictionary of entity ID to state snapshot.</returns>
  Line 231: /// <param name="entityId">The entity ID that transitioned.</param>
  Line 269: /// Gets animation parameter information for specified entity.
  Line 271: /// <param name="entityId">The entity ID to inspect.</param>
  Line 299: DebugLogger.LogError($"Failed to get parameter info for entity {entityId}: {ex.Message}", ex);
  Line 306: /// Gets blend weight information for specified entity.
  Line 308: /// <param name="entityId">The entity ID to inspect.</param>
  Line 337: DebugLogger.LogError($"Failed to get blend weight info for entity {entityId}: {ex.Message}", ex);
  Line 402: DebugLogger.LogInfo($"  Average transitions per entity: {stats.AverageTransitionsPerEntity:F2}");
- File: .\Engine\Animation\AnimationTransitionDebug.cs
  Line 57: /// Gets the entity ID being debugged.
  Line 104: /// <param name="entityId">The entity ID.</param>
- File: .\Engine\Animation\BlendTrees\IBlendNode.cs
  Line 30: /// <param name="context">Evaluation context containing entity and time information</param>
  Line 87: /// Entity identifier for context-specific evaluation.
- File: .\Engine\Animation\Diagnostics\AnimationDiagnostics.cs
  Line 227: /// <param name="entityId">The entity ID that transitioned.</param>
  Line 255: /// <param name="entityId">The entity ID that performed the blend.</param>
  Line 346: /// Gets state tracker for a specific entity.
  Line 348: /// <param name="entityId">The entity ID.</param>
  Line 349: /// <returns>State tracker for the entity.</returns>
  Line 515: /// Animation state tracker for entity-specific animation tracking.
- File: .\Engine\Animation\Events\AnimationEventContext.cs
  Line 18: /// P11-19-08: Entity identifier for the event.
  Line 19: /// Deterministic entity tracking for event context.
  Line 69: /// <param name="entityId">Entity identifier</param>
  Line 96: var info = $"AnimationEventContext: Entity={EntityId}, Clip={ClipId}, Track={TrackId}, ";
  Line 123: // Validate entity ID
  Line 126: result.AddWarning("Entity ID is 0, may indicate uninitialized context");
- File: .\Engine\Animation\Events\AnimationEventDispatcher.cs
  Line 201: /// <param name="entityId">Entity identifier for context</param>
  Line 264: /// <param name="entityId">Entity identifier</param>
- File: .\Engine\Animation\Events\AnimationEventECSIntegration.cs
  Line 19: /// P11-19-12: Dictionary of ECS event handlers by entity ID.
  Line 20: /// Deterministic storage for entity-specific event handlers.
  Line 37: /// P11-19-12: Registers an ECS event handler for a specific entity.
  Line 40: /// <param name="entityId">Entity ID to register handler for</param>
  Line 47: DebugLogger.Log("ERROR", "AnimationEventECSIntegration: Cannot register handler for entity ID 0");
  Line 69: DebugLogger.Log("WARNING", $"AnimationEventECSIntegration: Handler '{handler.HandlerName}' is already registered for entity {entityId}");
  Line 74: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Registered ECS handler '{handler.HandlerName}' for entity {entityId}");
  Line 80: DebugLogger.Log("ERROR", $"AnimationEventECSIntegration: Error registering ECS handler for entity {entityId}: {ex.Message}");
  Line 86: /// P11-19-12: Deregisters an ECS event handler for a specific entity.
  Line 89: /// <param name="entityId">Entity ID to deregister handler for</param>
  Line 96: DebugLogger.Log("ERROR", "AnimationEventECSIntegration: Cannot deregister handler for entity ID 0");
  Line 112: DebugLogger.Log("WARNING", $"AnimationEventECSIntegration: No handlers registered for entity {entityId}");
  Line 119: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Deregistered ECS handler '{handler.HandlerName}' for entity {entityId}");
  Line 121: // Clean up empty entity handler lists
  Line 129: DebugLogger.Log("WARNING", $"AnimationEventECSIntegration: Handler '{handler.HandlerName}' not found for entity {entityId}");
  Line 137: DebugLogger.Log("ERROR", $"AnimationEventECSIntegration: Error deregistering ECS handler for entity {entityId}: {ex.Message}");
  Line 218: /// Deterministic event dispatching with entity and global handlers.
  Line 220: /// <param name="entityId">Entity ID the event is for</param>
  Line 238: // Dispatch to entity-specific handlers first
  Line 248: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Entity handler '{handler.HandlerName}' processed event '{animationEvent.EventName}' for entity {entityId}");
  Line 253: DebugLogger.Log("ERROR", $"AnimationEventECSIntegration: Entity handler '{handler.HandlerName}' failed to process event '{animationEvent.EventName}': {ex.Message}");
  Line 266: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Global handler '{handler.HandlerName}' processed event '{animationEvent.EventName}' for entity {entityId}");
  Line 278: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: No ECS handlers processed event '{animationEvent.EventName}' for entity {entityId}");
  Line 282: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Event '{animationEvent.EventName}' dispatched to {handlersProcessed} ECS handlers for entity {entityId}");
  Line 295: /// P11-19-12: Gets all registered handlers for an entity.
  Line 298: /// <param name="entityId">Entity ID to get handlers for</param>
  Line 304: DebugLogger.Log("WARNING", "AnimationEventECSIntegration: Cannot get handlers for entity ID 0");
  Line 324: DebugLogger.Log("ERROR", $"AnimationEventECSIntegration: Error getting handlers for entity {entityId}: {ex.Message}");
  Line 351: /// P11-19-12: Clears all handlers for a specific entity.
  Line 352: /// Deterministic cleanup for entity handlers.
  Line 354: /// <param name="entityId">Entity ID to clear handlers for</param>
  Line 360: DebugLogger.Log("ERROR", "AnimationEventECSIntegration: Cannot clear handlers for entity ID 0");
  Line 372: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: Cleared {count} ECS handlers for entity {entityId}");
  Line 377: DebugLogger.Log("DEBUG", $"AnimationEventECSIntegration: No handlers to clear for entity {entityId}");
  Line 384: DebugLogger.Log("ERROR", $"AnimationEventECSIntegration: Error clearing handlers for entity {entityId}: {ex.Message}");
  Line 464: /// P11-19-12: Handles an animation event for an entity.
  Line 467: /// <param name="entityId">Entity ID the event is for</param>
  Line 580: /// Deterministic count for entity handlers.
  Line 591: /// P11-19-12: Handler counts per entity.
  Line 592: /// Deterministic mapping of entity to handler count.
  Line 615: summary += "\n  Entity Handler Counts:";
  Line 618: summary += $"\n    Entity {kvp.Key}: {kvp.Value} handlers";
- File: .\Engine\Animation\Events\IAnimationEventReceiver.cs
  Line 72: /// Entity identifier for context-specific event handling.
  Line 114: /// <param name="entityId">Entity identifier</param>
  Line 170: return $"AnimationEventContext[Entity:{EntityId}, Clip:{ClipId}, Track:{TrackId}, Time:{PlaybackTime:F3}s, Delta:{DeltaTime:F3}s, Looping:{IsLooping}, Loop:{LoopCount}]";
- File: .\Engine\Animation\States\JumpState.cs
  Line 193: // Check if entity is moving when landing
- File: .\Engine\Animation\States\MoveState.cs
  Line 78: /// P11-17-04: Movement threshold for determining if entity is actually moving.
- File: .\Engine\Animation\Visualization\AnimationStateVisualization.cs
  Line 67: /// Gets the entity ID being visualized.
  Line 112: /// Gets whether the entity is currently transitioning.
  Line 139: /// <param name="entityId">The entity ID to visualize.</param>
- File: .\Engine\Components\CollisionComponent.cs
  Line 72: /// Offset position of the collider relative to the entity's transform position.
  Line 114: /// <param name="offset">Offset position relative to entity</param>
  Line 136: /// <param name="offset">Offset position relative to entity</param>
  Line 159: /// <param name="offset">Offset position relative to entity</param>
- File: .\Engine\Components\PhysicsComponent.cs
  Line 41: /// Current velocity of the entity in world units per second.
  Line 46: /// Current acceleration of the entity in world units per second squared.
  Line 51: /// Mass of the entity (affects force calculations and collision response).
  Line 69: /// Whether this entity is kinematic (true = not affected by forces, false = affected by forces).
  Line 75: /// Maximum speed the entity can travel (null = no speed limit).
  Line 95: /// <param name="mass">Mass of the entity</param>
  Line 117: /// <param name="mass">Mass of the entity</param>
  Line 120: /// <param name="isKinematic">Whether entity is kinematic</param>
  Line 144: /// Applies a force to this entity (changes acceleration based on mass).
  Line 160: /// Applies an impulse to this entity (instantaneous velocity change).
- File: .\Engine\Components\StatsComponent.cs
  Line 5: Purpose:   P11-04-08-D - Core ECS component for tracking entity statistics.
  Line 33: /// Component for tracking entity statistics.
  Line 39: /// Gets or sets the total number of kills this entity has scored.
  Line 44: /// Gets or sets the total number of times this entity has died.
  Line 54: /// Gets or sets the longest kill streak this entity has achieved.
  Line 65: /// Gets or sets the timestamp of the last kill this entity scored.
  Line 71: /// Gets or sets the timestamp of the last time this entity died.
  Line 115: /// Adds a kill to the entity's statistics.
  Line 144: /// Adds a death to the entity's statistics.
  Line 156: /// Used for new game sessions or entity respawns with fresh stats.
- File: .\Engine\Components\TransformComponent.cs
  Line 4: Purpose:   P11-05-01 - Core ECS component for entity transform properties.
  Line 32: /// Component for entity transform properties.
  Line 38: /// Position of the entity in world space.
  Line 43: /// Rotation of the entity in radians.
  Line 48: /// Scale of the entity (1.0 = original size).
  Line 59: /// Gets or sets the respawn position for the entity.
  Line 108: /// Resets the entity position to the specified location.
  Line 118: /// Sets the respawn position for the entity.
  Line 119: /// P11-04-10-G: Stores the location where entity should respawn.
- File: .\Engine\Components\TriggerComponent.cs
  Line 5: Features: Trigger flag, radius, one-time trigger flag, triggered entity tracking.
  Line 16: /// P11-04-03-B: Stores trigger flag, radius, one-time trigger flag, and triggered entity tracking.
  Line 22: /// Whether this entity generates trigger events.
  Line 23: /// True if this entity should detect and publish trigger events.
  Line 34: /// Whether this trigger fires only once per target entity.
  Line 42: /// Key: Entity reference, Value: Timestamp of first trigger (for potential cooldown logic).
  Line 59: /// Uses bitwise AND operation with target entity layers.
  Line 82: /// <param name="isTrigger">Whether this entity generates trigger events</param>
  Line 105: /// Checks if a specific entity has already triggered this component.
  Line 107: /// <param name="entity">Entity to check</param>
  Line 108: /// <returns>True if entity has already triggered this component</returns>
  Line 109: public bool HasEntityTriggered(object entity)
  Line 111: if (entity == null)
  Line 114: return TriggeredEntities.ContainsKey(entity);
  Line 118: /// Marks an entity as having triggered this component.
  Line 120: /// <param name="entity">Entity that triggered this component</param>
  Line 121: public void MarkEntityTriggered(object entity)
  Line 123: if (entity == null)
  Line 126: TriggeredEntities[entity] = DateTime.UtcNow;
  Line 130: /// Removes an entity from the triggered entities list.
  Line 131: /// Used when an entity exits the trigger area.
  Line 133: /// <param name="entity">Entity to remove</param>
  Line 134: public void RemoveEntityTriggered(object entity)
  Line 136: if (entity != null && TriggeredEntities.ContainsKey(entity))
  Line 138: TriggeredEntities.Remove(entity);
  Line 143: /// Checks if an entity can trigger this component based on cooldown and trigger-once settings.
  Line 145: /// <param name="entity">Entity to check</param>
  Line 146: /// <returns>True if entity can trigger this component</returns>
  Line 147: public bool CanEntityTrigger(object entity)
  Line 149: if (entity == null || !Enabled || !IsTrigger)
  Line 153: if (TriggerOnce && HasEntityTriggered(entity))
  Line 157: if (TriggerCooldown > 0.0f && HasEntityTriggered(entity))
  Line 159: var lastTriggerTime = TriggeredEntities[entity];
- File: .\Engine\ECS\BaseComponent.cs
  Line 8: //       - Holding a reference to the owning Entity
  Line 21: //         2. Entity
  Line 31: /// Provides lifecycle hooks and a reference to the owning entity.
  Line 36: // PUBLIC API — ENTITY REFERENCE
  Line 40: /// The entity that owns this component.
  Line 41: /// Set internally by Entity.AddComponent and cleared on removal.
  Line 43: public Entity Entity { get; internal set; }
  Line 50: /// Called when the component is attached to an entity.
  Line 62: /// Called when the component is removed or the entity is destroyed.
  Line 76: var entityId = Entity != null ? Entity.Id.ToString() : "None";
  Line 77: return $"{GetType().Name} (Entity={entityId})";
- File: .\Engine\ECS\ECSDebugInspector.cs
  Line 42: // Entity details (limited to first 10 for readability)
  Line 43: info.AppendLine("Entity Details (first 10):");
  Line 47: var entity = entities[i];
  Line 48: info.AppendLine($"  [{i + 1}] {GetEntityDebugInfo(entity)}");
  Line 60: /// Gets debug information about a specific entity.
  Line 62: /// <param name="entity">The entity to inspect.</param>
  Line 64: public static string GetEntityDebugInfo(Entity entity)
  Line 66: if (entity == null)
  Line 67: return "Entity is null";
  Line 70: info.Append($"Entity.{entity.Id}");
  Line 71: info.Append($" (Enabled: {entity.IsEnabled}, Destroyed: {entity.IsDestroyed})");
  Line 73: if (entity.Components.Count > 0)
  Line 76: foreach (var component in entity.Components)
  Line 93: /// <returns>Dictionary mapping component types to entity counts.</returns>
  Line 98: foreach (var entity in ecsWorld.Entities)
  Line 100: foreach (var component in entity.Components)
  Line 120: public static List<Entity> FindEntitiesWithComponents(ECSWorld ecsWorld, params Type[] componentTypes)
  Line 123: return new List<Entity>();
  Line 125: return ecsWorld.Entities.Where(entity =>
  Line 128: entity.Components.Any(c => c.GetType() == componentType));
  Line 158: // Check for duplicate entity IDs
  Line 164: result.AddError($"Duplicate entity ID found: {duplicateId}");
  Line 168: foreach (var entity in ecsWorld.Entities)
  Line 170: foreach (var component in entity.Components)
  Line 172: if (component.Owner != entity)
  Line 174: result.AddWarning($"Entity {entity.Id} has component {component.GetType().Name} with incorrect owner reference");
  Line 181: foreach (var entity in destroyedEntities)
  Line 183: result.AddWarning($"Destroyed entity {entity.Id} still exists in world");
  Line 189: result.AddWarning($"High entity count: {ecsWorld.EntityCount} (may impact performance)");
  Line 195: result.AddWarning($"High average components per entity: {avgComponentsPerEntity:F1} (may impact performance)");
  Line 220: report.AppendLine($"Entity Count: {entityCount}");
  Line 226: // Calculate average components per entity
  Line 228: report.AppendLine($"Avg Components/Entity: {avgComponents:F2}");
  Line 253: const int entityOverhead = 64; // Rough estimate per entity
  Line 258: // Entity overhead
  Line 262: foreach (var entity in ecsWorld.Entities)
  Line 264: total += entity.Components.Count * componentOverhead;
- File: .\Engine\ECS\ECSVerificationReport.cs
  Line 47: report.AppendLine("   ✓ Entity class with unique IDs and lifecycle management");
  Line 50: report.AppendLine("   ✓ ECSWorld central registry with entity management");
  Line 67: report.AppendLine("   ✓ RenderSystem for Transform+Renderable entity rendering");
  Line 68: report.AppendLine("   ✓ Proper entity querying and component access");
  Line 85: report.AppendLine("   ✓ Legacy Entity base → TransformComponent");
  Line 95: report.AppendLine("   ✓ Component statistics and entity queries");
  Line 101: report.AppendLine($"   ✓ Entity creation: {perfTest.CreationTime:F1}ms for 1000 entities");
  Line 102: report.AppendLine($"   ✓ Entity queries: {perfTest.QueryTime:F1}ms for 100 queries");
  Line 103: report.AppendLine($"   ✓ Entity updates: {perfTest.UpdateTime:F1}ms for 1000 entities");
  Line 109: report.AppendLine("   ✓ P11-12-01: Entity.cs and IEntityComponent.cs created");
  Line 111: report.AppendLine("   ✓ P11-12-03: ECSWorld.cs with entity lifecycle management");
  Line 116: report.AppendLine("   ✓ P11-12-08: Legacy entity logic migration started");
  Line 134: report.AppendLine("   2. Resolve namespace conflicts with legacy Entity system");
  Line 165: // Entity creation test
  Line 169: var entity = world.CreateEntity();
  Line 170: entity.AddComponent(new TransformComponent());
  Line 173: entity.AddComponent(new RenderableComponent($"test_{i}"));
- File: .\Engine\ECS\ECSVerificationSuite.cs
  Line 75: // Test 8: Entity Factory and Migration
  Line 76: allPassed &= RunTest("Entity Factory", TestEntityFactory);
  Line 298: throw new Exception($"Entity creation too slow: {creationTime}ms for 500 entities");
  Line 301: throw new Exception($"Entity update too slow: {updateTime}ms for 60 frames with 500 entities");
  Line 353: var entity = _ecsWorld.CreateEntity();
  Line 360: entity.AddComponent(health);
  Line 361: entity.AddComponent(movement);
  Line 362: entity.AddComponent(active);
  Line 365: if (entity.GetComponent<HealthComponent>() == null)
  Line 367: if (entity.GetComponent<MovementComponent>() == null)
  Line 369: if (entity.GetComponent<ActiveComponent>() == null)
  Line 377: var removed = entity.RemoveComponent<ActiveComponent>();
  Line 381: if (entity.GetComponent<ActiveComponent>() != null)
  Line 384: // Destroy entity
  Line 385: _ecsWorld.DestroyEntity(entity);
  Line 387: if (!entity.IsDestroyed)
  Line 388: throw new Exception("Entity not marked as destroyed");
  Line 394: /// Tests entity factory and migration functionality.
  Line 419: DebugLogger.Log("INFO", "Entity factory test passed");
- File: .\Engine\ECS\ECSWorld.cs
  Line 5: //     Core container and lifecycle manager for the Entity Component System (ECS).
  Line 9: //       - Maintaining internal entity/component collections
  Line 15: //     • No external dependencies beyond Entity and BaseComponent.
  Line 21: //     including Entity.cs, BaseComponent.cs, and all components, will be built
  Line 32: /// Responsible for entity lifecycle, component updates, and world‑level operations.
  Line 41: /// Incrementing ID counter for deterministic entity creation.
  Line 47: /// Key: Entity ID
  Line 48: /// Value: Entity instance
  Line 50: private readonly Dictionary<int, Entity> _entities =
  Line 51: new Dictionary<int, Entity>();
  Line 54: // PUBLIC API — ENTITY LIFECYCLE
  Line 58: /// Creates a new entity with a unique ID and registers it with the world.
  Line 60: public Entity CreateEntity()
  Line 62: var entity = new Entity(_nextEntityId++, this);
  Line 63: _entities[entity.Id] = entity;
  Line 64: return entity;
  Line 68: /// Safely destroys an entity. If the entity is already destroyed or
  Line 71: public void DestroyEntity(Entity entity)
  Line 73: if (entity == null)
  Line 76: if (!_entities.ContainsKey(entity.Id))
  Line 79: entity.DestroyInternal();
  Line 80: _entities.Remove(entity.Id);
  Line 94: var snapshot = new List<Entity>(_entities.Values);
  Line 96: foreach (var entity in snapshot)
  Line 98: if (entity.IsAlive)
  Line 99: entity.Update(deltaTime);
  Line 108: /// Returns true if an entity with the given ID exists and is alive.
- File: .\Engine\ECS\Entity.cs
  Line 2: // FILE: Engine/ECS/Entity.cs
  Line 5: //     Represents a single entity within the ECS architecture.
  Line 10: //       - Maintaining entity state (alive / destroyed)
  Line 20: //     This is the authoritative Entity definition. All components and systems
  Line 30: /// Represents a single entity in the ECS architecture.
  Line 33: public sealed class Entity
  Line 45: /// True if the entity is active and has not been destroyed.
  Line 50: /// Reference to the world that owns this entity.
  Line 69: internal Entity(int id, ECSWorld world)
  Line 80: /// Adds a component to the entity. If a component of the same type
  Line 94: existing.Entity = null;
  Line 97: component.Entity = this;
  Line 112: component.Entity = null;
  Line 118: /// Returns true if the entity contains a component of the specified type.
  Line 154: /// Called by ECSWorld when the entity is destroyed.
  Line 167: component.Entity = null;
  Line 178: /// Returns a human‑readable summary of the entity and its components.
  Line 182: return $"Entity {Id} (Alive={IsAlive}, Components={_components.Count})";
- File: .\Engine\ECS\EntityFactory.cs
  Line 31: /// Creates a fully configured enemy entity with all required components.
  Line 33: public static Entity CreateEnemy(ECSWorld world, EnemyType type, Vector2 position)
  Line 38: var entity = world.CreateEntity();
  Line 41: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 42: entity.AddComponent(new RenderableComponent { SpriteId = $"enemy_{type.ToString().ToLower()}" });
  Line 47: entity.AddComponent(new EnemyTypeComponent { Type = type });
  Line 48: entity.AddComponent(new HealthComponent
  Line 53: entity.AddComponent(new MovementComponent { Speed = stats.Speed });
  Line 54: entity.AddComponent(new ScoreComponent { ScoreValue = stats.Score });
  Line 55: entity.AddComponent(new ActiveComponent { IsActive = true });
  Line 59: return entity;
  Line 63: /// Creates a projectile entity with damage, movement, and lifetime.
  Line 65: public static Entity CreateProjectile(
  Line 75: var entity = world.CreateEntity();
  Line 77: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 78: entity.AddComponent(new RenderableComponent { SpriteId = "projectile" });
  Line 80: entity.AddComponent(new DamageComponent { Damage = damage });
  Line 81: entity.AddComponent(new MovementComponent { Speed = velocity.Length() });
  Line 82: entity.AddComponent(new ActiveComponent { IsActive = true });
  Line 84: entity.AddComponent(new LifetimeComponent
  Line 91: return entity;
  Line 95: /// Creates a player entity with fixed stats and active state.
  Line 97: public static Entity CreatePlayer(ECSWorld world, Vector2 position)
  Line 102: var entity = world.CreateEntity();
  Line 104: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 105: entity.AddComponent(new RenderableComponent { SpriteId = "player" });
  Line 107: entity.AddComponent(new HealthComponent { CurrentHealth = 100, MaxHealth = 100 });
  Line 108: entity.AddComponent(new MovementComponent { Speed = 2.0f });
  Line 109: entity.AddComponent(new ActiveComponent { IsActive = true });
  Line 113: return entity;
  Line 117: /// Creates a tower entity with configurable type and health.
  Line 119: public static Entity CreateTower(ECSWorld world, Vector2 position, string towerType = "Basic")
  Line 124: var entity = world.CreateEntity();
  Line 126: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 127: entity.AddComponent(new RenderableComponent { SpriteId = $"tower_{towerType.ToLower()}" });
  Line 129: entity.AddComponent(new HealthComponent { CurrentHealth = 200, MaxHealth = 200 });
  Line 130: entity.AddComponent(new ActiveComponent { IsActive = true });
  Line 134: return entity;
  Line 142: /// Migrates a legacy Enemy object into a modern ECS entity.
  Line 145: public static Entity MigrateLegacyEnemy(ECSWorld world, LegacyEnemy legacy)
  Line 154: var entity = CreateEnemy(world, ecsType, legacy.Position);
  Line 157: entity.GetComponent<HealthComponent>().CurrentHealth = legacy.Health;
  Line 158: entity.GetComponent<MovementComponent>().Speed = legacy.Speed;
  Line 159: entity.GetComponent<ScoreComponent>().ScoreValue = legacy.Score;
  Line 160: entity.GetComponent<ActiveComponent>().IsActive = legacy.IsActive;
  Line 164: return entity;
  Line 168: /// Migrates a legacy Projectile object into a modern ECS entity.
  Line 171: public static Entity MigrateLegacyProjectile(ECSWorld world, LegacyProjectile legacy)
  Line 179: var entity = CreateProjectile(world, legacy.Position, legacy.Velocity, legacy.Damage);
  Line 181: entity.GetComponent<ActiveComponent>().IsActive = legacy.IsActive;
  Line 185: return entity;
  Line 321: public Entity Entity { get; internal set; }
  Line 327: public sealed class Entity
  Line 335: internal Entity(int id, ECSWorld world)
  Line 347: comp.Entity = this;
  Line 362: private readonly Dictionary<int, Entity> _entities = new();
  Line 364: public Entity CreateEntity()
  Line 366: var e = new Entity(_nextId++, this);
- File: .\Engine\ECS\EntityManager.cs
  Line 3: Purpose: P11-13-09 - Entity management system replacing legacy managers.
  Line 14: /// P11-13-09: Entity management system that replaces EnemyManager and ProjectileManager.
  Line 15: /// Provides query methods and entity lifecycle management using ECS queries.
  Line 35: public IEnumerable<Entity> GetEnemies()
  Line 44: public IEnumerable<Entity> GetActiveEnemies()
  Line 47: .Where(entity => entity.GetComponent<ActiveComponent>()?.IsActive == true);
  Line 55: public IEnumerable<Entity> GetEnemiesByType(EnemyType enemyType)
  Line 58: .Where(entity => entity.GetComponent<EnemyTypeComponent>()?.EnemyType == enemyType);
  Line 65: public IEnumerable<Entity> GetProjectiles()
  Line 68: .Where(entity => !entity.HasComponent<EnemyTypeComponent>()); // Projectiles don't have EnemyTypeComponent
  Line 75: public IEnumerable<Entity> GetActiveProjectiles()
  Line 78: .Where(entity =>
  Line 80: var active = entity.GetComponent<ActiveComponent>();
  Line 81: return active?.IsActive == true && !entity.HasComponent<EnemyTypeComponent>();
  Line 89: public IEnumerable<Entity> GetDamageDealers()
  Line 98: public IEnumerable<Entity> GetDamageableEntities()
  Line 101: .Where(entity => !entity.GetComponent<HealthComponent>()!.IsDead);
  Line 108: public IEnumerable<Entity> GetDeadEntities()
  Line 111: .Where(entity => entity.GetComponent<HealthComponent>()!.IsDead);
  Line 118: public IEnumerable<Entity> GetInactiveEntities()
  Line 121: .Where(entity => !entity.GetComponent<ActiveComponent>()!.IsActive);
  Line 130: public IEnumerable<Entity> GetEntitiesInRadius(Vector2 position, float radius)
  Line 133: .Where(entity => entity.IsEnabled && !entity.IsDestroyed)
  Line 134: .Where(entity =>
  Line 136: var transform = entity.GetComponent<TransformComponent>();
  Line 147: public IEnumerable<Entity> GetEnemiesInRadius(Vector2 position, float radius)
  Line 150: .Where(entity => entity.HasComponent<EnemyTypeComponent>());
  Line 159: public IEnumerable<Entity> GetProjectilesInRadius(Vector2 position, float radius)
  Line 162: .Where(entity => entity.HasComponent<DamageComponent>() && !entity.HasComponent<EnemyTypeComponent>());
  Line 171: public Entity? GetNearestEnemy(Vector2 position, float maxRange = float.MaxValue)
  Line 174: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
  Line 185: public Entity? GetNearestEnemyOfType(Vector2 position, EnemyType enemyType, float maxRange = float.MaxValue)
  Line 188: .Where(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position) <= maxRange)
  Line 189: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
  Line 202: foreach (var entity in deadEntities)
  Line 204: if (_ecsWorld.DestroyEntity(entity))
  Line 227: foreach (var entity in inactiveEntities)
  Line 229: if (_ecsWorld.DestroyEntity(entity))
  Line 274: var deadEnemies = GetDeadEntities().Where(entity => entity.HasComponent<EnemyTypeComponent>()).ToList();
  Line 301: /// <returns>Entity statistics.</returns>
  Line 314: foreach (Entity enemy in GetEnemies())
  Line 324: /// Gets debug information about the entity manager.
- File: .\Engine\ECS\IEntityComponent.cs
  Line 10: /// P11-12-01: Base interface for all entity components in the ECS system.
  Line 15: /// Gets the entity this component is attached to.
  Line 18: Entity? Owner { get; }
  Line 27: /// P11-12-04: Called when the component is attached to an entity.
  Line 29: /// <param name="entity">The entity this component is being attached to.</param>
  Line 30: void OnAttach(Entity entity);
  Line 33: /// P11-12-04: Called when the component is detached from its entity.
- File: .\Engine\ECS\Components\ActiveComponent.cs
  Line 3: Purpose: P11-13-04 - Component for managing entity active state.
  Line 23: /// Gets or sets whether this entity is currently active.
  Line 33: /// Gets or sets whether this entity should automatically deactivate after its lifetime expires.
  Line 42: /// Gets or sets the total lifetime of this entity in seconds.
  Line 64: /// Event fired when the entity becomes active.
  Line 69: /// Event fired when the entity becomes inactive.
  Line 74: /// Event fired when the entity's lifetime expires.
  Line 80: /// Entity starts active by default.
  Line 93: /// <param name="initiallyActive">Whether the entity starts active.</param>
  Line 103: /// <param name="initiallyActive">Whether the entity starts active.</param>
  Line 128: DebugLogger.Log("INFO", $"ActiveComponent: Lifetime expired for entity {Owner?.Id}");
  Line 133: /// Activates this entity.
  Line 145: DebugLogger.Log("INFO", $"ActiveComponent: Activated entity {Owner?.Id}");
  Line 149: /// Deactivates this entity.
  Line 159: DebugLogger.Log("INFO", $"ActiveComponent: Deactivated entity {Owner?.Id}");
  Line 163: /// Toggles the active state of this entity.
  Line 191: DebugLogger.Log("INFO", $"ActiveComponent: Set lifetime={_lifetime:F1}s, auto-deactivate={_autoDeactivate} for entity {Owner?.Id}");
  Line 204: DebugLogger.Log("INFO", $"ActiveComponent: Extended lifetime by {additionalTime:F1}s for entity {Owner?.Id}");
  Line 216: DebugLogger.Log("INFO", $"ActiveComponent: Reset lifetime timer for entity {Owner?.Id}");
  Line 220: /// Checks if this entity should be processed by systems.
  Line 223: /// <returns>True if the entity is active and should be processed.</returns>
- File: .\Engine\ECS\Components\DamageComponent.cs
  Line 3: Purpose: P11-13-01 - Component for entity damage information.
  Line 105: /// Event fired when this damage is applied to a target entity.
  Line 107: public event Action<Entity, float>? OnDamageApplied;
  Line 184: /// <param name="targetEntity">The entity that received the damage.</param>
  Line 186: public void NotifyDamageApplied(Entity targetEntity, float actualDamage)
  Line 189: DebugLogger.Log("INFO", $"DamageComponent: Damage {actualDamage:F1} applied to entity {targetEntity?.Id}");
- File: .\Engine\ECS\Components\EnemyTypeComponent.cs
  Line 303: DebugLogger.Log("INFO", $"EnemyTypeComponent: Changed enemy type from {oldType} to {enemyType} for entity {Owner?.Id}");
  Line 313: DebugLogger.Log("INFO", $"EnemyTypeComponent: Added behavior flags {flags} to entity {Owner?.Id}");
  Line 323: DebugLogger.Log("INFO", $"EnemyTypeComponent: Removed behavior flags {flags} from entity {Owner?.Id}");
- File: .\Engine\ECS\Components\HealthComponent.cs
  Line 5: //     Represents the health state of an entity.
  Line 27: /// Component that stores health-related data for an entity.
  Line 36: /// Current health value of the entity.
  Line 42: /// Maximum health value of the entity.
  Line 52: /// True if the entity is considered alive (CurrentHealth &gt; 0).
  Line 57: /// True if the entity is at full health.
  Line 96: /// Applies damage to the entity, reducing CurrentHealth but never below 0.
  Line 107: /// Heals the entity, increasing CurrentHealth but never above MaxHealth.
  Line 134: /// Instantly kills the entity by setting CurrentHealth to 0.
- File: .\Engine\ECS\Components\MovementComponent.cs
  Line 5: //     Represents basic movement properties for an entity.
  Line 36: /// Movement speed of the entity in units per second.
- File: .\Engine\ECS\Components\NavAgentComponent.cs
  Line 230: /// <param name="currentPosition">Current world position of the entity.</param>
  Line 249: /// <param name="currentPosition">Current world position of the entity.</param>
  Line 349: // This would typically be called from the entity's TransformComponent
- File: .\Engine\ECS\Components\RenderableComponent.cs
  Line 5: //     Marks an entity as renderable and provides basic rendering metadata.
  Line 28: /// Component that marks an entity as renderable and provides basic
  Line 38: /// Identifier for the sprite or visual asset used to render the entity.
- File: .\Engine\ECS\Components\ScoreComponent.cs
  Line 3: Purpose: P11-13-02 - Component for entity scoring information.
  Line 23: /// Gets or sets the base score value awarded when this entity is defeated.
  Line 42: /// Gets whether the score for this entity has already been awarded.
  Line 43: /// Prevents duplicate scoring for the same entity.
  Line 63: /// Event fired when score is awarded for this entity.
  Line 65: public event Action<Entity, int>? OnScoreAwarded;
  Line 114: /// Awards the score for this entity and marks it as awarded.
  Line 115: /// Called by ScoringSystem when the entity is defeated.
  Line 117: /// <param name="scoringEntity">The entity that earned the score (player, tower, etc.).</param>
  Line 119: public int AwardScore(Entity scoringEntity)
  Line 130: DebugLogger.Log("INFO", $"ScoreComponent: Awarded {finalScore} score to entity {scoringEntity?.Id}");
  Line 142: DebugLogger.Log("INFO", $"ScoreComponent: Reset awarded state for entity {Owner?.Id}");
  Line 154: DebugLogger.Log("INFO", $"ScoreComponent: Applied combo multiplier {comboMultiplier} to entity {Owner?.Id}");
  Line 166: DebugLogger.Log("INFO", $"ScoreComponent: Cleared combo multiplier for entity {Owner?.Id}");
- File: .\Engine\ECS\Components\TransformComponent.cs
  Line 5: //     Represents the spatial position of an entity in the game world.
  Line 28: /// Component that stores the 2D position of an entity.
  Line 37: /// X coordinate of the entity in world space.
  Line 42: /// Y coordinate of the entity in world space.
  Line 73: /// Moves the entity by the specified delta values.
  Line 82: /// Sets the entity's position to the specified coordinates.
- File: .\Engine\ECS\Systems\AISystem.cs
  Line 45: public event Action<Entity, AIState, AIState>? OnAIStateChanged;
  Line 46: public event Action<Entity>? OnTargetReached;
  Line 81: foreach (var entity in enemyEntities)
  Line 83: if (ShouldProcessEntity(entity))
  Line 85: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 88: ProcessAINavigation(entity, navAgent, deltaTime);
  Line 90: ProcessAILegacy(entity, deltaTime);
  Line 112: private void ProcessAINavigation(Entity entity, NavAgentComponent navAgent, float deltaTime)
  Line 114: var enemyType = entity.GetComponent<EnemyTypeComponent>();
  Line 115: var transform = entity.GetComponent<TransformComponent>();
  Line 116: var health = entity.GetComponent<HealthComponent>();
  Line 128: var targetPosition = DetermineNavigationTarget(entity, enemyType, transform.Position);
  Line 133: DebugLogger.Log("DEBUG", $"AISystem: Entity {entity.Id} navigating to {targetPosition}");
  Line 142: OnTargetReached?.Invoke(entity);
  Line 143: DebugLogger.Log("DEBUG", $"AISystem: Entity {entity.Id} reached navigation target");
  Line 150: private void ProcessAILegacy(Entity entity, float deltaTime)
  Line 152: var enemyType = entity.GetComponent<EnemyTypeComponent>();
  Line 153: var movement = entity.GetComponent<MovementComponent>();
  Line 154: var transform = entity.GetComponent<TransformComponent>();
  Line 155: var health = entity.GetComponent<HealthComponent>();
  Line 163: var currentState = GetOrCreateAIState(entity.Id);
  Line 164: var newState = DetermineAIState(entity, currentState);
  Line 168: OnAIStateChanged?.Invoke(entity, currentState.CurrentState, newState);
  Line 172: ExecuteBehavior(entity, newState, deltaTime);
  Line 173: UpdateAIState(entity.Id, deltaTime);
  Line 179: private Vector2 DetermineNavigationTarget(Entity entity, EnemyTypeComponent enemyType, Vector2 currentPosition)
  Line 191: return GetNextPatrolPoint(entity, currentPosition);
  Line 199: private Vector2 GetNextPatrolPoint(Entity entity, Vector2 currentPosition)
  Line 201: var state = GetOrCreateAIState(entity.Id);
  Line 229: private AIState DetermineAIState(Entity entity, AIStateData currentState)
  Line 231: var enemyType = entity.GetComponent<EnemyTypeComponent>();
  Line 232: var transform = entity.GetComponent<TransformComponent>();
  Line 233: var health = entity.GetComponent<HealthComponent>();
  Line 259: private void ExecuteBehavior(Entity entity, AIState state, float deltaTime)
  Line 261: var movement = entity.GetComponent<MovementComponent>();
  Line 262: var transform = entity.GetComponent<TransformComponent>();
  Line 274: ExecuteWanderingBehavior(entity, movement, transform, deltaTime);
  Line 278: ExecutePatrollingBehavior(entity, movement, transform, deltaTime);
  Line 288: ExecuteAttackingBehavior(entity, movement, transform, deltaTime);
  Line 306: private void ExecuteWanderingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 308: var state = _entityStates[entity.Id];
  Line 320: private void ExecutePatrollingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 322: var state = _entityStates[entity.Id];
  Line 341: private void ExecuteAttackingBehavior(Entity entity, MovementComponent movement, TransformComponent transform, float deltaTime)
  Line 343: var state = _entityStates[entity.Id];
  Line 349: PerformAttack(entity, transform);
  Line 354: private void PerformAttack(Entity entity, TransformComponent transform)
  Line 357: entity.AddComponent(damageComponent);
  Line 359: DebugLogger.Log("INFO", $"AISystem: Entity {entity.Id} performed melee attack");
  Line 393: private bool ShouldProcessEntity(Entity entity)
  Line 395: if (!entity.IsEnabled || entity.IsDestroyed)
  Line 398: if (!entity.HasComponent<EnemyTypeComponent>())
  Line 413: DebugLogger.Log("INFO", $"AISystem: Cleaned up {removedKeys.Count} destroyed entity states");
  Line 451: private void HandleAICollision(Entity aiEntity, Entity otherEntity, CollisionEvent collisionEvent)
  Line 467: private void HandleTriggerCollision(Entity aiEntity, Entity triggerEntity)
  Line 473: DebugLogger.Log("DEBUG", $"AISystem: AI entity {aiEntity.Id} entered trigger zone {triggerEntity.Id}");
  Line 477: private void HandleObstacleCollision(Entity aiEntity, Entity obstacleEntity, CollisionEvent collisionEvent)
  Line 490: DebugLogger.Log("DEBUG", $"AISystem: AI entity {aiEntity.Id} collided with obstacle {obstacleEntity.Id}");
  Line 494: private bool IsAIEntity(Entity entity)
  Line 495: => entity.HasComponent<EnemyTypeComponent>();
  Line 497: private bool IsObstacle(Entity entity)
  Line 499: var collider = entity.GetComponent<ColliderComponent>();
  Line 508: DebugLogger.Log("DEBUG", $"AISystem: Entity {controller.Entity.Id} started animation '{clipName}'");
  Line 513: DebugLogger.Log("DEBUG", $"AISystem: Entity {controller.Entity.Id} completed animation '{clipName}'");
  Line 518: DebugLogger.Log("DEBUG", $"AISystem: Entity {controller.Entity.Id} fired animation event '{animationEvent.Name}'");
- File: .\Engine\ECS\Systems\AnimationSystem.cs
  Line 132: foreach (var entity in controllers)
  Line 134: var controller = entity.GetComponent<AnimationControllerComponent>();
  Line 166: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} started animation '{currentClip}'");
  Line 176: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} completed animation '{currentClip}'");
  Line 203: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} fired event '{animationEvent.Name}' at time {animationEvent.Time:F3}");
  Line 255: // Create damage component for the entity
  Line 257: controller.Entity.AddComponent(damageComponent);
  Line 259: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} fired weapon for {damage} damage");
  Line 271: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} footstep sound '{soundName}' at volume {volume}");
  Line 284: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} playing sound '{soundName}' (volume: {volume}, loop: {loop})");
  Line 293: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
  Line 296: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Entity.Id} spawning effect '{effectType}' at {position}");
  Line 306: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
  Line 310: controller.Entity.AddComponent(areaDamageComponent);
  Line 312: DebugLogger.Log("DEBUG", $"AnimationSystem: Entity {controller.Id} created area damage: {damage} damage, {radius} radius at {position}");
  Line 347: foreach (var entity in stateMachines)
  Line 349: var stateMachine = entity.GetComponent<AnimationStateMachine>();
  Line 368: HandleTransitionActions(entity, stateMachine, transition);
  Line 378: /// <param name="entity">The entity with the state machine.</param>
  Line 381: private void HandleTransitionActions(Entity entity, AnimationStateMachine stateMachine, AnimationTransition transition)
  Line 391: DebugLogger.Log("DEBUG", $"AnimationSystem: Set parameter '{paramName}' = '{paramValue}' for entity {entity.Id}");
  Line 398: OnAnimationEventFired?.Invoke(entity.GetComponent<AnimationControllerComponent>(), animationEvent);
  Line 399: DebugLogger.Log("DEBUG", $"AnimationSystem: Fired event '{eventName}' from transition for entity {entity.Id}");
  Line 404: var controller = entity.GetComponent<AnimationControllerComponent>();
  Line 408: DebugLogger.Log("DEBUG", $"AnimationSystem: Played clip '{clipName}' from transition for entity {entity.Id}");
  Line 426: foreach (var entity in renderableEntities)
  Line 428: var renderable = entity.GetComponent<RenderableComponent>();
  Line 429: var animationController = entity.GetComponent<AnimationControllerComponent>();
  Line 430: var stateMachine = entity.GetComponent<AnimationStateMachine>();
  Line 434: UpdateRenderableFromController(entity, renderable, animationController);
  Line 438: UpdateRenderableFromStateMachine(entity, renderable, stateMachine);
  Line 446: private void UpdateRenderableFromController(Entity entity, RenderableComponent renderable, AnimationControllerComponent controller)
  Line 480: private void UpdateRenderableFromStateMachine(Entity entity, RenderableComponent renderable, AnimationStateMachine stateMachine)
- File: .\Engine\ECS\Systems\CollisionSystem.cs
  Line 133: private IEnumerable<(Entity, Entity)> GetPotentialCollisionPairs()
  Line 142: /// <param name="entityA">The first entity.</param>
  Line 143: /// <param name="entityB">The second entity.</param>
  Line 145: private bool ShouldTestCollision(Entity entityA, Entity entityB)
  Line 165: private List<CollisionEvent> ProcessNarrowPhaseCollisions(IEnumerable<(Entity, Entity)> potentialPairs)
  Line 186: /// <param name="entityA">The first entity.</param>
  Line 187: /// <param name="entityB">The second entity.</param>
  Line 189: private CollisionEvent? TestCollision(Entity entityA, Entity entityB)
- File: .\Engine\ECS\Systems\CombatSystem.cs
  Line 39: public event Action<Entity, Entity, float, DamageType>? OnDamageApplied;
  Line 42: /// Event fired when an entity is killed by combat.
  Line 44: public event Action<Entity, Entity>? OnEntityKilled;
  Line 135: .Where(entity => entity.GetComponent<ActiveComponent>()?.IsActive == true);
  Line 186: .Where(entity => HasAreaOfEffect(entity));
  Line 235: /// Applies damage from a source entity to a target entity.
  Line 265: DebugLogger.Log("INFO", $"CombatSystem: Entity {application.Target.Id} killed by {application.Source.Id}");
  Line 268: DebugLogger.Log("INFO", $"CombatSystem: Applied {actualDamage:F1} {application.DamageType} damage from entity {application.Source.Id} to {application.Target.Id}");
  Line 273: /// Applies knockback force to an entity.
  Line 275: /// <param name="target">The entity to apply knockback to.</param>
  Line 277: private void ApplyKnockback(Entity target, Vector2 knockbackForce)
  Line 283: DebugLogger.Log("DEBUG", $"CombatSystem: Applied knockback {knockbackForce} to entity {target.Id}");
  Line 290: /// <param name="source">The source entity dealing damage.</param>
  Line 291: /// <param name="target">The target entity receiving damage.</param>
  Line 293: private void QueueDamageApplication(Entity source, Entity target, DamageComponent damageComponent)
  Line 311: /// <param name="entity">The entity to search around.</param>
  Line 313: private IEnumerable<Entity> FindDamageableEntitiesNear(Entity entity)
  Line 315: var transform = entity.GetComponent<TransformComponent>();
  Line 317: return Enumerable.Empty<Entity>();
  Line 330: private IEnumerable<Entity> FindEntitiesInRadius(Vector2 position, float radius)
  Line 333: .Where(entity => entity.IsEnabled && !entity.IsDestroyed)
  Line 334: .Where(entity =>
  Line 336: var transform = entity.GetComponent<TransformComponent>();
  Line 344: /// <param name="source">The source entity.</param>
  Line 345: /// <param name="target">The target entity.</param>
  Line 347: private bool ShouldApplyDamage(Entity source, Entity target)
  Line 366: /// <param name="entity1">First entity.</param>
  Line 367: /// <param name="entity2">Second entity.</param>
  Line 369: private bool AreSameTeam(Entity entity1, Entity entity2)
  Line 382: /// <param name="entity1">First entity.</param>
  Line 383: /// <param name="entity2">Second entity.</param>
  Line 385: private float GetCollisionRadius(Entity entity1, Entity entity2)
  Line 392: /// Checks if an entity has area-of-effect damage capability.
  Line 394: /// <param name="entity">The entity to check.</param>
  Line 395: /// <returns>True if the entity has AoE capability.</returns>
  Line 396: private bool HasAreaOfEffect(Entity entity)
  Line 398: var damageComponent = entity.GetComponent<DamageComponent>();
  Line 403: /// Gets the area-of-effect radius for an entity.
  Line 405: /// <param name="entity">The entity.</param>
  Line 407: private float GetAreaOfEffectRadius(Entity entity)
  Line 410: var damageComponent = entity.GetComponent<DamageComponent>();
  Line 427: // Check if either entity has damage component
  Line 464: Entity? projectile = null;
  Line 465: Entity? target = null;
  Line 467: // Identify which entity is the projectile
  Line 492: /// P11-14-08: Checks if an entity is a projectile.
  Line 494: /// <param name="entity">The entity to check.</param>
  Line 495: /// <returns>True if the entity is a projectile.</returns>
  Line 496: private bool IsProjectile(Entity entity)
  Line 499: return entity.HasComponent<DamageComponent>() && !entity.HasComponent<EnemyTypeComponent>();
  Line 535: .Count(entity => entity.GetComponent<ActiveComponent>()?.IsActive == true);
  Line 549: public Entity Source;
  Line 550: public Entity Target;
- File: .\Engine\ECS\Systems\NavigationSystem.cs
  Line 145: /// Requests a path for a specific entity.
  Line 147: /// <param name="entityId">The entity ID requesting the path.</param>
  Line 166: DebugLogger.Log("DEBUG", $"NavigationSystem: Path requested for entity {entityId} from {startPos} to {endPos}");
  Line 172: /// <param name="entityId">The entity ID to cancel.</param>
  Line 178: DebugLogger.Log("DEBUG", $"NavigationSystem: Cancelled path request for entity {entityId}");
  Line 189: foreach (var entity in agents)
  Line 191: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 192: var transform = entity.GetComponent<TransformComponent>();
  Line 196: RequestPath(entity.Id, transform.Position, navAgent.TargetPosition);
  Line 243: // Apply path to the entity
  Line 244: var entity = _ecsWorld.GetEntity(request.EntityId);
  Line 245: if (entity != null)
  Line 247: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 257: DebugLogger.Log("DEBUG", $"NavigationSystem: Path calculated for entity {request.EntityId} with {path.Count} waypoints");
  Line 266: DebugLogger.Log("WARNING", $"NavigationSystem: No path found for entity {request.EntityId}");
  Line 276: DebugLogger.Log("ERROR", $"NavigationSystem: Pathfinding failed for entity {request.EntityId}: {ex.Message}");
  Line 288: foreach (var entity in agents)
  Line 290: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 291: var transform = entity.GetComponent<TransformComponent>();
  Line 296: UpdateNavigationAgent(entity, navAgent, transform, deltaTime);
  Line 304: /// <param name="entity">The entity to update.</param>
  Line 308: private void UpdateNavigationAgent(Entity entity, NavAgentComponent navAgent, TransformComponent transform, float deltaTime)
  Line 313: RequestPath(entity.Id, transform.Position, navAgent.TargetPosition);
  Line 329: if (IsAgentBlocked(entity, navAgent, transform.Position))
  Line 331: HandleBlockedAgent(entity, navAgent);
  Line 357: /// <param name="entity">The entity to check.</param>
  Line 361: private bool IsAgentBlocked(Entity entity, NavAgentComponent navAgent, Vector2 position)
  Line 379: /// <param name="entity">The blocked entity.</param>
  Line 381: private void HandleBlockedAgent(Entity entity, NavAgentComponent navAgent)
  Line 383: if (_blockedAgents.Contains(entity.Id))
  Line 386: _blockedAgents.Add(entity.Id);
  Line 403: // Check if either entity should affect navigation
  Line 423: /// P11-15-06: Updates navigation grid based on entity collision properties.
  Line 425: /// <param name="entity">The entity to update navigation for.</param>
  Line 426: private void UpdateNavigationForEntity(Entity entity)
  Line 428: var transform = entity.GetComponent<TransformComponent>();
  Line 429: var collider = entity.GetComponent<ColliderComponent>();
  Line 430: var active = entity.GetComponent<ActiveComponent>();
  Line 436: if (!ShouldAffectNavigation(entity, collider, active))
  Line 439: // Update navigation grid based on entity bounds and state
  Line 450: /// P11-15-06: Determines if an entity should affect navigation.
  Line 452: /// <param name="entity">The entity to check.</param>
  Line 455: /// <returns>True if entity should affect navigation.</returns>
  Line 456: private bool ShouldAffectNavigation(Entity entity, ColliderComponent collider, ActiveComponent? active)
  Line 478: /// P11-15-06: Gets the size of an entity for navigation updates.
  Line 481: /// <returns>Size of the entity.</returns>
  Line 496: .Where(entity => IsAgentInArea(entity, center, size))
  Line 499: foreach (var entity in affectedAgents)
  Line 501: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 517: /// <param name="entity">The agent entity.</param>
  Line 521: private bool IsAgentInArea(Entity entity, Vector2 areaCenter, Vector2 areaSize)
  Line 523: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\ECS\Systems\RenderSystem.cs
  Line 67: .Select(entity => new
  Line 69: Entity = entity,
  Line 70: Transform = entity.GetComponent<TransformComponent>()!,
  Line 71: Renderable = entity.GetComponent<RenderableComponent>()!
  Line 78: // Render each entity
  Line 81: RenderEntity(context, entityData.Entity, entityData.Transform, entityData.Renderable);
  Line 98: /// Renders a single entity.
  Line 101: /// <param name="entity">The entity to render.</param>
  Line 102: /// <param name="transform">The entity's transform component.</param>
  Line 103: /// <param name="renderable">The entity's renderable component.</param>
  Line 104: private void RenderEntity(IRenderContext context, Entity entity, TransformComponent transform, RenderableComponent renderable)
  Line 130: // For now, we'll draw a placeholder rectangle to show the entity position
  Line 143: // Debug: Draw entity ID for debugging (optional)
  Line 145: context.DrawText($"E{entity.Id}", (int)renderPosition.X, (int)renderPosition.Y - 10);
  Line 150: DebugLogger.Log("ERROR", $"RenderSystem: Failed to render entity {entity.Id}: {ex.Message}");
- File: .\Engine\ECS\Systems\ScoringSystem.cs
  Line 73: /// Event fired when score is awarded for an entity.
  Line 75: public event Action<Entity, int>? OnScoreAwarded;
  Line 139: foreach (var entity in entitiesWithHealth)
  Line 141: var health = entity.GetComponent<HealthComponent>();
  Line 145: var score = entity.GetComponent<ScoreComponent>();
  Line 149: // Award score for this entity
  Line 150: AwardScoreForEntity(entity, score);
  Line 155: /// Awards score for a defeated entity.
  Line 157: /// <param name="entity">The entity that was defeated.</param>
  Line 158: /// <param name="scoreComponent">The score component of the entity.</param>
  Line 159: private void AwardScoreForEntity(Entity entity, ScoreComponent scoreComponent)
  Line 167: // Determine the scoring entity (simplified - would be player or tower)
  Line 168: var scoringEntity = FindScoringEntity(entity);
  Line 184: RecordScoreEvent(entity, awardedScore);
  Line 190: DebugLogger.Log("INFO", $"ScoringSystem: Awarded {awardedScore} points for entity {entity.Id}. Total: {_totalScore}");
  Line 237: /// <param name="entity">The entity that was scored.</param>
  Line 239: private void RecordScoreEvent(Entity entity, int score)
  Line 243: EntityId = entity.Id,
  Line 269: /// Finds the entity that should receive credit for the kill.
  Line 272: /// <param name="defeatedEntity">The entity that was defeated.</param>
  Line 273: /// <returns>The entity that should receive score credit.</returns>
  Line 274: private Entity FindScoringEntity(Entity defeatedEntity)
  Line 277: // 1. Check if the entity was killed by a player-owned projectile
  Line 278: // 2. Check if the entity was killed by a tower
  Line 279: // 3. Check if the entity was killed by environmental damage
  Line 280: // 4. Return the appropriate player or tower entity
  Line 282: // For now, return a placeholder or the first non-enemy entity
  Line 300: foreach (var entity in scoreEntities)
  Line 302: var score = entity.GetComponent<ScoreComponent>();
- File: .\Engine\ECS\Testing\ECSTestSuite.cs
  Line 7: //       - Entity lifecycle
  Line 81: // 1. ENTITY LIFECYCLE
  Line 85: /// Validates entity creation, destruction, and alive state.
- File: .\Engine\Entities\Enemy.cs
  Line 4: Purpose: Base enemy entity class for the game.
  Line 20: /// Represents an enemy entity in the game.
  Line 23: public class Enemy : Entity
  Line 181: public void TakeDamage(float damage, Entity? damageSource = null)
  Line 245: public bool TryAttack(Entity target)
  Line 358: public Entity? DamageSource { get; set; }
- File: .\Engine\Entities\Entity.cs
  Line 2: File:    Entity.cs
  Line 3: Purpose: Base entity with position, rotation, scale and update/render lifecycle.
  Line 11: public abstract class Entity
- File: .\Engine\Entities\Projectile.cs
  Line 8: public class Projectile : Entity
- File: .\Engine\Entities\Soldier.cs
  Line 7: public class Soldier : Entity
- File: .\Engine\Entities\Turret.cs
  Line 7: public class Turret : Entity
- File: .\Engine\Managers\EntityManager.cs
  Line 4: Purpose: Manages Entity collection; integration with Physics, Animation, RenderSystem; UpdateAll; Component support.
  Line 5: P11-03-02-B: Support attaching and retrieving SpriteComponent instances for any entity.
  Line 18: private readonly List<Entity> _entities = new List<Entity>();
  Line 19: private readonly Dictionary<Entity, Dictionary<Type, object>> _components = new Dictionary<Entity, Dictionary<Type, object>>();
  Line 24: public IReadOnlyList<Entity> Entities => _entities;
  Line 41: public void AddEntity(Entity entity)
  Line 43: if (entity != null && !_entities.Contains(entity))
  Line 44: _entities.Add(entity);
  Line 47: public void RemoveEntity(Entity entity)
  Line 49: if (entity != null)
  Line 51: _entities.Remove(entity);
  Line 52: // P11-03-02-B: Remove all components when entity is removed
  Line 53: if (_components.ContainsKey(entity))
  Line 55: _components.Remove(entity);
  Line 63: /// P11-03-02-B: Attach a component to an entity.
  Line 66: /// <param name="entity">Target entity</param>
  Line 68: public void AddComponent<T>(Entity entity, T component) where T : class
  Line 70: if (entity == null || component == null)
  Line 73: if (!_components.ContainsKey(entity))
  Line 75: _components[entity] = new Dictionary<Type, object>();
  Line 78: _components[entity][typeof(T)] = component;
  Line 82: /// P11-03-02-B: Retrieve a component from an entity.
  Line 85: /// <param name="entity">Target entity</param>
  Line 87: public T? GetComponent<T>(Entity entity) where T : class
  Line 89: if (entity == null || !_components.ContainsKey(entity))
  Line 92: if (_components[entity].TryGetValue(typeof(T), out var component))
  Line 101: /// P11-03-02-B: Remove a component from an entity.
  Line 104: /// <param name="entity">Target entity</param>
  Line 106: public bool RemoveComponent<T>(Entity entity) where T : class
  Line 108: if (entity == null || !_components.ContainsKey(entity))
  Line 111: return _components[entity].Remove(typeof(T));
  Line 115: /// P11-03-02-B: Check if an entity has a specific component.
  Line 118: /// <param name="entity">Target entity</param>
  Line 119: /// <returns>True if entity has component</returns>
  Line 120: public bool HasComponent<T>(Entity entity) where T : class
  Line 122: return GetComponent<T>(entity) != null;
  Line 127: /// This is used by RenderingSystem for efficient entity filtering.
  Line 130: public IReadOnlyList<Entity> GetEntitiesWithSpriteAndTransform()
  Line 132: var result = new List<Entity>();
  Line 134: foreach (var entity in _entities)
  Line 136: if (HasComponent<SpriteComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 138: result.Add(entity);
  Line 147: /// This is used by AnimationSystem for efficient entity filtering.
  Line 150: public IReadOnlyList<Entity> GetEntitiesWithAnimationAndSprite()
  Line 152: var result = new List<Entity>();
  Line 154: foreach (var entity in _entities)
  Line 156: if (HasComponent<AnimationComponent>(entity) && HasComponent<SpriteComponent>(entity))
  Line 158: result.Add(entity);
  Line 167: /// This is used by ParticleSystem for efficient entity filtering.
  Line 170: public IReadOnlyList<Entity> GetEntitiesWithParticleEmitterAndTransform()
  Line 172: var result = new List<Entity>();
  Line 174: foreach (var entity in _entities)
  Line 176: if (HasComponent<ParticleEmitterComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 178: result.Add(entity);
  Line 187: /// This is used by UISystem for efficient entity filtering.
  Line 190: public IReadOnlyList<Entity> GetEntitiesWithUIComponent()
  Line 192: var result = new List<Entity>();
  Line 194: foreach (var entity in _entities)
  Line 196: if (HasComponent<UIComponent>(entity))
  Line 198: result.Add(entity);
  Line 207: /// This is used by CollisionSystem for efficient entity filtering.
  Line 210: public IReadOnlyList<Entity> GetEntitiesWithCollisionAndTransform()
  Line 212: var result = new List<Entity>();
  Line 214: foreach (var entity in _entities)
  Line 216: if (HasComponent<CollisionComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 218: result.Add(entity);
  Line 227: /// This is used by PhysicsSystem for efficient entity filtering.
  Line 230: public IReadOnlyList<Entity> GetEntitiesWithPhysicsAndTransform()
  Line 232: var result = new List<Entity>();
  Line 234: foreach (var entity in _entities)
  Line 236: if (HasComponent<PhysicsComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 238: result.Add(entity);
  Line 247: /// This is used by TriggerSystem for efficient entity filtering.
  Line 250: public IReadOnlyList<Entity> GetEntitiesWithTriggerAndTransform()
  Line 252: var result = new List<Entity>();
  Line 254: foreach (var entity in _entities)
  Line 256: if (HasComponent<TriggerComponent>(entity) && HasComponent<TransformComponent>(entity))
  Line 258: result.Add(entity);
- File: .\Engine\Navigation\NavigationDebugRenderer.cs
  Line 221: foreach (var entity in agents)
  Line 223: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 224: var transform = entity.GetComponent<TransformComponent>();
  Line 262: foreach (var entity in agents)
  Line 264: var navAgent = entity.GetComponent<NavAgentComponent>();
  Line 265: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\Navigation\NavigationGrid.cs
  Line 317: /// P11-15-06: Updates walkability based on entity collision components.
  Line 319: /// <param name="entityPosition">World position of the entity.</param>
  Line 320: /// <param name="entitySize">Size of the entity in world units.</param>
  Line 321: /// <param name="isWalkable">Whether the entity area should be walkable.</param>
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Line 22: /// NavAgent-based navigation architecture. Supports single-entity migration,
  Line 28: // SINGLE ENTITY MIGRATION
  Line 32: /// Migrates a single entity from legacy MovementComponent to NavAgentComponent.
  Line 36: public static bool MigrateToNavAgent(Entity entity, ECSWorld world)
  Line 38: if (entity == null)
  Line 39: throw new ArgumentNullException(nameof(entity));
  Line 47: var transform = entity.GetComponent<TransformComponent>();
  Line 48: var enemyType = entity.GetComponent<EnemyTypeComponent>();
  Line 49: var legacyMovement = entity.GetComponent<MovementComponent>();
  Line 53: DebugLogger.Log("MIGRATION", "Entity missing TransformComponent.");
  Line 59: DebugLogger.Log("MIGRATION", "Entity missing EnemyTypeComponent.");
  Line 64: if (entity.HasComponent<NavAgentComponent>())
  Line 66: DebugLogger.Log("MIGRATION", "Entity already has NavAgentComponent. Skipping.");
  Line 85: entity.AddComponent(navAgent);
  Line 89: entity.RemoveComponent<MovementComponent>();
  Line 91: DebugLogger.Log("MIGRATION", $"Migrated entity {entity.Id} to NavAgent.");
  Line 104: public static int BatchMigrateToNavAgent(IEnumerable<Entity> entities, ECSWorld world)
  Line 111: foreach (var entity in entities)
  Line 113: if (MigrateToNavAgent(entity, world))
  Line 127: /// Creates a new entity with NavAgent navigation already configured.
  Line 129: public static Entity CreateNavAgentEntity(ECSWorld world, float speed, Vector2 position)
  Line 134: var entity = world.CreateEntity();
  Line 136: entity.AddComponent(new TransformComponent { X = position.X, Y = position.Y });
  Line 137: entity.AddComponent(new NavAgentComponent
  Line 145: DebugLogger.Log("MIGRATION", $"Created NavAgent entity at {position}");
  Line 147: return entity;
  Line 192: public static MigrationValidationResult ValidateMigration(IEnumerable<Entity> entities)
  Line 196: foreach (var entity in entities)
  Line 200: if (!entity.HasComponent<NavAgentComponent>())
  Line 202: result.AddIssue(entity.Id, "Missing NavAgentComponent.");
  Line 206: if (entity.HasComponent<MovementComponent>())
  Line 208: result.AddIssue(entity.Id, "Legacy MovementComponent still present.");
  Line 212: if (!entity.HasComponent<TransformComponent>())
  Line 214: result.AddIssue(entity.Id, "Missing TransformComponent.");
  Line 218: if (!entity.HasComponent<EnemyTypeComponent>())
  Line 220: result.AddIssue(entity.Id, "Missing EnemyTypeComponent.");
  Line 340: public Entity Entity { get; internal set; }
  Line 346: public sealed class Entity
  Line 353: internal Entity(int id, ECSWorld world)
  Line 363: comp.Entity = this;
  Line 387: private readonly Dictionary<int, Entity> _entities = new();
  Line 389: public Entity CreateEntity()
  Line 391: var e = new Entity(_nextId++, this);
  Line 396: public List<Entity> GetAllEntities() => new(_entities.Values);
- File: .\Engine\Navigation\NavigationVerificationSuite.cs
  Line 179: var agents = new List<Entity>();
  Line 261: var agents = new List<Entity>();
  Line 312: var agents = new List<Entity>();
  Line 426: // Create entity with legacy movement
  Line 470: /// Creates a test agent entity.
  Line 473: /// <returns>Created agent entity.</returns>
  Line 474: private Entity CreateTestAgent(Vector2 position)
  Line 476: var entity = _ecsWorld.CreateEntity();
  Line 477: entity.AddComponent(new TransformComponent(position));
  Line 478: entity.AddComponent(new RenderableComponent());
  Line 479: entity.AddComponent(new NavAgentComponent(1.0f));
  Line 480: entity.AddComponent(new ActiveComponent(true));
  Line 481: return entity;
  Line 485: /// Creates a test enemy entity.
  Line 488: /// <returns>Created enemy entity.</returns>
  Line 489: private Entity CreateTestEnemy(Vector2 position)
  Line 491: var entity = _ecsWorld.CreateEntity();
  Line 492: entity.AddComponent(new TransformComponent(position));
  Line 493: entity.AddComponent(new RenderableComponent());
  Line 494: entity.AddComponent(new EnemyTypeComponent(EnemyType.Zombie));
  Line 495: entity.AddComponent(new HealthComponent(100f));
  Line 496: entity.AddComponent(new NavAgentComponent(1.0f));
  Line 497: entity.AddComponent(new ActiveComponent(true));
  Line 498: return entity;
  Line 502: /// Creates a test obstacle entity.
  Line 505: /// <returns>Created obstacle entity.</returns>
  Line 506: private Entity CreateTestObstacle(Vector2 position)
  Line 508: var entity = _ecsWorld.CreateEntity();
  Line 509: entity.AddComponent(new TransformComponent(position));
  Line 510: entity.AddComponent(new RenderableComponent());
  Line 511: entity.AddComponent(new ColliderComponent(
  Line 516: entity.AddComponent(new ActiveComponent(true));
  Line 517: return entity;
- File: .\Engine\Performance\DebugOverlay.cs
  Line 17: /// P30-09-06: Add entity count display.
  Line 93: /// Gets or sets whether to show entity count.
  Line 366: /// Renders entity count.
  Line 370: // This would get actual entity count
  Line 371: var entityCount = 0; // Would get from entity system
- File: .\Engine\Physics\ColliderComponent.cs
  Line 19: /// No collision layer - entity doesn't participate in collisions.
  Line 92: /// Gets or sets the collision layer this entity belongs to.
  Line 142: // Transform shape bounds by entity's world position
  Line 239: DebugLogger.Log("INFO", $"ColliderComponent: Set shape to {shape?.ShapeType} for entity {Owner?.Id}");
  Line 251: DebugLogger.Log("INFO", $"ColliderComponent: Set layer={layer}, mask={mask} for entity {Owner?.Id}");
  Line 272: /// Gets the world-space collision shape (transformed by entity position).
- File: .\Engine\Physics\CollisionDebugRenderer.cs
  Line 217: foreach (var entity in collidableEntities)
  Line 219: var collider = entity.GetComponent<ColliderComponent>();
  Line 220: var transform = entity.GetComponent<TransformComponent>();
- File: .\Engine\Physics\CollisionEvent.cs
  Line 17: /// The first entity in the collision.
  Line 19: public readonly Entity EntityA;
  Line 22: /// The second entity in the collision.
  Line 24: public readonly Entity EntityB;
  Line 42: /// Gets the other entity in the collision from the perspective of a given entity.
  Line 44: /// <param name="entity">The reference entity.</param>
  Line 45: /// <returns>The other entity in the collision.</returns>
  Line 46: public Entity GetOther(Entity entity)
  Line 48: if (entity.Id == EntityA.Id)
  Line 50: if (entity.Id == EntityB.Id)
  Line 53: throw new ArgumentException("Entity is not part of this collision", nameof(entity));
  Line 59: /// <param name="entityA">The first entity.</param>
  Line 60: /// <param name="entityB">The second entity.</param>
  Line 63: public CollisionEvent(Entity entityA, Entity entityB, bool isTrigger, ContactInfo contact)
  Line 75: /// <param name="entityA">The first entity.</param>
  Line 76: /// <param name="entityB">The second entity.</param>
  Line 78: public CollisionEvent(Entity entityA, Entity entityB, bool isTrigger) : this(entityA, entityB, isTrigger, default)
  Line 83: /// Checks if this collision event involves the specified entity.
  Line 85: /// <param name="entity">The entity to check.</param>
  Line 86: /// <returns>True if the entity is part of this collision.</returns>
  Line 87: public bool Involves(Entity entity)
  Line 89: return EntityA.Id == entity.Id || EntityB.Id == entity.Id;
  Line 95: /// <param name="entityA">The first entity.</param>
  Line 96: /// <param name="entityB">The second entity.</param>
  Line 98: public bool IsBetween(Entity entityA, Entity entityB)
  Line 110: // Use a combination of both entity IDs for consistent ordering
  Line 163: return $"Collision [Entity{EntityA.Id} <-> Entity{EntityB.Id}, Trigger: {IsTrigger}]";
  Line 179: /// Normalized vector pointing from the first entity to the second.
- File: .\Engine\Physics\CollisionShape.cs
  Line 21: /// This is relative to the entity's transform position.
- File: .\Engine\Physics\CollisionVerificationSuite.cs
  Line 229: var entity = CreateTestEnemy(new Vector2(x, y));
  Line 244: throw new Exception($"Entity creation too slow: {creationTime}ms for 500 entities");
  Line 247: throw new Exception($"Entity update too slow: {updateTime}ms for 60 frames with 500 entities");
  Line 281: var entities = new List<Entity>();
  Line 290: var entity = CreateTestEnemy(new Vector2(x, y));
  Line 291: entities.Add(entity);
  Line 301: private List<CollisionResult> RunDeterministicSimulation(List<Entity> entities, string setupName)
  Line 317: foreach (var entity in entities)
  Line 319: var health = entity.GetComponent<HealthComponent>();
  Line 322: EntityId = entity.Id,
  Line 523: /// Creates a test enemy entity with collision components.
  Line 526: /// <returns>The created enemy entity.</returns>
  Line 527: private Entity CreateTestEnemy(Vector2 position)
  Line 529: var entity = _ecsWorld.CreateEntity();
  Line 531: entity.AddComponent(new TransformComponent(position));
  Line 532: entity.AddComponent(new RenderableComponent());
  Line 533: entity.AddComponent(new EnemyTypeComponent(EnemyType.Zombie));
  Line 534: entity.AddComponent(new HealthComponent(100f));
  Line 535: entity.AddComponent(new MovementComponent(1.0f));
  Line 536: entity.AddComponent(new ScoreComponent(10));
  Line 537: entity.AddComponent(new ActiveComponent(true));
  Line 538: entity.AddComponent(new ColliderComponent(
  Line 544: return entity;
  Line 548: /// Creates a test projectile entity with collision components.
  Line 552: /// <returns>The created projectile entity.</returns>
  Line 553: private Entity CreateTestProjectile(Vector2 position, Vector2 velocity)
  Line 555: var entity = _ecsWorld.CreateEntity();
  Line 557: entity.AddComponent(new TransformComponent(position));
  Line 558: entity.AddComponent(new RenderableComponent());
  Line 559: entity.AddComponent(new DamageComponent(25f, DamageType.Ballistic));
  Line 560: entity.AddComponent(new MovementComponent());
  Line 561: entity.AddComponent(new ActiveComponent(5.0f, true));
  Line 562: entity.AddComponent(new ColliderComponent(
  Line 568: var movement = entity.GetComponent<MovementComponent>();
  Line 571: return entity;
  Line 575: /// Creates a test player entity with collision components.
  Line 578: /// <returns>The created player entity.</returns>
  Line 579: private Entity CreateTestPlayer(Vector2 position)
  Line 581: var entity = _ecsWorld.CreateEntity();
  Line 583: entity.AddComponent(new TransformComponent(position));
  Line 584: entity.AddComponent(new RenderableComponent());
  Line 585: entity.AddComponent(new HealthComponent(100f));
  Line 586: entity.AddComponent(new MovementComponent(2.0f));
  Line 587: entity.AddComponent(new ActiveComponent(true));
  Line 588: entity.AddComponent(new ColliderComponent(
  Line 594: return entity;
  Line 598: /// Creates a test trigger volume entity.
  Line 602: /// <returns>The created trigger entity.</returns>
  Line 603: private Entity CreateTestTrigger(Vector2 position, Vector2 size)
  Line 605: var entity = _ecsWorld.CreateEntity();
  Line 607: entity.AddComponent(new TransformComponent(position));
  Line 608: entity.AddComponent(new RenderableComponent());
  Line 609: entity.AddComponent(new ActiveComponent(true));
  Line 610: entity.AddComponent(new ColliderComponent(
  Line 616: return entity;
- File: .\Engine\Physics\SpatialPartitionGrid.cs
  Line 68: /// Inserts an entity into the spatial grid.
  Line 70: /// <param name="entity">The entity to insert.</param>
  Line 71: /// <param name="bounds">The world-space bounds of the entity.</param>
  Line 72: public void Insert(Entity entity, BoundingBox bounds)
  Line 74: if (entity == null)
  Line 75: throw new ArgumentNullException(nameof(entity));
  Line 78: Remove(entity);
  Line 80: // Calculate which cells this entity spans
  Line 86: Entity = entity,
  Line 92: _entityEntries[entity.Id] = entry;
  Line 94: // Add entity to each cell it spans
  Line 103: cell.Entities.Add(entity);
  Line 106: DebugLogger.Log("DEBUG", $"SpatialPartitionGrid: Inserted entity {entity.Id} into {cellIndices.Count} cells");
  Line 110: /// Removes an entity from the spatial grid.
  Line 112: /// <param name="entity">The entity to remove.</param>
  Line 113: /// <returns>True if the entity was removed, false if not found.</returns>
  Line 114: public bool Remove(Entity entity)
  Line 116: if (entity == null)
  Line 119: if (!_entityEntries.TryGetValue(entity.Id, out var entry))
  Line 122: // Remove entity from all cells it spans
  Line 127: cell.Entities.Remove(entity);
  Line 137: _entityEntries.Remove(entity.Id);
  Line 139: DebugLogger.Log("DEBUG", $"SpatialPartitionGrid: Removed entity {entity.Id}");
  Line 144: /// Updates an entity's position in the spatial grid.
  Line 146: /// <param name="entity">The entity to update.</param>
  Line 147: /// <param name="newBounds">The new world-space bounds of the entity.</param>
  Line 148: /// <returns>True if the entity was updated, false if not found.</returns>
  Line 149: public bool Update(Entity entity, BoundingBox newBounds)
  Line 151: if (entity == null)
  Line 154: if (!_entityEntries.TryGetValue(entity.Id, out var entry))
  Line 167: Remove(entity);
  Line 168: Insert(entity, newBounds);
  Line 170: DebugLogger.Log("DEBUG", $"SpatialPartitionGrid: Updated entity {entity.Id} position");
  Line 179: public IEnumerable<Entity> Query(BoundingBox area)
  Line 182: var result = new HashSet<Entity>();
  Line 188: foreach (var entity in cell.Entities)
  Line 190: if (_entityEntries.TryGetValue(entity.Id, out var entry))
  Line 192: // Check if entity actually intersects the query area
  Line 195: result.Add(entity);
  Line 209: public IEnumerable<(Entity, Entity)> GetPotentialCollisions()
  Line 212: var result = new List<(Entity, Entity)>();
  Line 240: /// Gets potential collision pairs for a specific entity.
  Line 242: /// <param name="entity">The entity to check.</param>
  Line 243: /// <returns>Collection of entities that might collide with the specified entity.</returns>
  Line 244: public IEnumerable<Entity> GetPotentialCollisions(Entity entity)
  Line 246: if (!_entityEntries.TryGetValue(entity.Id, out var entry))
  Line 247: return Enumerable.Empty<Entity>();
  Line 249: var result = new HashSet<Entity>();
  Line 261: if (otherEntity.Id != entity.Id)
  Line 278: public IEnumerable<Entity> GetEntitiesInRadius(Vector2 center, float radius)
  Line 283: return candidates.Where(entity =>
  Line 285: if (!_entityEntries.TryGetValue(entity.Id, out var entry))
  Line 395: /// Creates an ordered pair from two entity IDs.
  Line 397: /// <param name="id1">The first entity ID.</param>
  Line 398: /// <param name="id2">The second entity ID.</param>
  Line 412: public HashSet<Entity> Entities { get; }
  Line 417: Entities = new HashSet<Entity>();
  Line 422: /// Represents an entity entry in the spatial partition grid.
  Line 426: public Entity Entity { get; set; }
- File: .\Engine\Scene\Entity.cs
  Line 9: /// Base entity class for scene objects.
  Line 10: /// P20-07-03: Implements entity with position, update, and render functionality.
  Line 12: public abstract class Entity
  Line 24: /// Gets the unique identifier for the entity.
  Line 33: /// Gets or sets the position of the entity.
  Line 49: /// Gets or sets the size of the entity.
  Line 65: /// Gets or sets the rotation of the entity (in radians).
  Line 81: /// Gets or sets whether the entity is enabled.
  Line 97: /// Gets or sets whether the entity is visible.
  Line 113: /// Gets or sets the scene this entity belongs to.
  Line 129: /// Gets the list of components attached to this entity.
  Line 134: /// Gets the bounding rectangle of the entity.
  Line 139: /// Gets the center position of the entity.
  Line 174: /// Event fired when the entity is destroyed.
  Line 179: /// Initializes a new entity.
  Line 181: /// <param name="id">Unique identifier for the entity.</param>
  Line 184: protected Entity(string id = null, Vector2? position = null, Vector2? size = null)
  Line 195: DebugLogger.Log("DEBUG", $"Entity: Created '{_id}' at {_position} size {_size}");
  Line 199: /// Updates the entity.
  Line 200: /// P20-07-03: Implements entity update functionality.
  Line 219: // Update entity-specific logic
  Line 224: DebugLogger.Log("ERROR", $"Entity: Failed to update '{_id}' - {ex.Message}");
  Line 229: /// Renders the entity.
  Line 230: /// P20-07-03: Implements entity render functionality.
  Line 249: // Render entity-specific elements
  Line 254: DebugLogger.Log("ERROR", $"Entity: Failed to render '{_id}' - {ex.Message}");
  Line 259: /// Adds a component to the entity.
  Line 267: DebugLogger.Log("WARNING", "Entity: Cannot add null component");
  Line 273: DebugLogger.Log("WARNING", $"Entity: Component '{component.GetType().Name}' already exists on '{_id}'");
  Line 278: component.Entity = this;
  Line 281: DebugLogger.Log("DEBUG", $"Entity: Added component '{component.GetType().Name}' to '{_id}'");
  Line 286: /// Removes a component from the entity.
  Line 299: component.Entity = null;
  Line 300: DebugLogger.Log("DEBUG", $"Entity: Removed component '{component.GetType().Name}' from '{_id}'");
  Line 342: /// Removes the entity from its scene.
  Line 344: /// <returns>True if entity was removed successfully.</returns>
  Line 354: /// Destroys the entity.
  Line 367: component.Entity = null;
  Line 374: DebugLogger.Log("DEBUG", $"Entity: Destroyed '{_id}'");
  Line 378: DebugLogger.Log("ERROR", $"Entity: Failed to destroy '{_id}' - {ex.Message}");
  Line 383: /// Moves the entity by the specified offset.
  Line 392: /// Rotates the entity by the specified angle.
  Line 401: /// Scales the entity by the specified factor.
  Line 410: /// Checks if this entity collides with another entity.
  Line 412: /// <param name="other">The other entity to check collision with.</param>
  Line 414: public bool CollidesWith(Entity other)
  Line 423: /// Entity-specific update logic.
  Line 433: /// Entity-specific render logic.
  Line 443: /// Gets entity information as a string.
  Line 447: return $"Entity: Id='{_id}', Pos={_position}, Size={_size}, " +
  Line 454: /// Interface for entity components.
  Line 460: /// Gets or sets the entity this component belongs to.
  Line 462: Entity Entity { get; set; }
  Line 487: /// Called when the component is added to an entity.
  Line 492: /// Called when the component is removed from an entity.
  Line 498: /// Base component class for entity components.
  Line 502: private Entity _entity;
  Line 507: /// Gets or sets the entity this component belongs to.
  Line 509: public Entity Entity
  Line 567: /// Called when the component is added to an entity.
  Line 571: DebugLogger.Log("DEBUG", $"Component: Added '{GetType().Name}' to entity");
  Line 575: /// Called when the component is removed from an entity.
  Line 579: DebugLogger.Log("DEBUG", $"Component: Removed '{GetType().Name}' from entity");
- File: .\Engine\Scene\GameplayScene.cs
  Line 16: private readonly List<Entity> _gameEntities;
  Line 67: _gameEntities = new List<Entity>();
  Line 245: var playerEntity = new Entity("Player", new Vector2(400, 300), new Vector2(32, 32));
  Line 279: foreach (var entity in _gameEntities)
  Line 281: // Update entity logic
  Line 282: entity.Update(deltaTime);
  Line 349: foreach (var entity in _gameEntities)
  Line 351: RemoveEntity(entity);
- File: .\Engine\Scene\Scene.cs
  Line 17: private List<Entity> _entities;
  Line 42: public IReadOnlyList<Entity> Entities => _entities.AsReadOnly();
  Line 70: /// Event fired when an entity is added to the scene.
  Line 72: public event Action<Entity> OnEntityAdded;
  Line 75: /// Event fired when an entity is removed from the scene.
  Line 77: public event Action<Entity> OnEntityRemoved;
  Line 88: _entities = new List<Entity>();
  Line 228: var entity = _entities[i];
  Line 229: if (entity.IsEnabled)
  Line 231: entity.Update(deltaTime);
  Line 257: foreach (var entity in _entities)
  Line 259: if (entity.IsVisible)
  Line 261: entity.Render(renderer);
  Line 275: /// Adds an entity to the scene.
  Line 277: /// <param name="entity">The entity to add.</param>
  Line 278: /// <returns>True if entity was added successfully.</returns>
  Line 279: public bool AddEntity(Entity entity)
  Line 281: if (entity == null)
  Line 283: DebugLogger.Log("WARNING", "Scene: Cannot add null entity");
  Line 287: if (_entities.Contains(entity))
  Line 289: DebugLogger.Log("WARNING", $"Scene: Entity '{entity.Id}' already exists in scene '{_name}'");
  Line 293: _entities.Add(entity);
  Line 294: entity.Scene = this;
  Line 295: OnEntityAdded?.Invoke(entity);
  Line 297: DebugLogger.Log("DEBUG", $"Scene: Added entity '{entity.Id}' to '{_name}'");
  Line 302: /// Removes an entity from the scene.
  Line 304: /// <param name="entity">The entity to remove.</param>
  Line 305: /// <returns>True if entity was removed successfully.</returns>
  Line 306: public bool RemoveEntity(Entity entity)
  Line 308: if (entity == null)
  Line 311: var removed = _entities.Remove(entity);
  Line 314: entity.Scene = null;
  Line 315: OnEntityRemoved?.Invoke(entity);
  Line 316: DebugLogger.Log("DEBUG", $"Scene: Removed entity '{entity.Id}' from '{_name}'");
  Line 323: /// Gets an entity by ID.
  Line 325: /// <param name="id">The ID of the entity to find.</param>
  Line 326: /// <returns>The entity, or null if not found.</returns>
  Line 327: public Entity GetEntity(string id)
  Line 329: return _entities.Find(entity => entity.Id == id);
  Line 335: /// <typeparam name="T">The entity type.</typeparam>
  Line 337: public List<T> GetEntities<T>() where T : Entity
  Line 340: foreach (var entity in _entities)
  Line 342: if (entity is T typedEntity)
  Line 380: foreach (var entity in _entities)
  Line 382: entity.Scene = null;
  Line 383: OnEntityRemoved?.Invoke(entity);
- File: .\Engine\State\GameplayState.cs
  Line 67: // - Process entity updates
- File: .\Engine\Systems\AnimationSystem.cs
  Line 4: Purpose: Subsystem for managing entity animation playback.
  Line 5: Features: Entity iteration, time-based animation advancement, frame updates, SpriteComponent integration.
  Line 19: /// Subsystem for managing entity animation playback.
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 70: /// - Writes correct frame data into entity's SpriteComponent
  Line 87: foreach (var entity in animatedEntities)
  Line 89: UpdateEntityAnimation(entity, deltaTime);
  Line 105: // Since we're working with Entity objects, we need to access the internal entity list
  Line 109: foreach (var entity in entities)
  Line 111: if (_entityManager.HasComponent<AnimationComponent>(entity) &&
  Line 112: _entityManager.HasComponent<SpriteComponent>(entity))
  Line 114: result.Add(entity);
  Line 122: /// P11-03-03-B: Updates animation for a single entity.
  Line 124: private void UpdateEntityAnimation(object entity, float deltaTime)
  Line 128: var animationComponent = _entityManager.GetComponent<AnimationComponent>(entity);
  Line 129: var spriteComponent = _entityManager.GetComponent<SpriteComponent>(entity);
  Line 161: // P11-03-03-B: Write correct frame data into entity's SpriteComponent
  Line 171: DebugLog($"AnimationSystem: Failed to update entity animation - {ex.Message}");
- File: .\Engine\Systems\CollisionSystem.cs
  Line 11: - Detects collisions and overlaps between entity pairs
  Line 13: - Provides spatial queries for entity intersection testing
  Line 130: /// <param name="entityManager">Entity manager for component access</param>
  Line 659: /// - Component-based design provides deterministic collision detection based on entity state
  Line 666: /// - Overlap resolution adjusts entity positions based on mass ratios
- File: .\Engine\Systems\EventLogSystem.cs
  Line 152: var message = $"TRIGGER ENTER: Entity {evt.TargetEntityId} entered trigger {evt.SourceEntityId} at {evt.Timestamp:HH:mm:ss.fff}";
  Line 174: var message = $"TRIGGER EXIT: Entity {evt.TargetEntityId} exited trigger {evt.SourceEntityId} at {evt.Timestamp:HH:mm:ss.fff}{durationStr}";
  Line 271: /// - Detailed logging includes timestamps, entity IDs, and event-specific data
- File: .\Engine\Systems\ParticleSystem.cs
  Line 97: /// <param name="entityManager">Entity manager for component access</param>
  Line 187: foreach (var entity in emitters)
  Line 189: var emitter = _entityManager.GetComponent<ParticleEmitterComponent>(entity);
  Line 190: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 261: foreach (var entity in entities)
  Line 263: if (_entityManager.HasComponent<ParticleEmitterComponent>(entity) &&
  Line 264: _entityManager.HasComponent<TransformComponent>(entity))
  Line 266: result.Add(entity);
  Line 369: // Create a temporary particle emitter entity for the effect
  Line 503: /// Spawns a temporary particle emitter entity for the effect.
  Line 532: // 1. Create a temporary entity with these components
  Line 535: // 4. Remove the entity after the effect duration
- File: .\Engine\Systems\PhysicsSystem.cs
  Line 20: Kinematic entity support for animated objects and platforms.
  Line 61: /// <param name="entityManager">Entity manager for component access</param>
  Line 121: foreach (var entity in physicsEntities)
  Line 123: UpdateEntityPhysics(entity, deltaTime);
  Line 135: /// Updates physics for a single entity.
  Line 137: private void UpdateEntityPhysics(object entity, float deltaTime)
  Line 141: var physics = _entityManager.GetComponent<PhysicsComponent>(entity);
  Line 142: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 172: DebugLog($"PhysicsSystem: Failed to update entity physics - {ex.Message}");
  Line 177: /// Updates kinematic entity position based on velocity.
  Line 258: /// Applies a force to an entity with physics component.
  Line 260: /// <param name="entity">Entity to apply force to</param>
  Line 262: public void ApplyForce(object entity, PointF force)
  Line 266: var physics = _entityManager.GetComponent<PhysicsComponent>(entity);
  Line 270: DebugLog($"PhysicsSystem: Applied force {force} to entity");
  Line 280: /// Applies an impulse to an entity with physics component.
  Line 282: /// <param name="entity">Entity to apply impulse to</param>
  Line 284: public void ApplyImpulse(object entity, PointF impulse)
  Line 288: var physics = _entityManager.GetComponent<PhysicsComponent>(entity);
  Line 292: DebugLog($"PhysicsSystem: Applied impulse {impulse} to entity");
  Line 361: /// - Component-based design provides deterministic physics based on entity state
- File: .\Engine\Systems\RenderingSystem.cs
  Line 6: Implements full dependency injection, entity iteration, asset management, and platform rendering.
  Line 8: Role:      Central rendering system for all visual entity rendering.
  Line 18: Efficient entity iteration with component filtering.
  Line 21: Transform integration for proper entity positioning and scaling.
  Line 93: /// <param name="entityManager">Entity manager for accessing renderable entities</param>
  Line 164: foreach (var entity in renderableEntities)
  Line 166: ProcessComponentBasedEntity(entity);
  Line 188: /// P11-03-02-C: Processes entity using component-based rendering.
  Line 190: private void ProcessComponentBasedEntity(Entity entity)
  Line 194: var spriteComponent = _entityManager.GetComponent<SpriteComponent>(entity);
  Line 195: var transformComponent = _entityManager.GetComponent<TransformComponent>(entity);
  Line 203: DebugLog($"RenderingSystem: Skipping invisible entity {entity.GetType().Name}");
  Line 227: DebugLog($"RenderingSystem: Failed to process entity {entity.GetType().Name} - {ex.Message}");
  Line 306: /// RENDERING IS DRIVEN PURELY BY ENTITY STATE:
  Line 313: /// - Component-based design provides deterministic rendering based on entity state
  Line 360: /// Data structure for entity render information.
  Line 384: /// Render command containing all data needed to draw an entity.
  Line 393: public Entity? Entity { get; set; }
- File: .\Engine\Systems\TriggerSystem.cs
  Line 24: /// respects TriggerOnce and avoids re-triggering the same entity, and does not apply gameplay logic directly.
  Line 39: /// <param name="entityManager">Entity manager for component access</param>
  Line 77: /// - Respects TriggerOnce and avoids re-triggering the same entity
  Line 121: /// P11-04-03-C: Checks proximities for a single trigger entity.
  Line 142: // Check if target can trigger this entity
  Line 162: /// Checks if a target entity can trigger a trigger entity.
  Line 202: // Additional check: ensure target entity's collider is within trigger radius
  Line 203: // This accounts for target entity size
  Line 211: DebugLog($"TriggerSystem: Failed to check entity in trigger range - {ex.Message}");
  Line 217: /// Gets the effective radius of a target entity's collider.
  Line 278: // P11-04-03-C: Respects TriggerOnce and avoids re-triggering the same entity
  Line 282: // Mark entity as triggered
  Line 315: // Remove entity from triggered entities list
  Line 404: /// - TriggerEnterEvent: Published when entity enters trigger range
  Line 407: /// - TriggerExitEvent: Published when entity exits trigger range
  Line 438: /// - Events include source entity, target entity, and timestamp
- File: .\Engine\Systems\UISystem.cs
  Line 71: /// <param name="entityManager">Entity manager for component access</param>
  Line 276: foreach (var entity in entities)
  Line 278: if (_entityManager.HasComponent<UIComponent>(entity))
  Line 280: result.Add(entity);
  Line 288: /// Updates a single UI entity.
  Line 290: private void UpdateUIEntity(object entity, float deltaTime)
  Line 294: var uiComponent = _entityManager.GetComponent<UIComponent>(entity);
  Line 304: DebugLog($"UISystem: Failed to update UI entity - {ex.Message}");
  Line 309: /// Renders a single UI entity.
  Line 312: private void RenderUIEntity(object entity, IRenderContext context)
  Line 316: var uiComponent = _entityManager.GetComponent<UIComponent>(entity);
  Line 348: DebugLog($"UISystem: Failed to render UI entity - {ex.Message}");
  Line 450: /// P11-04-10-H: Shows respawn countdown for the specified entity.
  Line 452: /// <param name="entityId">The entity respawning</param>
  Line 459: DebugLog($"UISystem: Showed respawn countdown for entity {entityId} ({respawnTime}s)");
  Line 468: /// P11-04-10-H: Hides respawn countdown for the specified entity.
  Line 470: /// <param name="entityId">The entity whose countdown to hide</param>
  Line 476: DebugLog($"UISystem: Hid respawn countdown for entity {entityId}");
  Line 485: /// Updates respawn countdown time for the specified entity.
  Line 487: /// <param name="entityId">The entity respawning</param>
  Line 494: DebugLog($"UISystem: Updated respawn countdown for entity {entityId} ({remainingTime}s remaining)");
  Line 720: /// P11-04-12-I: Shows the inventory panel for the specified entity.
  Line 722: /// <param name="entityId">The entity whose inventory to show</param>
  Line 729: DebugLog($"UISystem: Showed inventory panel for entity {entityId}");
  Line 884: /// <param name="entity">Entity with UIComponent</param>
  Line 886: public void UpdateUIText(object entity, string text)
  Line 890: var uiComponent = _entityManager.GetComponent<UIComponent>(entity);
  Line 906: /// <param name="entity">Entity with UIComponent</param>
  Line 908: public void UpdateUIVisibility(object entity, bool isVisible)
  Line 912: var uiComponent = _entityManager.GetComponent<UIComponent>(entity);
- File: .\Engine\Systems\WaveSystem.cs
  Line 4158: // Check entity limit (simplified check)
  Line 4161: // This would require access to entity count - simplified for now
  Line 4162: // In a real implementation, you'd check actual entity limits
  Line 4443: // In a real implementation, we would track the actual entity ID
  Line 5903: // Simulate movement speed sampling (in real implementation, get from entity)
  Line 7609: suggestions.Add("Review entity lifecycle management and cleanup policies");
- File: .\Engine\Systems\Achievements\AchievementDefinition.cs
  Line 70: /// Optional entity type filter for entity-specific achievements
  Line 142: /// <param name="entityType">Optional entity type filter</param>
  Line 186: /// <summary>Kills of specific entity type</summary>
- File: .\Engine\Systems\Achievements\ChallengeDefinition.cs
  Line 58: /// Optional entity type filter for entity-specific challenges
  Line 151: /// <param name="entityType">Optional entity type filter</param>
  Line 217: /// <summary>Kills of specific entity type</summary>
- File: .\Engine\Systems\Events\AchievementUnlockedEvent.cs
  Line 68: /// Entity ID that triggered the achievement completion (if applicable)
- File: .\Engine\Systems\Events\ChallengeCompletedEvent.cs
  Line 80: /// Entity ID that triggered the challenge completion (if applicable)
- File: .\Engine\Systems\Events\EntityDiedEvent.cs
  Line 3: Purpose: Event class for entity death notifications.
  Line 4: Features: Entity identification, killer tracking, timestamp, death reason.
  Line 16: /// Event published when an entity dies in the game world.
  Line 23: /// Gets or sets the unique identifier of the entity that died.
  Line 29: /// Gets or sets the identifier of the entity that caused the death.
  Line 35: /// Gets or sets the timestamp when the entity died.
  Line 41: /// Gets or sets the reason for the entity's death.
  Line 49: /// Gets or sets the location where the entity died.
  Line 54: /// Gets or sets the entity's current health at time of death.
  Line 79: return $"EntityDiedEvent: Entity={EntityId}, " +
  Line 88: /// Categories of entity death types.
- File: .\Engine\Systems\Events\EntityRespawnedEvent.cs
  Line 3: Purpose: Event class for entity respawn notifications.
  Line 4: Features: Entity identification, respawn location, timestamp, respawn context.
  Line 15: /// Event published when an entity respawns in the game world.
  Line 22: /// Gets or sets the unique identifier of the entity that respawned.
  Line 28: /// Gets or sets the timestamp when the entity respawned.
  Line 34: /// Gets or sets the location where the entity respawned.
  Line 40: /// Gets or sets the reason for the entity's respawn.
  Line 48: /// Gets or sets the respawn delay in seconds before the entity actually respawned.
  Line 53: /// Gets or sets the health percentage the entity respawned with (0.0 to 1.0).
  Line 73: return $"EntityRespawnedEvent: Entity={EntityId}, " +
- File: .\Engine\Systems\Events\GameOverEvent.cs
  Line 76: /// Gets or sets the entity that triggered the game over (if applicable).
- File: .\Engine\Systems\Events\IEventListener.cs
  Line 42: ///         Debug.Log($"Entity {evt.TargetEntityId} entered trigger {evt.SourceEntityId}");
- File: .\Engine\Systems\Events\ItemAddedEvent.cs
  Line 7: /// Event fired when an item is successfully added to an entity's inventory.
  Line 20: /// ID of the entity that received the item.
  Line 52: /// ID of the entity or system that provided the item (if applicable).
  Line 65: /// Current total quantity of this item type in the entity's inventory after addition.
  Line 92: /// <param name="entityId">ID of the entity receiving the item</param>
  Line 124: var sourceEntityInfo = SourceEntityId.HasValue ? $" (entity {SourceEntityId.Value})" : "";
  Line 126: return $"[ItemAdded] Entity {EntityId} received {Quantity}x {ItemDefinitionId}{partialInfo}{sourceInfo}{sourceEntityInfo} (total: {TotalQuantity}) ({Timestamp:yyyy-MM-dd HH:mm:ss})";
- File: .\Engine\Systems\Events\ItemPickupEvent.cs
  Line 8: /// Event fired when an entity picks up an item.
  Line 9: /// Contains information about the item being picked up and the entity receiving it.
  Line 21: /// ID of the entity that picked up the item.
  Line 65: /// ID of the entity or container that dropped the item (if applicable).
  Line 92: /// <param name="entityId">ID of the entity picking up the item</param>
  Line 111: return $"[ItemPickup] Entity {EntityId} picked up {Quantity}x {ItemDefinitionId} at {Position} ({Timestamp:yyyy-MM-dd HH:mm:ss})";
- File: .\Engine\Systems\Events\ItemRemovedEvent.cs
  Line 7: /// Event fired when an item is removed from an entity's inventory.
  Line 20: /// ID of the entity that had the item removed.
  Line 45: /// ID of the target entity (if applicable).
  Line 46: /// Used for transfers, sales, or when the item benefits another entity.
  Line 58: /// Current total quantity of this item type in the entity's inventory after removal.
  Line 98: /// <param name="entityId">ID of the entity losing the item</param>
  Line 147: var targetInfo = TargetEntityId.HasValue ? $" to entity {TargetEntityId.Value}" : "";
  Line 151: return $"[ItemRemoved] Entity {EntityId} lost {Quantity}x {ItemDefinitionId} ({Reason}){targetInfo}{valueInfo}{voluntaryInfo} (remaining: {RemainingQuantity}) ({Timestamp:yyyy-MM-dd HH:mm:ss})";
- File: .\Engine\Systems\Events\ItemUseEvent.cs
  Line 7: /// Event fired when an entity uses an item.
  Line 20: /// ID of the entity using the item.
  Line 38: /// ID of the target entity (if applicable).
  Line 86: /// <param name="entityId">ID of the entity using the item</param>
  Line 98: /// Creates a new item use event with a target entity.
  Line 100: /// <param name="entityId">ID of the entity using the item</param>
  Line 102: /// <param name="targetEntityId">ID of the target entity</param>
  Line 116: /// <param name="entityId">ID of the entity using the item</param>
  Line 157: var targetInfo = TargetEntityId.HasValue ? $" on entity {TargetEntityId.Value}" :
  Line 164: return $"[ItemUse] Entity {EntityId} used {Quantity}x {ItemDefinitionId}{targetInfo}{resultInfo} ({Timestamp:yyyy-MM-dd HH:mm:ss})";
- File: .\Engine\Systems\Events\KillAttributedEvent.cs
  Line 49: /// Gets or sets the unique identifier of the entity that died (victim).
  Line 55: /// Gets or sets the identifier of the entity that caused the death (killer).
  Line 61: /// Gets or sets the team of the killer entity.
  Line 75: /// True when the entity caused its own death.
- File: .\Engine\Systems\Events\LevelUpEvent.cs
  Line 74: /// Entity ID that triggered the level-up (if applicable)
- File: .\Engine\Systems\Events\ResourceChangedEvent.cs
  Line 7: /// Event fired when an entity's resources change.
  Line 20: /// ID of the entity whose resources changed.
  Line 58: /// ID of the entity or system that caused the change (if applicable).
  Line 104: /// <param name="entityId">ID of the entity whose resources changed</param>
  Line 124: /// <param name="sourceEntityId">ID of the entity causing the change (optional)</param>
  Line 169: var sourceEntityInfo = SourceEntityId.HasValue ? $" (entity {SourceEntityId.Value})" : "";
  Line 173: return $"[ResourceChanged] Entity {EntityId} {changeType} {GetAbsoluteChange()} {ResourceType}{sourceInfo}{sourceEntityInfo}{contextInfo}{depletionInfo} ({OldAmount} → {NewAmount}) ({Timestamp:yyyy-MM-dd HH:mm:ss})";
- File: .\Engine\Systems\Events\TriggerEnterEvent.cs
  Line 4: Features: Source entity, target entity, timestamp, serialization support.
  Line 15: /// Event published when an entity enters a trigger's detection range.
  Line 16: /// P11-04-03-D: Includes SourceEntityId (the trigger), TargetEntityId (the entity that entered), and Timestamp.
  Line 23: /// The trigger entity that detected the target entity.
  Line 24: /// This entity has a TriggerComponent and initiated the trigger event.
  Line 30: /// The entity that entered the trigger's detection range.
  Line 31: /// This entity typically has a CollisionComponent and was detected by the trigger.
  Line 52: /// <param name="sourceEntityId">The trigger entity</param>
  Line 53: /// <param name="targetEntityId">The entity that entered the trigger</param>
  Line 64: /// <param name="sourceEntityId">The trigger entity</param>
  Line 65: /// <param name="targetEntityId">The entity that entered the trigger</param>
- File: .\Engine\Systems\Events\TriggerExitEvent.cs
  Line 4: Features: Source entity, target entity, timestamp, serialization support.
  Line 15: /// Event published when an entity exits a trigger's detection range.
  Line 16: /// P11-04-03-D: Includes SourceEntityId (the trigger), TargetEntityId (the entity that exited), and Timestamp.
  Line 23: /// The trigger entity that detected the target entity leaving.
  Line 24: /// This entity has a TriggerComponent and initiated the trigger event.
  Line 30: /// The entity that exited the trigger's detection range.
  Line 31: /// This entity typically has a CollisionComponent and was previously detected by the trigger.
  Line 50: /// Duration the entity was within the trigger range (calculated if available).
  Line 58: /// <param name="sourceEntityId">The trigger entity</param>
  Line 59: /// <param name="targetEntityId">The entity that exited the trigger</param>
  Line 70: /// <param name="sourceEntityId">The trigger entity</param>
  Line 71: /// <param name="targetEntityId">The entity that exited the trigger</param>
  Line 74: /// <param name="duration">How long the entity was in the trigger range</param>
- File: .\Engine\Systems\Events\ZoneEnteredEvent.cs
  Line 5: Features: Zone entity, subject entity, timestamp, serialization support.
  Line 17: /// Event published when an entity enters a zone's detection range.
  Line 18: /// P11-04-05-E: Includes ZoneEntityId (the zone), SubjectEntityId (the entity that entered), and Timestamp.
  Line 25: /// The zone entity that detected the subject entity entering.
  Line 26: /// This entity has a ZoneComponent and defines the zone properties.
  Line 32: /// The entity that entered the zone's detection range.
  Line 33: /// This could be a player, enemy, or any other entity with collision components.
  Line 60: /// <param name="zoneEntityId">The zone entity</param>
  Line 61: /// <param name="subjectEntityId">The entity that entered the zone</param>
  Line 74: /// <param name="zoneEntityId">The zone entity</param>
  Line 75: /// <param name="subjectEntityId">The entity that entered the zone</param>
- File: .\Engine\Systems\Events\ZoneExitedEvent.cs
  Line 5: Features: Zone entity, subject entity, timestamp, duration tracking, serialization support.
  Line 17: /// Event published when an entity exits a zone's detection range.
  Line 18: /// P11-04-05-E: Includes ZoneEntityId (the zone), SubjectEntityId (the entity that exited), Timestamp, and Duration.
  Line 25: /// The zone entity that detected the subject entity leaving.
  Line 26: /// This entity has a ZoneComponent and defines the zone properties.
  Line 32: /// The entity that exited the zone's detection range.
  Line 33: /// This could be a player, enemy, or any other entity with collision components.
  Line 52: /// Duration the entity was within the zone.
  Line 66: /// <param name="zoneEntityId">The zone entity</param>
  Line 67: /// <param name="subjectEntityId">The entity that exited the zone</param>
  Line 79: /// <param name="zoneEntityId">The zone entity</param>
  Line 80: /// <param name="subjectEntityId">The entity that exited the zone</param>
  Line 83: /// <param name="duration">How long the entity was in the zone</param>
- File: .\Engine\Systems\Gameplay\AnimationSystem.cs
  Line 8: Manages entity animation states and produces updated transforms
  Line 24: /// Describes the current animation state of an entity.
  Line 64: /// Manages entity animation states and produces updated transforms.
  Line 73: /// Read-only snapshot of all current animation transforms, keyed by entity ID.
  Line 94: /// Registers an entity for animation tracking.
  Line 105: /// Removes an entity from animation tracking.
- File: .\Engine\Systems\Gameplay\AnimationTriggerSystem.cs
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 69: /// Handles EntityDiedEvent by triggering death animations on the deceased entity.
  Line 71: /// <param name="deathEvent">The entity death event containing death information</param>
  Line 82: DebugLog($"AnimationTriggerSystem: Processing death for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 84: // Get the entity's AnimationComponent
  Line 88: DebugLog($"AnimationTriggerSystem: Entity {deathEvent.EntityId} has no AnimationComponent - skipping animation trigger");
  Line 95: DebugLog($"AnimationTriggerSystem: Death animation triggered for Entity {deathEvent.EntityId}");
  Line 99: DebugLog($"AnimationTriggerSystem: Failed to process death animation for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 115: DebugLog($"AnimationTriggerSystem: Playing death animation '{animationName}' for Entity {deathEvent.EntityId}");
  Line 128: DebugLog($"AnimationTriggerSystem: Failed to trigger death animation for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 154: /// <param name="entityId">The ID of the entity whose animation was triggered</param>
  Line 159: string logMessage = $"ANIMATION_TRIGGER: Entity={entityId}, Animation={animationName}, DeathType={deathType}, Timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
- File: .\Engine\Systems\Gameplay\CameraSystem.cs
  Line 37: // Optional follow target (entity ID in AnimationSystem)
  Line 101: /// Sets the camera to follow an entity tracked by AnimationSystem.
  Line 148: /// Updates the camera to follow an entity using its animation transform.
  Line 169: /// Returns the follow target entity ID, or -1 if no target is set.
- File: .\Engine\Systems\Gameplay\DamageSystem.cs
  Line 70: /// applies damage to target entities, supports damage amount, damage type, and source entity tracking,
  Line 104: /// <param name="entityManager">Entity manager for component access</param>
  Line 189: // Handle entity destruction if needed
  Line 228: /// Gets the damage amount for a projectile entity.
  Line 248: /// Gets the damage type for a projectile entity.
  Line 268: /// Applies damage to a target entity.
  Line 278: DebugLog($"DamageSystem: Target entity {targetEntityId} has no HealthComponent");
  Line 325: /// Handles entity destruction after damage application.
  Line 331: // Remove entity from EntityManager
  Line 334: DebugLog($"DamageSystem: Entity {result.TargetEntityId} destroyed by damage from {result.SourceEntityId}");
  Line 338: DebugLog($"DamageSystem: Failed to handle entity destruction - {ex.Message}");
  Line 392: /// - Tracks damage source entity for audit and gameplay purposes
  Line 393: /// - Handles entity destruction when health reaches zero
  Line 410: /// - Entity destruction can be extended with death animations, loot drops, or score updates
- File: .\Engine\Systems\Gameplay\DeathEffectSystem.cs
  Line 9: plays death sound effects at the entity's position. Supports different effects based
  Line 10: on DeathType or entity type with gameplay-agnostic design.
  Line 41: /// <param name="entityManager">Entity manager for component access</param>
  Line 86: /// Handles EntityDiedEvent by spawning death effects at the entity's position.
  Line 88: /// <param name="deathEvent">The entity death event containing death information</param>
  Line 99: DebugLog($"DeathEffectSystem: Processing death effects for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 101: // Get the entity's position from TransformComponent
  Line 105: DebugLog($"DeathEffectSystem: Entity {deathEvent.EntityId} has no TransformComponent - using default position");
  Line 110: // Spawn death effects at the entity's position
  Line 113: DebugLog($"DeathEffectSystem: Death effects spawned for Entity {deathEvent.EntityId} at position {transformComponent.Position}");
  Line 117: DebugLog($"DeathEffectSystem: Failed to process death effects for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 144: DebugLog($"DeathEffectSystem: Failed to spawn death effects for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 168: DebugLog($"DeathEffectSystem: Failed to spawn particle effects for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 192: DebugLog($"DeathEffectSystem: Failed to play sound effects for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 279: /// <param name="entityId">The ID of the entity whose effects were spawned</param>
  Line 288: string logMessage = $"DEATH_EFFECTS: Entity={entityId}, Position={position}, DeathType={deathType}, Particles=[{particleEffects}], Sounds=[{soundEffects}], Timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
- File: .\Engine\Systems\Gameplay\DeathSystem.cs
  Line 4: Purpose: System for detecting entity deaths and publishing death events.
  Line 9: marks entity as dead and publishes EntityDiedEvent.
  Line 22: /// System that monitors entity health and publishes death events.
  Line 29: private readonly HashSet<Entity> _processedDeaths = new HashSet<Entity>();
  Line 45: /// <param name="entityManager">The entity manager.</param>
  Line 69: /// Updates the death system and checks for entity deaths.
  Line 79: var deadEntities = new List<Entity>();
  Line 82: foreach (var entity in entities)
  Line 84: var healthComponent = entity.GetComponent<HealthComponent>();
  Line 85: if (healthComponent != null && healthComponent.IsDead && !_processedDeaths.Contains(entity))
  Line 87: deadEntities.Add(entity);
  Line 88: _processedDeaths.Add(entity);
  Line 99: _processedDeaths.RemoveWhere(entity =>
  Line 101: var healthComponent = entity.GetComponent<HealthComponent>();
  Line 107: /// Processes the death of a specific entity.
  Line 109: /// <param name="entity">The entity that died.</param>
  Line 110: private void ProcessEntityDeath(Entity entity)
  Line 117: EntityId = entity.Id,
  Line 118: KillerEntityId = GetKillerEntityId(entity),
  Line 120: DeathReason = DetermineDeathReason(entity)
  Line 126: LogEntityDeath(entity, deathEvent);
  Line 133: System.Diagnostics.Debug.WriteLine($"Error processing death for entity {entity.Id}: {ex.Message}");
  Line 138: /// Determines the ID of the entity that killed this entity.
  Line 140: /// <param name="entity">The entity that died.</param>
  Line 141: /// <returns>The killer entity ID, or null if unknown.</returns>
  Line 142: private object? GetKillerEntityId(Entity entity)
  Line 144: var healthComponent = entity.GetComponent<HealthComponent>();
  Line 145: if (healthComponent?.LastDamageSource is Entity killerEntity)
  Line 154: /// Determines the reason for entity death.
  Line 156: /// <param name="entity">The entity that died.</param>
  Line 158: private string DetermineDeathReason(Entity entity)
  Line 160: var healthComponent = entity.GetComponent<HealthComponent>();
  Line 170: /// Logs entity death for audit purposes.
  Line 172: /// <param name="entity">The entity that died.</param>
  Line 174: private void LogEntityDeath(Entity entity, EntityDiedEvent deathEvent)
  Line 176: var message = $"ENTITY DEATH: ID={entity.Id}, " +
- File: .\Engine\Systems\Gameplay\EntityRemovalSystem.cs
  Line 27: private readonly Dictionary<Entity, PendingRemoval> _pendingRemovals = new Dictionary<Entity, PendingRemoval>();
  Line 36: /// Gets whether the entity removal system is initialized.
  Line 43: /// <param name="entityManager">The entity manager.</param>
  Line 52: /// Initializes the entity removal System and subscribes to death events.
  Line 59: // Subscribe to entity death events
  Line 66: /// Updates the entity removal System and processes pending removals.
  Line 79: var entity = kvp.Key;
  Line 86: _pendingRemovals.Remove(entity);
  Line 90: _pendingRemovals[entity] = pendingRemoval;
  Line 97: ProcessEntityRemoval(removal.Entity);
  Line 102: /// Handles entity death events and schedules removal.
  Line 104: /// <param name="deathEvent">The entity death event.</param>
  Line 109: // Find the entity that died
  Line 110: var entity = _entityManager.GetAllEntities()
  Line 113: if (entity == null)
  Line 115: System.Diagnostics.Debug.WriteLine($"EntityRemovalSystem: Could not find entity {deathEvent.EntityId} for removal");
  Line 120: ScheduleEntityRemoval(entity, DefaultRemovalDelay);
  Line 122: LogEntityRemovalScheduled(entity, deathEvent, DefaultRemovalDelay);
  Line 126: System.Diagnostics.Debug.WriteLine($"Error scheduling removal for entity {deathEvent.EntityId}: {ex.Message}");
  Line 131: /// Schedules an entity for removal after the specified delay.
  Line 133: /// <param name="entity">The entity to remove.</param>
  Line 135: public void ScheduleEntityRemoval(Entity entity, float delay)
  Line 137: if (entity == null)
  Line 140: _pendingRemovals[entity] = new PendingRemoval
  Line 142: Entity = entity,
  Line 151: /// Immediately schedules an entity for removal.
  Line 153: /// <param name="entity">The entity to remove.</param>
  Line 154: public void ScheduleImmediateRemoval(Entity entity)
  Line 156: ScheduleEntityRemoval(entity, 0);
  Line 160: /// Processes the actual removal of an entity.
  Line 162: /// <param name="entity">The entity to remove.</param>
  Line 163: private void ProcessEntityRemoval(Entity entity)
  Line 167: // Remove from entity manager
  Line 168: _entityManager.RemoveEntity(entity);
  Line 171: LogEntityRemovalCompleted(entity);
  Line 178: System.Diagnostics.Debug.WriteLine($"Error removing entity {entity.Id}: {ex.Message}");
  Line 183: /// Logs when an entity removal is scheduled.
  Line 185: /// <param name="entity">The entity being removed.</param>
  Line 188: private void LogEntityRemovalScheduled(Entity entity, EntityDiedEvent deathEvent, float delay)
  Line 190: var message = $"REMOVAL SCHEDULED: Entity {entity.Id}, " +
  Line 201: /// Logs when an entity removal is completed.
  Line 203: /// <param name="entity">The entity that was removed.</param>
  Line 204: private void LogEntityRemovalCompleted(Entity entity)
  Line 206: var message = $"REMOVAL COMPLETED: Entity {entity.Id} removed from world";
  Line 214: /// Cancels a pending entity removal.
  Line 216: /// <param name="entity">The entity to cancel removal for.</param>
  Line 218: public bool CancelPendingRemoval(Entity entity)
  Line 220: if (_pendingRemovals.Remove(entity))
  Line 222: LogEntityRemovalCancelled(entity);
  Line 230: /// Logs when an entity removal is cancelled.
  Line 232: /// <param name="entity">The entity whose removal was cancelled.</param>
  Line 233: private void LogEntityRemovalCancelled(Entity entity)
  Line 235: var message = $"REMOVAL CANCELLED: Entity {entity.Id} removal cancelled";
  Line 243: /// Gets statistics about entity removals.
  Line 245: /// <returns>Entity removal statistics.</returns>
  Line 257: /// Resets the entity removal System state.
  Line 265: /// Shuts down the entity removal System.
  Line 286: /// Represents a pending entity removal.
  Line 290: public Entity Entity { get; set; }
  Line 298: /// Statistics for the entity removal System.
- File: .\Engine\Systems\Gameplay\GameOverSystem.cs
  Line 71: /// <param name="entityManager">Entity manager for component access</param>
  Line 79: // Subscribe to entity death events
  Line 108: /// <param name="triggeringEntityId">Optional entity that triggered game over</param>
  Line 195: /// Handles entity death events to check for game over conditions.
  Line 198: /// <param name="deathEvent">The entity death event</param>
  Line 206: // Check if a player entity died
  Line 213: // Check if a critical entity (like base/objective) died
  Line 392: /// Determines if the specified entity is a player entity.
  Line 393: /// P11-04-10-D: Checks entity type or components to identify players.
  Line 395: /// <param name="entityId">The entity to check</param>
  Line 396: /// <returns>True if the entity is a player, false otherwise</returns>
  Line 399: // Check if entity has player-specific components or tags
  Line 405: /// Determines if the specified entity is a critical entity.
  Line 408: /// <param name="entityId">The entity to check</param>
  Line 409: /// <returns>True if the entity is critical, false otherwise</returns>
  Line 412: // Check if entity has critical/base components or tags
  Line 413: // This is a simplified implementation - adjust based on your entity system
- File: .\Engine\Systems\Gameplay\InventorySystem.cs
  Line 20: private readonly Dictionary<Entity, InventoryComponent> _inventoryComponents;
  Line 25: /// <param name="entityManager">Entity manager for component access</param>
  Line 33: _inventoryComponents = new Dictionary<Entity, InventoryComponent>();
  Line 41: /// Gets the inventory component for an entity.
  Line 43: /// <param name="entity">Entity to get inventory for</param>
  Line 45: public InventoryComponent GetInventory(Entity entity)
  Line 47: if (entity == null) return null;
  Line 49: if (_inventoryComponents.TryGetValue(entity, out var component))
  Line 52: // Try to get component from entity manager
  Line 53: component = _entityManager.GetComponent<InventoryComponent>(entity);
  Line 56: _inventoryComponents[entity] = component;
  Line 63: /// Adds an inventory component to an entity.
  Line 65: /// <param name="entity">Entity to add inventory to</param>
  Line 68: public InventoryComponent AddInventoryToEntity(Entity entity, int maxSlots = 20)
  Line 70: if (entity == null) throw new ArgumentNullException(nameof(entity));
  Line 73: _entityManager.AddComponent(entity, inventory);
  Line 74: _inventoryComponents[entity] = inventory;
  Line 87: var entity = _entityManager.GetEntity(pickupEvent.EntityId);
  Line 88: if (entity == null)
  Line 90: Console.WriteLine($"[InventorySystem] Entity not found for pickup: {pickupEvent.EntityId}");
  Line 94: var inventory = GetInventory(entity);
  Line 97: Console.WriteLine($"[InventorySystem] Entity {pickupEvent.EntityId} has no inventory component");
  Line 133: Console.WriteLine($"[InventorySystem] Entity {pickupEvent.EntityId} picked up {addResult.AddedQuantity}x {pickupEvent.ItemDefinitionId}");
  Line 154: var entity = _entityManager.GetEntity(useEvent.EntityId);
  Line 155: if (entity == null)
  Line 157: Console.WriteLine($"[InventorySystem] Entity not found for item use: {useEvent.EntityId}");
  Line 161: var inventory = GetInventory(entity);
  Line 164: Console.WriteLine($"[InventorySystem] Entity {useEvent.EntityId} has no inventory component");
  Line 168: // Check if entity has the item
  Line 171: Console.WriteLine($"[InventorySystem] Entity {useEvent.EntityId} does not have {useEvent.Quantity}x {useEvent.ItemDefinitionId}");
  Line 205: Console.WriteLine($"[InventorySystem] Entity {useEvent.EntityId} used {removeResult.RemovedQuantity}x {useEvent.ItemDefinitionId}");
  Line 222: /// Drops an item from an entity's inventory.
  Line 224: /// <param name="entityId">ID of the entity dropping the item</param>
  Line 233: var entity = _entityManager.GetEntity(entityId);
  Line 234: if (entity == null) return false;
  Line 236: var inventory = GetInventory(entity);
  Line 278: // TODO: Create dropped item entity in the world
  Line 279: // This would involve creating a new entity with position components
  Line 280: Console.WriteLine($"[InventorySystem] Entity {entityId} dropped {itemToDrop.Quantity}x {itemDefinitionId}");
  Line 307: /// <param name="fromEntityId">ID of the source entity</param>
  Line 308: /// <param name="toEntityId">ID of the target entity</param>
  Line 329: Console.WriteLine($"[InventorySystem] Source entity {fromEntityId} doesn't have enough {itemDefinitionId}");
- File: .\Engine\Systems\Gameplay\KillAttributionSystem.cs
  Line 33: /// <param name="entityManager">Entity manager for component access</param>
  Line 71: /// <param name="deathEvent">The entity death event containing death information</param>
  Line 82: DebugLog($"KillAttributionSystem: Processing attribution for Entity {deathEvent.EntityId}");
  Line 96: DebugLog($"KillAttributionSystem: Failed to process attribution for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 117: DebugLog($"KillAttributionSystem: Environmental death detected for Entity {deathEvent.EntityId}");
  Line 133: DebugLog($"KillAttributionSystem: Self-inflicted death detected for Entity {deathEvent.EntityId}");
  Line 150: /// Determines the team for an entity based on its ID and components.
  Line 152: /// <param name="entityId">The ID of the entity to determine team for</param>
  Line 153: /// <returns>Team affiliation of the entity</returns>
  Line 157: // For this implementation, we'll use heuristics based on entity ID patterns
  Line 255: // In a full implementation, this would use an entity value system
- File: .\Engine\Systems\Gameplay\MetaProgressionSystem.cs
  Line 205: // Award XP based on entity type and death type
  Line 246: /// <param name="entityType">Type of entity killed</param>
  Line 251: // Base XP values by entity type
- File: .\Engine\Systems\Gameplay\PathfindingSystem.cs
  Line 8: Manages entity navigation paths and produces movement vectors
  Line 12: - Outputs movement vectors and MovementState per entity.
  Line 24: /// Describes the movement state of a navigating entity.
  Line 50: /// Per-entity movement output produced by PathfindingSystem each frame.
  Line 68: /// Internal tracking data for a navigating entity.
  Line 93: /// Manages entity navigation paths and produces movement vectors.
  Line 103: /// Read-only snapshot of movement outputs per entity, keyed by entity ID.
  Line 125: /// Registers a navigating entity with its starting position and speed.
  Line 140: /// Removes an entity from the pathfinding system.
  Line 149: /// Sets a navigation path (list of waypoints) for an entity.
  Line 150: /// The entity will begin moving toward the first waypoint on the next Update.
  Line 234: /// Retrieves the movement output for a specific entity.
  Line 235: /// Returns null if the entity is not registered.
- File: .\Engine\Systems\Gameplay\PickupSystem.cs
  Line 5: Features: Event subscription, pickup detection, effect application, entity removal, audit logging.
  Line 69: /// of a pickup item, applies pickup effect (e.g., increase score, grant power-up), removes pickup entity
  Line 103: /// <param name="entityManager">Entity manager for component access</param>
  Line 186: // P11-04-05-C: Removes pickup entity from the world after collection
  Line 234: DebugLog($"PickupSystem: Pickup entity {pickupEntityId} has no PickupComponent");
  Line 359: /// P11-04-05-C: Removes pickup entity from the world after collection.
  Line 366: DebugLog($"PickupSystem: Removed pickup entity {pickupEntityId} from world");
  Line 370: DebugLog($"PickupSystem: Failed to remove pickup entity - {ex.Message}");
  Line 457: /// - Tracks pickup source entity for audit and gameplay purposes
- File: .\Engine\Systems\Gameplay\ResourceSystem.cs
  Line 23: /// <param name="entityManager">Entity manager for component access</param>
  Line 33: /// Gets the current amount of a specific resource for an entity.
  Line 35: /// <param name="entityId">ID of the entity</param>
  Line 47: /// Gets all resources for an entity.
  Line 49: /// <param name="entityId">ID of the entity</param>
  Line 59: /// Adds resources to an entity.
  Line 61: /// <param name="entityId">ID of the entity to receive resources</param>
  Line 74: var entity = _entityManager.GetEntity(entityId);
  Line 75: if (entity == null)
  Line 77: Console.WriteLine($"[ResourceSystem] Entity not found: {entityId}");
  Line 103: Console.WriteLine($"[ResourceSystem] Added {amount} {resourceType} to entity {entityId} (total: {newAmount})");
  Line 108: /// Spends/Removes resources from an entity.
  Line 110: /// <param name="entityId">ID of the entity to spend resources from</param>
  Line 123: var entity = _entityManager.GetEntity(entityId);
  Line 124: if (entity == null)
  Line 126: Console.WriteLine($"[ResourceSystem] Entity not found: {entityId}");
  Line 132: Console.WriteLine($"[ResourceSystem] Entity {entityId} has no resources");
  Line 140: Console.WriteLine($"[ResourceSystem] Entity {entityId} doesn't have enough {resourceType} (has: {currentAmount}, needs: {amount})");
  Line 168: Console.WriteLine($"[ResourceSystem] Spent {amount} {resourceType} from entity {entityId} (remaining: {newAmount})");
  Line 173: /// Checks if an entity has enough of a specific resource.
  Line 175: /// <param name="entityId">ID of the entity to check</param>
  Line 178: /// <returns>True if the entity has enough resources</returns>
  Line 187: /// Sets the resource amount for an entity to a specific value.
  Line 189: /// <param name="entityId">ID of the entity</param>
  Line 202: var entity = _entityManager.GetEntity(entityId);
  Line 203: if (entity == null)
  Line 205: Console.WriteLine($"[ResourceSystem] Entity not found: {entityId}");
  Line 239: Console.WriteLine($"[ResourceSystem] Set {resourceType} for entity {entityId} to {amount} (was: {oldAmount})");
  Line 246: /// <param name="fromEntityId">ID of the source entity</param>
  Line 247: /// <param name="toEntityId">ID of the target entity</param>
  Line 258: Console.WriteLine($"[ResourceSystem] Transfer failed: source entity {fromEntityId} doesn't have enough {resourceType}");
  Line 276: Console.WriteLine($"[ResourceSystem] Transferred {amount} {resourceType} from entity {fromEntityId} to {toEntityId}");
  Line 281: /// Removes all resources for an entity.
  Line 283: /// <param name="entityId">ID of the entity to clear resources for</param>
  Line 306: Console.WriteLine($"[ResourceSystem] Cleared all resources for entity {entityId}");
  Line 347: /// <param name="entityManager">Entity manager for validation</param>
  Line 404: /// Resource data for a single entity.
- File: .\Engine\Systems\Gameplay\RespawnSystem.cs
  Line 4: Purpose: System for handling entity respawn logic and timing.
  Line 5: Features: Entity death subscription, respawn delay management, component reset.
  Line 21: /// System for handling entity respawn logic and timing.
  Line 51: /// <param name="entityManager">Entity manager for component access</param>
  Line 59: // Subscribe to entity death events
  Line 96: /// Manually requests a respawn for the specified entity.
  Line 99: /// <param name="entityId">The entity to respawn</param>
  Line 103: /// <returns>True if respawn request was queued, false if entity not found or already respawning</returns>
  Line 108: DebugLog($"RespawnSystem: Cannot respawn entity {entityId} - no HealthComponent found");
  Line 114: DebugLog($"RespawnSystem: Entity {entityId} already has pending respawn");
  Line 121: DebugLog($"RespawnSystem: Cannot respawn entity {entityId} - not dead");
  Line 137: DebugLog($"RespawnSystem: Queued manual respawn for entity {entityId} in {request.RespawnDelay}s");
  Line 143: /// Handles entity death events and queues auto-respawn if enabled.
  Line 146: /// <param name="deathEvent">The entity death event</param>
  Line 156: DebugLog($"RespawnSystem: Auto-respawn disabled for entity {deathEvent.EntityId}");
  Line 162: DebugLog($"RespawnSystem: Entity {deathEvent.EntityId} already has pending respawn");
  Line 178: DebugLog($"RespawnSystem: Queued auto-respawn for entity {deathEvent.EntityId} in {request.RespawnDelay}s");
  Line 182: /// Executes the respawn for the specified entity.
  Line 185: /// <param name="entityId">The entity to respawn</param>
  Line 196: DebugLog($"RespawnSystem: Reset health for entity {entityId} to {request.HealthPercentage * 100}%");
  Line 204: DebugLog($"RespawnSystem: Reset position for entity {entityId} to {request.RespawnLocation}");
  Line 212: DebugLog($"RespawnSystem: Reset stats for entity {entityId}");
  Line 229: DebugLog($"RespawnSystem: Successfully respawned entity {entityId}");
  Line 233: DebugLog($"RespawnSystem: Failed to respawn entity {entityId} - {ex.Message}");
  Line 238: /// Determines if the specified entity is a player entity.
  Line 239: /// P11-04-10-B: Checks entity type or components to identify players.
  Line 241: /// <param name="entityId">The entity to check</param>
  Line 242: /// <returns>True if the entity is a player, false otherwise</returns>
  Line 245: // Check if entity has player-specific components or tags
  Line 246: // This is a simplified implementation - adjust based on your entity system
  Line 252: /// Gets the default respawn location for the specified entity.
  Line 253: /// P11-04-10-B: Returns entity's stored respawn position or a default location.
  Line 255: /// <param name="entityId">The entity to get respawn location for</param>
- File: .\Engine\Systems\Gameplay\RoundResetSystem.cs
  Line 66: /// <param name="entityManager">Entity manager for component access</param>
  Line 74: // Subscribe to entity death events to track enemy kills
  Line 161: /// Handles entity death events to track enemy kills and player deaths.
  Line 164: /// <param name="deathEvent">The entity death event</param>
  Line 412: /// Determines if the specified entity is an enemy entity.
  Line 413: /// P11-04-10-C: Checks entity type or components to identify enemies.
  Line 415: /// <param name="entityId">The entity to check</param>
  Line 416: /// <returns>True if the entity is an enemy, false otherwise</returns>
  Line 419: // Check if entity has enemy-specific components or tags
  Line 420: // This is a simplified implementation - adjust based on your entity system
  Line 425: /// Determines if the specified entity is a player entity.
  Line 426: /// P11-04-10-C: Checks entity type or components to identify players.
  Line 428: /// <param name="entityId">The entity to check</param>
  Line 429: /// <returns>True if the entity is a player, false otherwise</returns>
  Line 432: // Check if entity has player-specific components or tags
  Line 433: // This is a simplified implementation - adjust based on your entity system
- File: .\Engine\Systems\Gameplay\ScoreSystem.cs
  Line 4: Purpose: System for awarding and managing score based on entity kills and death types.
  Line 5: Features: Score multipliers, per-entity score tracking, team-based scoring, audit logging.
  Line 7: P11-04-08-B: System subscribes to EntityDiedEvent, awards score to killer entity,
  Line 8: supports score multipliers based on DeathType or entity type, tracks total and per-entity score.
  Line 19: /// System responsible for awarding and managing game scores based on entity kills.
  Line 36: // Score multipliers by entity type (could be extended with entity type system)
  Line 47: /// <param name="entityManager">Entity manager for component access</param>
  Line 84: /// Handles EntityDiedEvent by awarding score to the killer entity.
  Line 86: /// <param name="deathEvent">The entity death event containing death information</param>
  Line 97: DebugLog($"ScoreSystem: Processing score for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 102: DebugLog($"ScoreSystem: No score awarded - Entity {deathEvent.EntityId} died from environmental causes");
  Line 110: DebugLog($"ScoreSystem: Awarded {scoreAwarded} score to Entity {deathEvent.KillerEntityId}");
  Line 114: DebugLog($"ScoreSystem: Failed to process score for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 120: /// Applies multipliers based on death type and entity characteristics.
  Line 126: // Base score value (could be enhanced with entity type system)
  Line 132: // Apply entity type multiplier (if available)
  Line 143: /// Gets the base score value for an entity.
  Line 144: /// In a full implementation, this would use an entity type system.
  Line 146: /// <param name="entityId">The ID of the entity</param>
  Line 150: // For this implementation, we'll use a simple heuristic based on entity ID
  Line 153: // Example scoring based on entity ID patterns
  Line 187: /// Gets the score multiplier for an entity type.
  Line 189: /// <param name="entityId">The ID of the entity</param>
  Line 190: /// <returns>Entity type multiplier</returns>
  Line 213: /// Awards score to an entity and updates tracking.
  Line 215: /// <param name="entityId">The entity to award score to</param>
  Line 220: // Update per-entity score
  Line 238: /// Gets the score for a specific entity.
  Line 240: /// <param name="entityId">The entity ID to query</param>
  Line 241: /// <returns>Score for the specified entity</returns>
  Line 304: /// <param name="entityId">The entity that received score</param>
  Line 309: string logMessage = $"SCORE_AWARD: Entity={entityId}, Score={score}, " +
  Line 373: /// Gets or sets the average score per entity.
- File: .\Engine\Systems\Gameplay\StatsTrackerSystem.cs
  Line 4: Purpose: System for tracking and updating entity statistics including kills, deaths, and kill streaks.
  Line 5: Features: Per-entity stat tracking, kill streak management, death type categorization, audit logging.
  Line 7: P11-04-08-C: System subscribes to EntityDiedEvent, tracks per-entity stats
  Line 20: /// System responsible for tracking and updating entity statistics.
  Line 21: /// P11-04-08-C: Subscribes to EntityDiedEvent and manages per-entity stats.
  Line 36: /// <param name="entityManager">Entity manager for component access</param>
  Line 74: /// <param name="deathEvent">The entity death event containing death information</param>
  Line 85: DebugLog($"StatsTrackerSystem: Processing stats for Entity {deathEvent.EntityId}, DeathType: {deathEvent.DeathType}");
  Line 100: DebugLog($"StatsTrackerSystem: Failed to process stats for Entity {deathEvent.EntityId} - {ex.Message}");
  Line 105: /// Updates death statistics for the victim entity.
  Line 107: /// <param name="victimEntityId">The ID of the entity that died</param>
  Line 115: DebugLog($"StatsTrackerSystem: Updated death stats for Entity {victimEntityId}");
  Line 119: /// Updates kill statistics for the killer entity.
  Line 121: /// <param name="killerEntityId">The ID of the entity that scored the kill</param>
  Line 130: DebugLog($"StatsTrackerSystem: Updated kill stats for Entity {killerEntityId}, DeathType: {deathType}");
  Line 134: /// Gets or creates a StatsComponent for an entity.
  Line 135: /// Ensures every tracked entity has a stats component.
  Line 137: /// <param name="entityId">The ID of the entity</param>
  Line 138: /// <returns>StatsComponent for the entity</returns>
  Line 145: // Create new stats component if entity doesn't have one
  Line 148: DebugLog($"StatsTrackerSystem: Created StatsComponent for Entity {entityId}");
  Line 155: /// Gets statistics for a specific entity.
  Line 157: /// <param name="entityId">The entity ID to query</param>
  Line 158: /// <returns>StatsComponent for the entity, or null if not found</returns>
  Line 165: /// Resets statistics for a specific entity.
  Line 167: /// <param name="entityId">The entity ID to reset</param>
  Line 174: DebugLog($"StatsTrackerSystem: Reset stats for Entity {entityId}");
  Line 178: DebugLog($"StatsTrackerSystem: Cannot reset stats - Entity {entityId} has no StatsComponent");
  Line 183: /// Resets kill streak for a specific entity.
  Line 185: /// <param name="entityId">The entity ID to reset streak for</param>
  Line 192: DebugLog($"StatsTrackerSystem: Reset kill streak for Entity {entityId}");
  Line 196: DebugLog($"StatsTrackerSystem: Cannot reset kill streak - Entity {entityId} has no StatsComponent");
  Line 201: /// Resets all entity statistics.
  Line 209: foreach (var entity in allEntities)
  Line 211: var statsComponent = _entityManager.GetComponent<StatsComponent>(entity);
  Line 226: /// <returns>Array of entity IDs sorted by kills (descending)</returns>
  Line 232: foreach (var entity in _entityManager.Entities)
  Line 234: var stats = _entityManager.GetComponent<StatsComponent>(entity);
  Line 237: entityStats.Add((entity, stats.Kills));
  Line 253: /// <returns>Array of entity IDs with active streaks</returns>
  Line 258: foreach (var entity in _entityManager.Entities)
  Line 260: var stats = _entityManager.GetComponent<StatsComponent>(entity);
  Line 263: entitiesWithStreaks.Add(entity);
  Line 279: foreach (var entity in _entityManager.Entities)
  Line 281: var stats = _entityManager.GetComponent<StatsComponent>(entity);
  Line 291: DebugLog($"StatsTrackerSystem: Kill streak timeout for Entity {entity} (streak was {oldStreak})");
  Line 309: foreach (var entity in allEntities)
  Line 311: var stats = _entityManager.GetComponent<StatsComponent>(entity);
  Line 396: /// Gets or sets the average kills per tracked entity.
  Line 411: $"Avg Kills/Entity: {AverageKillsPerEntity:F2}";
- File: .\Engine\Systems\Gameplay\TowerSystem.cs
  Line 210: // Add to entity manager for rendering/physics integration
  Line 211: var entity = new Entity(_nextTowerId++, position);
  Line 212: entity.Scale = definition.Size;
  Line 213: _entityManager.AddEntity(entity);
  Line 277: // Remove from entity manager
  Line 278: var entities = _entityManager.GetEntities<Entity>()
  Line 282: foreach (var entity in entities)
  Line 284: _entityManager.RemoveEntity(entity);
- File: .\Engine\Systems\Gameplay\ZoneTriggerSystem.cs
  Line 104: /// <param name="entityManager">Entity manager for component access</param>
  Line 217: // Check if entity was actually in this zone
  Line 241: /// Checks if an entity is a zone entity.
  Line 251: DebugLog($"ZoneTriggerSystem: Failed to check if entity is zone - {ex.Message}");
  Line 257: /// Gets the zone type for a zone entity.
  Line 306: /// Calculates how long an entity was in a zone.
  Line 311: // In a full implementation, we'd track entry timestamps per entity-zone pair
  Line 316: /// Tracks when an entity enters a zone.
  Line 329: /// Checks if an entity is currently in a specific zone.
  Line 337: /// Updates tracking when an entity exits a zone.
  Line 407: var message = $"ZONE ENTERED: Entity {result.SubjectEntityId} entered {result.ZoneType} zone {result.ZoneEntityId} at {result.Timestamp:HH:mm:ss.fff}";
  Line 419: var message = $"ZONE EXITED: Entity {result.SubjectEntityId} exited {result.ZoneType} zone {result.ZoneEntityId} at {result.Timestamp:HH:mm:ss.fff}{durationStr}";
  Line 594: var message = $"ZONE ENTERED: Entity {result.SubjectEntityId} entered {result.ZoneType} zone {result.ZoneEntityId} at {result.Timestamp:HH:mm:ss.fff}";
  Line 607: var message = $"ZONE EXITED: Entity {result.SubjectEntityId} exited {result.ZoneType} zone {result.ZoneEntityId} at {result.Timestamp:HH:mm:ss.fff}{durationStr}";
- File: .\Engine\Systems\Gameplay\Interaction\InteractionSystem.cs
  Line 143: // In full implementation, this would query the entity system for interactable components
- File: .\Engine\Systems\Hazards\Analytics\HazardAnalyticsValidator.cs
  Line 58: result.Errors.Add($"Kill event for entity {killEvent.VictimEntityId} has no attribution record");
- File: .\Engine\Systems\Hazards\Core\HazardEffect.cs
  Line 144: /// Applies all effects to a target entity
  Line 146: /// <param name="target">Entity to apply effects to</param>
  Line 161: /// <param name="target">Entity to check</param>
  Line 162: /// <returns>True if entity can be affected</returns>
  Line 194: /// <param name="target">Entity to affect</param>
  Line 227: // Implementation depends on entity system
  Line 236: // Implementation depends on entity system
  Line 245: // Implementation depends on entity system
  Line 254: // Implementation depends on entity system
  Line 263: // Implementation depends on entity system
  Line 268: /// Checks if an entity has a specific tag
  Line 272: // Implementation depends on entity tagging system
- File: .\Engine\Systems\Hazards\Core\HazardManager.cs
  Line 808: /// <param name="victimEntityId">ID of the entity that was killed</param>
  Line 809: /// <param name="victimType">Type of the entity that was killed</param>
  Line 1577: /// <param name="victimEntityId">ID of the entity that was killed</param>
  Line 3234: /// <param name="creator">Entity that created the hazard</param>
- File: .\Engine\Systems\Hazards\Core\HazardZone.cs
  Line 110: foreach (var entity in entitiesInRadius)
  Line 112: Effect.Apply(entity);
- File: .\Engine\Systems\Hazards\Core\StandardHazardZone.cs
  Line 21: /// Note: This is a placeholder implementation - actual entity detection would depend on the game's entity system
  Line 27: // In a real implementation, this would query the entity system for entities within radius
- File: .\Engine\Systems\Hazards\Nuke\NukeBlast.cs
  Line 95: foreach (var entity in entitiesInRadius)
  Line 98: Effect.Apply(entity);
  Line 101: RecordKillAttribution(entity, "NukeBlast");
  Line 114: /// <param name="entity">Entity that was affected</param>
  Line 116: private void RecordKillAttribution(object entity, string killReason)
  Line 126: int entityId = GetEntityId(entity);
  Line 127: string entityType = GetEntityType(entity);
  Line 141: /// P11-07-11: Gets entity ID for kill attribution (placeholder implementation)
  Line 143: private int GetEntityId(object entity)
  Line 145: // Placeholder implementation - would depend on entity system
  Line 146: return entity.GetHashCode();
  Line 150: /// P11-07-11: Gets entity type for kill attribution (placeholder implementation)
  Line 152: private string GetEntityType(object entity)
  Line 154: // Placeholder implementation - would depend on entity system
  Line 155: return entity.GetType().Name;
  Line 176: // In a real implementation, this would query the entity system for entities within radius
- File: .\Engine\Systems\Hazards\Nuke\NukeCloud.cs
  Line 41: /// P11-07-09: Dictionary tracking radiation exposure per entity for stacking effects
  Line 227: foreach (var entity in entitiesInRadius)
  Line 229: int entityId = GetEntityId(entity);
  Line 230: Vector3 entityPosition = GetEntityPosition(entity);
  Line 254: ApplyImmediateEffects(entity, localIntensity);
  Line 257: OnEntityExposed(entity, timeScaledDamage, localIntensity);
  Line 265: /// P11-07-09: Called when an entity is exposed to radiation with enhanced parameters
  Line 268: /// <param name="entity">Entity that was exposed</param>
  Line 270: /// <param name="intensity">Local radiation intensity at entity position</param>
  Line 271: protected virtual void OnEntityExposed(object entity, float damage, float intensity)
  Line 274: Debug.Log($"NukeCloud: Entity exposed to {damage:F1} radiation damage at intensity {intensity:F2}");
  Line 286: foreach (var entity in entitiesInRadius)
  Line 288: if (entityExposure.ContainsKey(GetEntityId(entity))
  Line 292: .WithDamage(accumulatedDamage * GetEntityExposureRatio(GetEntityId(entity))
  Line 295: damageEffect.Apply(entity);
  Line 298: RecordKillAttribution(entity, "Radiation_Damage");
  Line 309: /// <param name="entity">Entity that was affected</param>
  Line 311: private void RecordKillAttribution(object entity, string killReason)
  Line 321: int entityId = GetEntityId(entity);
  Line 322: string entityType = GetEntityType(entity);
  Line 336: /// P11-07-11: Gets entity type for kill attribution (placeholder implementation)
  Line 338: private string GetEntityType(object entity)
  Line 340: // Placeholder implementation - would depend on entity system
  Line 341: return entity.GetType().Name;
  Line 355: /// P11-07-09: Updates entity exposure tracking for stacking effects
  Line 370: /// P11-07-09: Gets stacking multiplier based on entity exposure
  Line 387: /// P11-07-09: Gets entity's share of accumulated damage based on exposure
  Line 406: private void ApplyImmediateEffects(object entity, float intensity)
  Line 413: slowEffect.Apply(entity);
  Line 417: /// P11-07-09: Gets entity ID for tracking (placeholder implementation)
  Line 419: private int GetEntityId(object entity)
  Line 421: // Placeholder implementation - would depend on entity system
  Line 422: return entity.GetHashCode();
  Line 426: /// P11-07-09: Gets entity position for distance calculations (placeholder implementation)
  Line 428: private Vector3 GetEntityPosition(object entity)
  Line 430: // Placeholder implementation - would depend on entity system
  Line 553: // In a real implementation, this would query the entity system for entities within radius
- File: .\Engine\Systems\Hazards\Nuke\RadiationExposure.cs
  Line 8: /// P11-07-09: Tracks radiation exposure data for a single entity
  Line 14: /// Total accumulated radiation damage for this entity
- File: .\Engine\Systems\Persistence\LoadSystem.cs
  Line 5: Features: World state restoration, entity recreation, event publishing.
  Line 47: /// <param name="entityManager">Entity manager for component access</param>
  Line 218: DebugLog("LoadSystem: Failed to find or create player entity");
  Line 234: DebugLog($"LoadSystem: Player state loaded successfully for entity {playerEntity}");
  Line 248: /// <param name="playerEntity">The player entity</param>
  Line 264: DebugLog("LoadSystem: Warning - Player entity missing TransformComponent");
  Line 272: /// <param name="playerEntity">The player entity</param>
  Line 288: DebugLog("LoadSystem: Warning - Player entity missing HealthComponent");
  Line 296: /// <param name="playerEntity">The player entity</param>
  Line 308: DebugLog("LoadSystem: Warning - Player entity missing StatsComponent");
  Line 316: /// <param name="playerEntity">The player entity</param>
  Line 518: /// <returns>True if persistent entity loading was successful, false otherwise</returns>
  Line 539: DebugLog($"LoadSystem: Persistent entity loading failed - {ex.Message}");
  Line 545: /// Loads a single persistent entity from save data.
  Line 546: /// P11-04-11-E: Creates entity and applies saved state.
  Line 548: /// <param name="entityData">The entity data to load</param>
  Line 549: /// <returns>True if entity loading was successful, false otherwise</returns>
  Line 556: DebugLog($"LoadSystem: Skipping invalid entity data - {entityData.EntityId}");
  Line 560: // Create entity based on type
  Line 561: var entity = CreateEntity(entityData);
  Line 562: if (entity == null)
  Line 564: DebugLog($"LoadSystem: Failed to create entity - {entityData.EntityId}");
  Line 568: // Apply entity data
  Line 569: ApplyEntityData(entity, entityData);
  Line 571: DebugLog($"LoadSystem: Loaded persistent entity - {entityData.EntityType}:{entityData.EntityId}");
  Line 576: DebugLog($"LoadSystem: Failed to load entity {entityData.EntityId} - {ex.Message}");
  Line 582: /// Creates an entity based on entity data.
  Line 583: /// P11-04-11-E: Instantiates entity of appropriate type.
  Line 585: /// <param name="entityData">The entity data describing the entity to create</param>
  Line 586: /// <returns>Created entity, or null if creation failed</returns>
  Line 591: // This would use the entity creation system
  Line 593: DebugLog($"LoadSystem: Would create {entityData.EntityType} entity with ID {entityData.EntityId}");
  Line 598: DebugLog($"LoadSystem: Entity creation failed for {entityData.EntityId} - {ex.Message}");
  Line 604: /// Applies saved data to an entity.
  Line 605: /// P11-04-11-E: Restores entity components and state.
  Line 607: /// <param name="entity">The entity to apply data to</param>
  Line 608: /// <param name="entityData">The entity data to apply</param>
  Line 609: private void ApplyEntityData(object entity, PersistentEntityData entityData)
  Line 614: ApplyEntityTransform(entity, entityData);
  Line 617: ApplyEntityHealth(entity, entityData);
  Line 619: // Apply entity-specific state
  Line 620: ApplyEntitySpecificState(entity, entityData);
  Line 622: DebugLog($"LoadSystem: Applied data to entity {entityData.EntityId}");
  Line 626: DebugLog($"LoadSystem: Failed to apply data to entity {entityData.EntityId} - {ex.Message}");
  Line 631: /// Applies transform data to an entity.
  Line 632: /// P11-04-11-E: Restores entity position, rotation, and scale.
  Line 634: /// <param name="entity">The entity to apply transform to</param>
  Line 635: /// <param name="entityData">The entity data containing transform information</param>
  Line 636: private void ApplyEntityTransform(object entity, PersistentEntityData entityData)
  Line 638: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 652: _entityManager.AddComponent(entity, newTransform);
  Line 653: DebugLog($"LoadSystem: Created TransformComponent for entity {entityData.EntityId}");
  Line 658: /// Applies health data to an entity.
  Line 659: /// P11-04-11-E: Restores entity current health and max health.
  Line 661: /// <param name="entity">The entity to apply health to</param>
  Line 662: /// <param name="entityData">The entity data containing health information</param>
  Line 663: private void ApplyEntityHealth(object entity, PersistentEntityData entityData)
  Line 665: var health = _entityManager.GetComponent<HealthComponent>(entity);
  Line 679: _entityManager.AddComponent(entity, newHealth);
  Line 680: DebugLog($"LoadSystem: Created HealthComponent for entity {entityData.EntityId}");
  Line 685: /// Applies entity-specific state data.
  Line 686: /// P11-04-11-E: Restores custom state based on entity type.
  Line 688: /// <param name="entity">The entity to apply state to</param>
  Line 689: /// <param name="entityData">The entity data containing state information</param>
  Line 690: private void ApplyEntitySpecificState(object entity, PersistentEntityData entityData)
  Line 804: // Find existing player entity
  Line 806: foreach (var entity in entities)
  Line 808: if (IsPlayerEntity(entity))
  Line 809: return entity;
  Line 812: // Create new player entity if none exists
  Line 813: DebugLog("LoadSystem: Would create new player entity");
  Line 817: private bool IsPlayerEntity(object entity)
  Line 819: return _entityManager.HasComponent<PlayerComponent>(entity) ||
  Line 820: _entityManager.HasComponent<StatsComponent>(entity);
- File: .\Engine\Systems\Persistence\SaveGameData.cs
  Line 208: foreach (var entity in PersistentEntities)
  Line 210: if (entity == null || !entity.IsValid())
  Line 435: /// Persistent entity data for save/load operations.
  Line 442: /// Gets or sets the entity identifier.
  Line 447: /// Gets or sets the entity type identifier.
  Line 452: /// Gets or sets the entity position.
  Line 457: /// Gets or sets the entity rotation in radians.
  Line 462: /// Gets or sets the entity scale.
  Line 467: /// Gets or sets the entity's current health.
  Line 472: /// Gets or sets the entity's maximum health.
  Line 477: /// Gets or sets entity-specific state data.
  Line 490: /// Validates the persistent entity data.
  Line 501: /// Returns a string representation of the persistent entity.
- File: .\Engine\Systems\Persistence\SaveManager.cs
  Line 49: /// <param name="entityManager">Entity manager for component access</param>
  Line 323: // Collect player entity data
  Line 331: DebugLog("SaveManager: Warning - No player entity found");
  Line 363: /// <param name="playerEntity">The player entity</param>
  Line 485: foreach (var entity in entities)
  Line 487: if (ShouldPersistEntity(entity))
  Line 489: var entityData = CollectEntityData(entity);
  Line 501: /// Determines if an entity should be persisted in save data.
  Line 504: /// <param name="entity">The entity to check</param>
  Line 505: /// <returns>True if entity should be persisted, false otherwise</returns>
  Line 506: private bool ShouldPersistEntity(object entity)
  Line 508: // Don't persist player entity (handled separately)
  Line 509: if (IsPlayerEntity(entity))
  Line 513: return _entityManager.HasComponent<EnemyComponent>(entity) ||
  Line 514: _entityManager.HasComponent<SpawnerComponent>(entity) ||
  Line 515: _entityManager.HasComponent<PersistentComponent>(entity);
  Line 519: /// Collects data for a single entity.
  Line 520: /// P11-04-11-B: Extracts entity position, health, and state data.
  Line 522: /// <param name="entity">The entity to collect data for</param>
  Line 523: /// <returns>PersistentEntityData for the entity</returns>
  Line 524: private PersistentEntityData CollectEntityData(object entity)
  Line 530: EntityId = entity.ToString() ?? "Unknown",
  Line 531: EntityType = GetEntityType(entity)
  Line 535: var transform = _entityManager.GetComponent<TransformComponent>(entity);
  Line 544: var health = _entityManager.GetComponent<HealthComponent>(entity);
  Line 551: // Collect entity-specific state data
  Line 552: CollectEntityStateData(entity, entityData);
  Line 558: DebugLog($"SaveManager: Failed to collect data for entity {entity} - {ex.Message}");
  Line 564: /// Collects entity-specific state data.
  Line 565: /// P11-04-11-B: Extracts custom state based on entity type.
  Line 567: /// <param name="entity">The entity</param>
  Line 568: /// <param name="entityData">The entity data to populate</param>
  Line 569: private void CollectEntityStateData(object entity, PersistentEntityData entityData)
  Line 574: if (_entityManager.HasComponent<EnemyComponent>(entity))
  Line 580: else if (_entityManager.HasComponent<SpawnerComponent>(entity))
  Line 625: /// <param name="playerEntity">The player entity</param>
  Line 703: var entity = CreateEntityFromData(entityData);
  Line 704: if (entity != null)
  Line 706: ApplyEntityData(entity, entityData);
  Line 807: foreach (var entity in entities)
  Line 809: if (IsPlayerEntity(entity))
  Line 810: return entity;
  Line 821: // Create new player entity if none exists
  Line 822: // This would use the entity creation system
  Line 823: DebugLog("SaveManager: Warning - No player entity found, would need to create one");
  Line 827: private bool IsPlayerEntity(object entity)
  Line 829: return _entityManager.HasComponent<PlayerComponent>(entity) ||
  Line 830: _entityManager.HasComponent<StatsComponent>(entity);
  Line 833: private string GetEntityType(object entity)
  Line 835: if (_entityManager.HasComponent<EnemyComponent>(entity))
  Line 837: if (_entityManager.HasComponent<SpawnerComponent>(entity))
  Line 839: if (_entityManager.HasComponent<PlayerComponent>(entity))
  Line 847: // Implementation would depend on entity management system
  Line 854: // Implementation would depend on entity creation system
  Line 855: DebugLog($"SaveManager: Would create entity of type {entityData.EntityType}");
  Line 859: private void ApplyEntityData(object entity, PersistentEntityData entityData)
  Line 861: // This would apply entity data to created entities
  Line 862: // Implementation would depend on entity system
  Line 863: DebugLog($"SaveManager: Would apply data to entity {entityData.EntityId}");
- File: .\Engine\Systems\Rendering\RenderSystem.cs
  Line 5: Features: Entity rendering, layer management, render context handling.
  Line 23: /// Manages render layers, entity sorting, and drawing operations.
  Line 29: private readonly Dictionary<int, List<Entity>> _renderLayers = new Dictionary<int, List<Entity>>();
  Line 45: /// <param name="entityManager">The entity manager for accessing entities.</param>
  Line 62: _renderLayers[i] = new List<Entity>();
  Line 92: _renderLayers[i] = new List<Entity>();
  Line 107: foreach (var entity in _renderLayers[layer])
  Line 109: RenderEntity(entity);
  Line 117: /// Renders all entities in the entity manager.
  Line 127: // Get all entities from the entity manager
  Line 131: foreach (var entity in entities)
  Line 133: var renderComponent = entity.GetComponent<RenderComponent>();
  Line 139: _renderLayers[layer] = new List<Entity>();
  Line 141: _renderLayers[layer].Add(entity);
  Line 147: /// Renders a specific entity.
  Line 149: /// <param name="entity">The entity to render.</param>
  Line 150: public void RenderEntity(Entity entity)
  Line 155: if (entity == null)
  Line 158: var renderComponent = entity.GetComponent<RenderComponent>();
  Line 162: var transformComponent = entity.GetComponent<TransformComponent>();
  Line 166: // Get entity position and size
- File: .\Engine\Systems\UI\HealthBarRenderer.cs
  Line 8: Supports configurable bar size, color, and offset. Clamps health bar width to entity's
  Line 44: /// Gets or sets the vertical offset above entity position for health bar rendering.
  Line 76: /// <param name="entityManager">Entity manager for component access</param>
  Line 101: foreach (var entity in entitiesWithHealth)
  Line 103: RenderEntityHealthBar(entity, context);
  Line 120: foreach (var entity in entities)
  Line 122: if (_entityManager.HasComponent<HealthComponent>(entity) &&
  Line 123: _entityManager.HasComponent<TransformComponent>(entity))
  Line 125: result.Add(entity);
  Line 133: /// Renders a health bar for a single entity.
  Line 136: private void RenderEntityHealthBar(object entity, IRenderContext context)
  Line 140: var healthComponent = _entityManager.GetComponent<HealthComponent>(entity);
  Line 141: var transformComponent = _entityManager.GetComponent<TransformComponent>(entity);
  Line 151: // Skip rendering if hiding full health and entity is healthy
  Line 175: DebugLog($"HealthBarRenderer: Rendered health bar for entity - Health: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth} ({healthRatio:P0})");
  Line 179: DebugLog($"HealthBarRenderer: Failed to render entity health bar - {ex.Message}");
- File: .\Engine\Systems\UI\InventoryPanelRenderer.cs
  Line 54: /// Shows the inventory panel for the specified entity.
  Line 56: /// <param name="entityId">Entity ID whose inventory to display</param>
  Line 66: DebugLog($"InventoryPanelRenderer: Showing inventory for entity {entityId}");
- File: .\Engine\Systems\UI\ScoreDisplaySystem.cs
  Line 89: /// <param name="entityManager">Entity manager for component access</param>
- File: .\Tools\AnimationSystemTest.cs
  Line 50: // Test 5: Basic entity setup
  Line 51: Console.WriteLine("Test 5: Setting up test entity...");
  Line 52: var entity = ecsWorld.CreateEntity();
  Line 53: entity.AddComponent(controller);
  Line 54: entity.AddComponent(stateMachine);
  Line 55: Console.WriteLine($"✓ Test entity created with ID: {entity.Id}");
  Line 69: var inspectionData = AnimationDebugTools.InspectEntity(entity);
  Line 70: Console.WriteLine($"✓ Entity inspection completed - Has controller: {inspectionData.HasAnimationController}");
  Line 79: entity.RemoveComponent<AnimationControllerComponent>();
  Line 80: entity.RemoveComponent<AnimationStateMachine>();
  Line 81: ecsWorld.DestroyEntity(entity);

---

### Type: RenderItem

Found definition(s):
- File: .\Engine\Systems\RenderQueue.cs
  Namespace: SASZombieAssaultTD.Engine

Referenced in:
- File: .\Engine\Rendering\RenderQueue.cs
  Line 14: public void Enqueue(Systems.RenderItem item)
  Line 24: public IReadOnlyList<Systems.RenderItem> Items => _queue.Items;
- File: .\Engine\Systems\RenderingSystem.cs
  Line 216: var renderItem = new RenderItem(
  Line 223: _renderQueue.Enqueue(renderItem);
  Line 252: /// P11-08-19: Simplified to work with RenderItem structure.
  Line 254: private void DrawRenderItem(IRenderContext context, RenderItem item)
- File: .\Engine\Systems\RenderQueue.cs
  Line 11: private readonly List<RenderItem> _items = new();
  Line 13: public void Enqueue(RenderItem item)
  Line 23: public IReadOnlyList<RenderItem> Items => _items;
  Line 56: public sealed class RenderItem
  Line 86: public RenderItem(int layer, string textureName, float x, float y)

---

### Type: AudioSettings

No definition found in project.

Referenced in:
- File: .\Engine\Audio\AudioEngine.cs
  Line 67: private AudioSettings _audioSettings;
  Line 304: _audioSettings = new AudioSettings();
  Line 838: public void SetAudioSettings(AudioSettings settings)

---

### Type: AssetManager

Found definition(s):
- File: .\Engine\Systems\Assets\AssetManager.cs
  Namespace: SASZombieAssaultTD.Engine.Assets

Referenced in:
- File: .\Engine\Components\SpriteComponent.cs
  Line 8: - Stores texture asset reference for AssetManager lookup
  Line 22: Texture asset IDs are resolved through the AssetManager system.
  Line 39: /// Texture or sprite asset ID for AssetManager lookup.
- File: .\Engine\Components\UIComponent.cs
  Line 20: /// Text or sprite asset ID for AssetManager lookup.
- File: .\Engine\Core\Managers\SystemManager.cs
  Line 192: typeof(AssetManager),
- File: .\Engine\Systems\ParticleSystem.cs
  Line 85: private readonly AssetManager _assetManager;
  Line 98: /// <param name="assetManager">Asset manager for particle textures</param>
  Line 100: public ParticleSystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 103: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
- File: .\Engine\Systems\RenderingSystem.cs
  Line 10: - Manages asset loading and caching through AssetManager
  Line 43: /// P11-03-01-B: Constructor accepts AssetManager, EntityManager, EventBus, and platform renderer interface.
  Line 72: private readonly AssetManager _assetManager;
  Line 92: /// <param name="assetManager">Asset manager for retrieving sprite/texture assets</param>
  Line 99: AssetManager assetManager,
  Line 106: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
  Line 144: /// - Retrieves sprite asset from AssetManager
  Line 207: // P11-03-02-C: Retrieve sprite asset from AssetManager
  Line 256: // Retrieve texture from AssetManager
- File: .\Engine\Systems\UISystem.cs
  Line 35: private readonly AssetManager _assetManager;
  Line 72: /// <param name="assetManager">Asset manager for UI assets</param>
  Line 74: public UISystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 77: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
  Line 357: // In a full implementation, this would load fonts from AssetManager
- File: .\Engine\Systems\Assets\AssetManager.cs
  Line 3: File:    AssetManager.cs
  Line 20: public class AssetManager
  Line 75: DebugLog("AssetManager: Starting initialization...");
  Line 81: DebugLog($"AssetManager: Initialized with {_assetMetadata.Count} registered assets");
  Line 85: DebugLog($"AssetManager: Initialization failed - {ex.Message}");
  Line 86: throw new InvalidOperationException("Failed to initialize AssetManager", ex);
  Line 95: DebugLog($"AssetManager: GetAsset failed - Not initialized (Key: {key})");
  Line 96: throw new InvalidOperationException("AssetManager not initialized");
  Line 101: DebugLog("AssetManager: GetAsset failed - Invalid key (null or empty)");
  Line 112: DebugLog($"AssetManager: Retrieved cached asset '{key}' as type {typeof(T).Name}");
  Line 117: DebugLog($"AssetManager: Type mismatch for asset '{key}' - Expected {typeof(T).Name}, got {asset.GetType().Name}");
  Line 123: DebugLog($"AssetManager: Loading asset '{key}' on demand");
  Line 133: DebugLog("AssetManager: PreloadAssets failed - Not initialized");
  Line 134: throw new InvalidOperationException("AssetManager not initialized");
  Line 139: DebugLog("AssetManager: PreloadAssets failed - Null keys collection");
  Line 144: DebugLog($"AssetManager: Preloading {keysList.Count} assets...");
  Line 155: DebugLog("AssetManager: Skipping null/empty key during preload");
  Line 163: DebugLog($"AssetManager: Asset '{key}' already loaded, skipping");
  Line 175: DebugLog($"AssetManager: Failed to preload asset '{key}': {ex.Message}");
  Line 179: DebugLog($"AssetManager: Preload complete - Success: {successCount}, Errors: {errorCount}");
  Line 187: DebugLog($"AssetManager: No metadata found for asset '{key}'");
  Line 194: DebugLog($"AssetManager: Asset validation failed for '{key}': {_validationErrors[key]}");
  Line 203: DebugLog($"AssetManager: Loaded asset '{key}' as type {loadedAsset.GetType().Name}");
  Line 211: DebugLog($"AssetManager: Type conversion failed for '{key}' - Expected {typeof(T).Name}, got {loadedAsset.GetType().Name}");
  Line 217: DebugLog($"AssetManager: Failed to load asset '{key}': {ex.Message}");
  Line 271: DebugLog($"AssetManager: Loading texture '{metadata.Key}' from '{metadata.Path}'");
  Line 279: DebugLog($"AssetManager: Loading sound effect '{metadata.Key}' from '{metadata.Path}'");
  Line 287: DebugLog($"AssetManager: Loading music track '{metadata.Key}' from '{metadata.Path}'");
  Line 295: DebugLog($"AssetManager: Loading JSON data '{metadata.Key}' from '{metadata.Path}'");
  Line 307: DebugLog($"AssetManager: Loading binary data '{metadata.Key}' from '{metadata.Path}'");
  Line 328: DebugLog($"AssetManager: Validating asset '{key}'");
  Line 377: DebugLog($"AssetManager: Asset '{key}' validation passed");
  Line 384: DebugLog($"AssetManager: Asset '{key}' validation failed with exception: {ex.Message}");
  Line 521: DebugLog("AssetManager: Loading asset metadata from AssetRegistry...");
  Line 537: DebugLog($"AssetManager: Registered asset '{asset.Key}' ({type}) from '{asset.Value}'");
  Line 541: DebugLog($"AssetManager: Failed to register asset '{asset.Key}': {ex.Message}");
  Line 545: DebugLog($"AssetManager: Loaded metadata for {loadedCount} assets");
  Line 634: DebugLog($"AssetManager: Disposed asset '{key}'");
  Line 638: DebugLog($"AssetManager: Failed to dispose asset '{key}': {ex.Message}");
  Line 646: DebugLog($"AssetManager: Unloaded asset '{key}'");
  Line 656: DebugLog("AssetManager: Unloading all assets...");
  Line 676: DebugLog($"AssetManager: Failed to dispose asset '{kvp.Key}': {ex.Message}");
  Line 684: DebugLog($"AssetManager: Unloaded all assets - Disposed: {disposedCount}, Errors: {errorCount}");
  Line 696: DebugLog("AssetManager: Starting shutdown...");
  Line 703: DebugLog("AssetManager: Shutdown complete");
- File: .\Engine\Systems\Gameplay\TowerSystem.cs
  Line 109: private readonly AssetManager _assetManager;
  Line 116: public TowerSystem(EntityManager entityManager, EventBus eventBus, AssetManager assetManager)
  Line 120: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
- File: .\Engine\Systems\UI\InventoryPanelRenderer.cs
  Line 28: private readonly AssetManager _assetManager;
  Line 43: /// <param name="assetManager">Asset manager for UI assets</param>
  Line 45: public InventoryPanelRenderer(AssetManager assetManager, TextRenderer textRenderer)
  Line 47: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
- File: .\Engine\Systems\UI\ItemTooltipRenderer.cs
  Line 27: private readonly AssetManager _assetManager;
  Line 42: /// <param name="assetManager">Asset manager for UI assets</param>
  Line 44: public ItemTooltipRenderer(AssetManager assetManager, TextRenderer textRenderer)
  Line 46: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));
- File: .\Engine\Systems\UI\ResourceDisplayRenderer.cs
  Line 27: private readonly AssetManager _assetManager;
  Line 42: /// <param name="assetManager">Asset manager for UI assets</param>
  Line 44: public ResourceDisplayRenderer(AssetManager assetManager, TextRenderer textRenderer)
  Line 46: _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));

---

### Type: ParticleSystem

Found definition(s):
- File: .\Engine\Systems\ParticleSystem.cs
  Namespace: SASZombieAssaultTD.Engine

Referenced in:
- File: .\Engine\Managers\EntityManager.cs
  Line 167: /// This is used by ParticleSystem for efficient entity filtering.
- File: .\Engine\Systems\ParticleSystem.cs
  Line 3: File:    ParticleSystem.cs
  Line 82: public class ParticleSystem
  Line 95: /// Creates a new ParticleSystem with required dependencies.
  Line 100: public ParticleSystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 106: DebugLog("ParticleSystem: Constructed with required dependencies");
  Line 119: DebugLog("ParticleSystem: Starting initialization...");
  Line 128: DebugLog("ParticleSystem: Initialization complete - Particle pool initialized with 200 particles");
  Line 132: DebugLog($"ParticleSystem: Initialization failed - {ex.Message}");
  Line 133: throw new InvalidOperationException("Failed to initialize ParticleSystem", ex);
  Line 149: DebugLog("ParticleSystem: Update failed - Not initialized");
  Line 172: DebugLog($"ParticleSystem: Update complete - Active particles: {_particles.Count}");
  Line 176: DebugLog($"ParticleSystem: Update failed - {ex.Message}");
  Line 280: /// - Maintains separation of concerns (ParticleSystem manages simulation, RenderingSystem handles drawing)
  Line 320: DebugLog($"ParticleSystem: Rendered {_particles.Count} particles");
  Line 324: DebugLog($"ParticleSystem: Render failed - {ex.Message}");
  Line 352: DebugLog($"ParticleSystem: SpawnEffect failed - Not initialized");
  Line 358: DebugLog($"ParticleSystem: SpawnEffect failed - Invalid effect name");
  Line 364: DebugLog($"ParticleSystem: Spawning effect '{effectName}' at position {position}");
  Line 372: DebugLog($"ParticleSystem: Effect '{effectName}' spawned successfully at position {position}");
  Line 376: DebugLog($"ParticleSystem: Failed to spawn effect '{effectName}' at position {position} - {ex.Message}");
  Line 576: DebugLog("ParticleSystem: Starting shutdown...");
  Line 582: DebugLog("ParticleSystem: Shutdown complete");
  Line 589: /// The ParticleSystem does NOT require any event subscriptions for its core functionality.
  Line 592: /// - ParticleSystem queries EntityManager for entities with required components
- File: .\Engine\Systems\RenderingSystem.cs
  Line 78: private ParticleSystem? _particleSystem;
  Line 89: /// P11-03-04-D: Accepts ParticleSystem for particle rendering integration.
  Line 96: /// <param name="particleSystem">Particle system for particle effects (optional)</param>
  Line 103: ParticleSystem? particleSystem = null,
  Line 110: _particleSystem = particleSystem;
  Line 148: /// P11-03-04-D: Also renders particles if ParticleSystem is available
  Line 169: // P11-03-04-D: Render particles if ParticleSystem is available
- File: .\Engine\Systems\Gameplay\DeathEffectSystem.cs
  Line 29: private readonly ParticleSystem _particleSystem;
  Line 42: /// <param name="particleSystem">Particle system for spawning visual effects</param>
  Line 47: ParticleSystem particleSystem,
  Line 52: _particleSystem = particleSystem ?? throw new ArgumentNullException(nameof(particleSystem));
- File: .\Engine\Systems\Hazards\Nuke\NukeCloud.cs
  Line 68: private ParticleSystem cloudParticles;
- File: .\Engine\Systems\Hazards\Visual\HazardVisualSystem.cs
  Line 355: var particleSystems = visualObject.GetComponentsInChildren<ParticleSystem>();
  Line 449: var particleSystems = instance.VisualObject.GetComponentsInChildren<ParticleSystem>();
  Line 1048: var mainParticleSystem = particleContainer.AddComponent<ParticleSystem>();
  Line 1072: colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
  Line 1075: instance.ParticleSystem = mainParticleSystem;
  Line 1266: var particleSystems = instance.VisualObject.GetComponentsInChildren<ParticleSystem>();
  Line 1324: var particleSystems = instance.VisualObject.GetComponentsInChildren<ParticleSystem>();
  Line 1854: public ParticleSystem ParticleSystem { get; set; }

---

### Type: UISystem

Found definition(s):
- File: .\Engine\Systems\UISystem.cs
  Namespace: SASZombieAssaultTD.Engine
- File: .\Engine\Systems\UI\UISystem.cs
  Namespace: SASZombieAssaultTD.Engine.UI

Referenced in:
- File: .\Engine\Managers\EntityManager.cs
  Line 187: /// This is used by UISystem for efficient entity filtering.
- File: .\Engine\Scenes\BaseScene.cs
  Line 11: using UISystem = SASZombieAssaultTD.Engine.UI.UISystem;
  Line 31: protected UISystem? UISystem => _gameRoot?.UISystem;
- File: .\Engine\State\StateMachineIntegration.cs
  Line 76: /// <param name="uiSystem">The UI system to integrate with.</param>
  Line 77: public static void IntegrateWithUISystem(StateMachine stateMachine, UISystem uiSystem)
  Line 81: if (uiSystem == null)
  Line 82: throw new ArgumentNullException(nameof(uiSystem));
- File: .\Engine\Systems\RenderingSystem.cs
  Line 81: private UISystem? _uiSystem;
  Line 90: /// P11-03-05-D: Accepts UISystem for UI rendering integration.
  Line 97: /// <param name="uiSystem">UI system for UI elements (optional)</param>
  Line 104: UISystem? uiSystem = null)
  Line 111: _uiSystem = uiSystem;
  Line 149: /// P11-03-05-D: Also renders UI elements if UISystem is available
- File: .\Engine\Systems\UISystem.cs
  Line 3: File:    UISystem.cs
  Line 32: public class UISystem
  Line 65: /// Creates a new UISystem with required dependencies.
  Line 74: public UISystem(EntityManager entityManager, AssetManager assetManager, EventBus eventBus)
  Line 103: DebugLog("UISystem: Constructed with all UI subsystems including game lifecycle UI, inventory/resource UI, and achievement/challenge UI");
  Line 117: DebugLog("UISystem: Starting initialization...");
  Line 123: DebugLog("UISystem: Initialization complete with all subsystems ready");
  Line 127: DebugLog($"UISystem: Initialization failed - {ex.Message}");
  Line 128: throw new InvalidOperationException("Failed to initialize UISystem", ex);
  Line 141: DebugLog("UISystem: Update failed - Not initialized");
  Line 153: DebugLog($"UISystem: Update failed - {ex.Message}");
  Line 190: DebugLog("UISystem: Updated all UI subsystems including game lifecycle UI, inventory/resource UI, and achievement/challenge UI");
  Line 194: DebugLog($"UISystem: Failed to update UI subsystems - {ex.Message}");
  Line 216: DebugLog($"UISystem: Render failed - {ex.Message}");
  Line 260: DebugLog("UISystem: Rendered all UI subsystems including game lifecycle UI, inventory/resource UI, and achievement/challenge UI");
  Line 264: DebugLog($"UISystem: Failed to render UI subsystems - {ex.Message}");
  Line 304: DebugLog($"UISystem: Failed to update UI entity - {ex.Message}");
  Line 348: DebugLog($"UISystem: Failed to render UI entity - {ex.Message}");
  Line 390: DebugLog($"UISystem: Showed game over screen (Victory: {wasVictory}, Score: {finalScore})");
  Line 394: DebugLog($"UISystem: Failed to show game over screen - {ex.Message}");
  Line 406: DebugLog("UISystem: Hid game over screen");
  Line 410: DebugLog($"UISystem: Failed to hide game over screen - {ex.Message}");
  Line 425: DebugLog($"UISystem: Showed round complete notification (Round: {roundNumber}, Score: {roundScore})");
  Line 429: DebugLog($"UISystem: Failed to show round complete notification - {ex.Message}");
  Line 441: DebugLog("UISystem: Hid round complete notification");
  Line 445: DebugLog($"UISystem: Failed to hide round complete notification - {ex.Message}");
  Line 459: DebugLog($"UISystem: Showed respawn countdown for entity {entityId} ({respawnTime}s)");
  Line 463: DebugLog($"UISystem: Failed to show respawn countdown - {ex.Message}");
  Line 476: DebugLog($"UISystem: Hid respawn countdown for entity {entityId}");
  Line 480: DebugLog($"UISystem: Failed to hide respawn countdown - {ex.Message}");
  Line 494: DebugLog($"UISystem: Updated respawn countdown for entity {entityId} ({remainingTime}s remaining)");
  Line 498: DebugLog($"UISystem: Failed to update respawn countdown - {ex.Message}");
  Line 513: DebugLog($"UISystem: Would show save confirmation dialog for slot {saveSlotId}");
  Line 514: DebugLog($"UISystem: Save summary: {saveSummary}");
  Line 524: DebugLog($"UISystem: Failed to show save confirmation dialog - {ex.Message}");
  Line 539: DebugLog($"UISystem: Would show load confirmation dialog for slot {saveSlotId}");
  Line 540: DebugLog($"UISystem: Load summary: {saveSummary}");
  Line 550: DebugLog($"UISystem: Failed to show load confirmation dialog - {ex.Message}");
  Line 565: DebugLog($"UISystem: Would show save slot selection dialog (Mode: {(isSaving ? "Save" : "Load")})");
  Line 566: DebugLog($"UISystem: Available saves: {availableSaves?.Count ?? 0}");
  Line 572: DebugLog($"UISystem:   - {save.GetDisplaySummary()}");
  Line 585: DebugLog($"UISystem: Failed to show save slot selection dialog - {ex.Message}");
  Line 601: DebugLog($"UISystem: Would show incompatible save version dialog");
  Line 602: DebugLog($"UISystem: Save version: {saveVersion}, Current version: {currentVersion}");
  Line 603: DebugLog($"UISystem: Error: {errorMessage}");
  Line 614: DebugLog($"UISystem: Failed to show incompatible save version dialog - {ex.Message}");
  Line 630: DebugLog($"UISystem: Would show {operation} progress dialog - {progress:P0} - {message}");
  Line 641: DebugLog($"UISystem: Failed to show save/load progress dialog - {ex.Message}");
  Line 654: DebugLog("UISystem: Would hide all save/load dialogs");
  Line 663: DebugLog($"UISystem: Failed to hide save/load dialogs - {ex.Message}");
  Line 678: DebugLog($"UISystem: Would show save completed notification for slot {saveSlotId}");
  Line 679: DebugLog($"UISystem: Save summary: {saveSummary}");
  Line 689: DebugLog($"UISystem: Failed to show save completed notification - {ex.Message}");
  Line 704: DebugLog($"UISystem: Would show load completed notification for slot {saveSlotId}");
  Line 705: DebugLog($"UISystem: Load summary: {loadSummary}");
  Line 715: DebugLog($"UISystem: Failed to show load completed notification - {ex.Message}");
  Line 729: DebugLog($"UISystem: Showed inventory panel for entity {entityId}");
  Line 733: DebugLog($"UISystem: Failed to show inventory panel - {ex.Message}");
  Line 745: DebugLog("UISystem: Hid inventory panel");
  Line 749: DebugLog($"UISystem: Failed to hide inventory panel - {ex.Message}");
  Line 762: DebugLog("UISystem: Updated inventory panel");
  Line 766: DebugLog($"UISystem: Failed to update inventory panel - {ex.Message}");
  Line 779: DebugLog("UISystem: Updated resource display");
  Line 783: DebugLog($"UISystem: Failed to update resource display - {ex.Message}");
  Line 797: DebugLog($"UISystem: Showed item tooltip at position {position}");
  Line 801: DebugLog($"UISystem: Failed to show item tooltip - {ex.Message}");
  Line 813: DebugLog("UISystem: Hid item tooltip");
  Line 817: DebugLog($"UISystem: Failed to hide item tooltip - {ex.Message}");
  Line 831: DebugLog("UISystem: Updated item tooltip");
  Line 835: DebugLog($"UISystem: Failed to update item tooltip - {ex.Message}");
  Line 848: DebugLog($"UISystem: Set resource display visibility to {isVisible}");
  Line 852: DebugLog($"UISystem: Failed to set resource display visibility - {ex.Message}");
  Line 876: DebugLog($"UISystem: Failed to get inventory UI statistics - {ex.Message}");
  Line 894: DebugLog($"UISystem: Updated UI text to '{text}'");
  Line 899: DebugLog($"UISystem: Failed to update UI text - {ex.Message}");
  Line 916: DebugLog($"UISystem: Updated UI visibility to {isVisible}");
  Line 921: DebugLog($"UISystem: Failed to update UI visibility - {ex.Message}");
  Line 958: DebugLog("UISystem: Starting shutdown...");
  Line 973: DebugLog("UISystem: Shutdown complete with all subsystems including game lifecycle UI");
  Line 977: DebugLog($"UISystem: Shutdown failed - {ex.Message}");
  Line 985: /// The UISystem does NOT require any event subscriptions for its core functionality.
  Line 988: /// - UISystem queries EntityManager for entities with UIComponent
- File: .\Engine\Systems\Gameplay\GameOverSystem.cs
  Line 362: // 1. Call uiSystem.ShowGameOverScreen(wasVictory, finalStats)
- File: .\Engine\Systems\UI\UISystem.cs
  Line 3: public class UISystem { }

---

### Type: RenderQueue

Found definition(s):
- File: .\Engine\Systems\RenderQueue.cs
  Namespace: SASZombieAssaultTD.Engine

Referenced in:
- File: .\Engine\Rendering\RenderQueue.cs
  Line 6: /// Rendering-layer queue frontend. Delegates to the Systems.RenderQueue.
  Line 7: /// Renamed from RenderQueue to resolve the duplicate class name with
  Line 8: /// Engine.Systems.RenderQueue.
  Line 12: private readonly Systems.RenderQueue _queue = new();
- File: .\Engine\Systems\RenderingSystem.cs
  Line 84: private readonly RenderQueue _renderQueue = new RenderQueue();
  Line 172: // P11-08-20: Use centralized RenderQueue sorting for deterministic ordering
  Line 215: // P11-08-19: Use centralized RenderQueue instead of local list
  Line 233: /// P11-08-19: Uses unified RenderQueue for consistent rendering pipeline.
- File: .\Engine\Systems\RenderQueue.cs
  Line 9: public sealed class RenderQueue
- File: .\Engine\Systems\UI\LayoutSystem.cs
  Line 13: public void Render(UIElementBase element, RenderQueue queue, IRenderContext context)

---

### Type: Texture2D

Found definition(s):
- File: .\Engine\Rendering\Texture2D.cs
  Namespace: SASZombieAssaultTD.Engine.Rendering

Referenced in:
- File: .\Engine\Rendering\Sprite.cs
  Line 14: private Texture2D _texture;
  Line 28: public Texture2D Texture
  Line 140: public Sprite(Texture2D texture = null, Vector2? position = null, Vector2? size = null)
- File: .\Engine\Rendering\SpriteBatchRenderer.cs
  Line 19: public Texture2D Texture;
  Line 29: public void Draw(Texture2D texture, Rectangle dest)
- File: .\Engine\Rendering\Texture2D.cs
  Line 20: public sealed class Texture2D : IDisposable
  Line 46: /// Constructs a Texture2D with optional pixel data.
  Line 56: public Texture2D(string name, int width, int height, byte[]? pixels)
  Line 76: public static Texture2D LoadFromFile(string filePath, TextureCache? textureCache = null)
  Line 92: var texture = new Texture2D(fileName, 0, 0, null);
  Line 163: throw new ObjectDisposedException(nameof(Texture2D));
- File: .\Engine\Rendering\TextureCache.cs
  Line 24: private readonly Dictionary<string, Texture2D> _cache = new();
  Line 37: public bool TryGet(string filePath, out Texture2D? texture)
  Line 47: public void Add(string filePath, Texture2D texture)
- File: .\Engine\Rendering\Zombies\ZombieRenderer.cs
  Line 15: private readonly Texture2D _texture;
  Line 17: public ZombieRenderer(Texture2D texture)
- File: .\Engine\Systems\DefaultPlatformRenderer.cs
  Line 46: Texture2D texture,
- File: .\Engine\Systems\RenderingSystem.cs
  Line 208: var texture = _assetManager.GetAsset<Texture2D>(spriteComponent.AssetId);
  Line 257: var texture = _assetManager.GetAsset<Texture2D>(item.TextureName);
  Line 349: Texture2D texture,
  Line 388: public Texture2D? Texture { get; set; }
- File: .\Engine\Systems\UISystem.cs
  Line 338: var texture = _assetManager.GetAsset<Texture2D>(uiComponent.AssetId);
- File: .\Engine\Systems\Assets\AssetDiscovery.cs
  Line 123: AssetType.Texture => typeof(Texture2D),
- File: .\Engine\Systems\Assets\AssetKey.cs
  Line 20: /// The runtime type of the asset (e.g., Texture2D, Sound, JsonData).
- File: .\Engine\Systems\Assets\AssetManager.cs
  Line 269: private Texture2D LoadTextureAsset(AssetMetadata metadata)
  Line 274: return Texture2D.LoadFromFile(metadata.Path, _textureCache);
- File: .\Engine\Systems\Hazards\Analytics\HazardDebugOverlay.cs
  Line 528: var texture = new Texture2D((int)size.x, (int)size.y);
- File: .\Engine\Systems\Hazards\Visual\HazardVisualSystem.cs
  Line 841: var texture = new Texture2D(64, 64);
  Line 864: var texture = new Texture2D(64, 64);

---

### Type: IManagedSystem

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\SystemManager.cs
  Line 11: //     - Initializes all IManagedSystem implementations in dependency order
  Line 56: private readonly List<IManagedSystem> _managedSystems = new();
  Line 142: if (system is not IManagedSystem managedSystem)
  Line 144: LogInfo($"System {systemType.Name} does not implement IManagedSystem — skipping lifecycle management.");

---

### Type: IUpdatableSystem

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\UpdateManager.cs
  Line 8: //     that implement IUpdatableSystem.
  Line 53: /// <see cref="IUpdatableSystem"/>. Handles update orchestration, performance
  Line 58: private readonly List<IUpdatableSystem> _updatableSystems = new();
  Line 119: if (system is not IUpdatableSystem updatable)

---

### Type: IRenderableSystem

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\RenderManager.cs
  Line 8: //     that implement IRenderableSystem.
  Line 56: /// <see cref="IRenderableSystem"/>. Handles render orchestration,
  Line 61: private readonly List<IRenderableSystem> _renderableSystems = new();
  Line 126: if (system is not IRenderableSystem renderable)

---

### Type: Vector2

Found definition(s):
- File: .\Engine\Core\Interfaces\IGameStateMachine.cs
  Namespace: SASZombieAssaultTD.Engine.Core.Interfaces
- File: .\Engine\Core\Math\VectorTypes.cs
  Namespace: SASZombieAssaultTD.Engine.Math
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Namespace: SASZombieAssaultTD.Engine.Navigation

Referenced in:
- File: .\Engine\Animation\AnimationClip.cs
  Line 206: public Vector2 TransformOffset { get; }
  Line 229: TransformOffset = Vector2.Zero;
  Line 240: public void SetTransformOffset(Vector2 offset)
- File: .\Engine\Animation\AnimationParameters.cs
  Line 179: /// P11-16-04: Vector2 animation parameter.
  Line 183: private Vector2 _value;
  Line 185: public Vector2Parameter(string name, Vector2 defaultValue, bool isReadOnly = false)
  Line 201: return value is Vector2;
- File: .\Engine\Animation\AnimationTrack.cs
  Line 280: /// P11-16-01: Interpolatable implementation for Vector2 values.
  Line 282: public struct InterpolatableVector2 : IInterpolatable<Vector2>
  Line 284: private readonly Vector2 _value;
  Line 286: public InterpolatableVector2(Vector2 value)
  Line 291: public Vector2 Interpolate(Vector2 target, float t)
  Line 296: public Vector2 Extrapolate(float timeDelta)
- File: .\Engine\Core\Input\InputModule.cs
  Line 48: private Vector2 _mousePosition = Vector2.Zero;
  Line 49: private Vector2 _previousMousePosition = Vector2.Zero;
  Line 84: public Vector2 MousePosition => _mousePosition;
  Line 89: public Vector2 MouseDelta => _mousePosition - _previousMousePosition;
  Line 647: public Vector2 MousePosition { get; set; }
  Line 648: public Vector2 MouseDelta { get; set; }
  Line 835: public Vector2 MousePosition { get; set; }
- File: .\Engine\Core\Interfaces\IDebugRenderer.cs
  Line 130: void DrawScreenText(Vector2 position, string text, System.Drawing.Color color, int fontSize);
  Line 169: void DrawGrid(Vector3 position, Vector2 size, float spacing, System.Drawing.Color color);
- File: .\Engine\Core\Interfaces\IGameStateMachine.cs
  Line 298: Vector2 ProjectToScreen(Vector3 worldPosition);
  Line 305: Vector3 UnprojectFromScreen(Vector2 screenPosition);
  Line 464: Vector2 MousePosition { get; }
  Line 469: Vector2 MouseDelta { get; }
  Line 571: public struct Vector2
  Line 575: public Vector2(float x, float y)
  Line 580: public static Vector2 Zero => new Vector2(0, 0);
  Line 581: public static Vector2 One => new Vector2(1, 1);
  Line 583: public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
  Line 584: public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
  Line 585: public static Vector2 operator *(Vector2 a, float s) => new Vector2(a.X * s, a.Y * s);
  Line 586: public static Vector2 operator /(Vector2 a, float s) => new Vector2(a.X / s, a.Y / s);
- File: .\Engine\Core\Math\VectorTypes.cs
  Line 9: public struct Vector2
  Line 14: public Vector2(float x, float y)
  Line 20: public static Vector2 Zero => new Vector2(0, 0);
  Line 21: public static Vector2 One => new Vector2(1, 1);
- File: .\Engine\ECS\ECSVerificationSuite.cs
  Line 141: var enemy = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 146: var projectile = EntityFactory.CreateProjectile(_ecsWorld, new Vector2(50, 100), new Vector2(200, 0), 25f, DamageType.Ballistic);
  Line 180: var enemy = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 211: aiSystem.PlayerPosition = new Vector2(200, 200);
  Line 214: var enemy = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 229: var distanceToPlayer = Vector2.Distance(finalPosition, aiSystem.PlayerPosition);
  Line 230: var initialDistanceToPlayer = Vector2.Distance(initialPosition, aiSystem.PlayerPosition);
  Line 244: var enemy1 = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 245: var enemy2 = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 252: aiSystem.PlayerPosition = new Vector2(200, 200);
  Line 264: var distance = Vector2.Distance(position1, position2);
  Line 283: EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(x, y));
  Line 323: var enemy = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Zombie, new Vector2(100, 100));
  Line 324: var projectile = EntityFactory.CreateProjectile(_ecsWorld, new Vector2(50, 100), new Vector2(200, 0), 50f, DamageType.Ballistic);
  Line 399: var enemy = EntityFactory.CreateEnemy(_ecsWorld, EnemyType.Runner, new Vector2(50, 50));
  Line 400: var projectile = EntityFactory.CreateProjectile(_ecsWorld, new Vector2(100, 100), new Vector2(1, 0), 25f);
- File: .\Engine\ECS\EntityFactory.cs
  Line 33: public static Entity CreateEnemy(ECSWorld world, EnemyType type, Vector2 position)
  Line 67: Vector2 position,
  Line 68: Vector2 velocity,
  Line 97: public static Entity CreatePlayer(ECSWorld world, Vector2 position)
  Line 119: public static Entity CreateTower(ECSWorld world, Vector2 position, string towerType = "Basic")
  Line 249: public Vector2 Position;
  Line 258: public Vector2 Position;
  Line 259: public Vector2 Velocity;
- File: .\Engine\ECS\EntityManager.cs
  Line 130: public IEnumerable<Entity> GetEntitiesInRadius(Vector2 position, float radius)
  Line 137: return transform != null && Vector2.Distance(transform.Position, position) <= radius;
  Line 147: public IEnumerable<Entity> GetEnemiesInRadius(Vector2 position, float radius)
  Line 159: public IEnumerable<Entity> GetProjectilesInRadius(Vector2 position, float radius)
  Line 171: public Entity? GetNearestEnemy(Vector2 position, float maxRange = float.MaxValue)
  Line 174: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
  Line 185: public Entity? GetNearestEnemyOfType(Vector2 position, EnemyType enemyType, float maxRange = float.MaxValue)
  Line 188: .Where(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position) <= maxRange)
  Line 189: .OrderBy(entity => Vector2.Distance(entity.GetComponent<TransformComponent>()!.Position, position))
- File: .\Engine\ECS\Components\AnimationControllerComponent.cs
  Line 324: /// <returns>Current transform offset, or Vector2.Zero if not available.</returns>
  Line 325: public Vector2 GetCurrentTransformOffset()
  Line 328: return frame?.TransformOffset ?? Vector2.Zero;
- File: .\Engine\ECS\Components\DamageComponent.cs
  Line 59: private Vector2 _knockbackForce;
  Line 83: public Vector2 KnockbackForce
  Line 116: _knockbackForce = Vector2.Zero;
  Line 138: public DamageComponent(float damageAmount, DamageType damageType, Vector2 knockbackForce, bool isCritical = false) : this()
  Line 153: public void SetDamage(float damageAmount, DamageType damageType, Vector2 knockbackForce, bool isCritical = false)
  Line 168: public void SetKnockback(float magnitude, Vector2 direction)
  Line 170: if (direction == Vector2.Zero)
  Line 172: _knockbackForce = Vector2.Zero;
  Line 176: var normalizedDirection = Vector2.Normalize(direction);
- File: .\Engine\ECS\Components\NavAgentComponent.cs
  Line 17: private Vector2 _targetPosition;
  Line 18: private List<Vector2> _currentPath;
  Line 33: public Vector2 TargetPosition
  Line 49: public IReadOnlyList<Vector2> CurrentPath => _currentPath.AsReadOnly();
  Line 119: public float DistanceToTarget => Vector2.Distance(GetCurrentPosition(), _targetPosition);
  Line 125: Vector2.Distance(GetCurrentPosition(), _currentPath[_currentPathIndex]) : 0f;
  Line 130: public event Action<NavAgentComponent, List<Vector2>>? OnPathStarted;
  Line 150: public event Action<NavAgentComponent, Vector2>? OnWaypointReached;
  Line 159: _targetPosition = Vector2.Zero;
  Line 160: _currentPath = new List<Vector2>();
  Line 179: public void SetPath(List<Vector2> path)
  Line 198: OnPathStarted?.Invoke(this, new List<Vector2>(_currentPath));
  Line 233: public Vector2 AdvanceAlongPath(float deltaTime, Vector2 currentPosition, FlowField? flowField = null)
  Line 252: private Vector2 AdvanceAlongFlowField(float deltaTime, Vector2 currentPosition, FlowField flowField)
  Line 270: var worldDirection = new Vector2(flowDirection.X, flowDirection.Y);
  Line 271: var normalizedDirection = Vector2.Normalize(worldDirection);
  Line 284: public bool HasReachedTarget(Vector2 currentPosition)
  Line 286: return Vector2.Distance(currentPosition, _targetPosition) <= _stoppingDistance;
  Line 316: public void MoveTo(Vector2 target)
  Line 335: public Vector2 GetNextWaypoint()
  Line 347: public Vector2 GetCurrentPosition()
- File: .\Engine\ECS\Systems\AISystem.cs
  Line 31: private Vector2 _playerPosition;
  Line 35: public Vector2 PlayerPosition
  Line 54: _playerPosition = Vector2.Zero;
  Line 130: if (Vector2.Distance(navAgent.TargetPosition, targetPosition) > navAgent.StoppingDistance)
  Line 179: private Vector2 DetermineNavigationTarget(Entity entity, EnemyTypeComponent enemyType, Vector2 currentPosition)
  Line 183: if (Vector2.Distance(currentPosition, _playerPosition) <= DetectionRange)
  Line 196: private Vector2 GetNearestObjective(Vector2 currentPosition)
  Line 197: => new Vector2(500, 500);
  Line 199: private Vector2 GetNextPatrolPoint(Entity entity, Vector2 currentPosition)
  Line 206: currentPosition + new Vector2(100, 0),
  Line 207: currentPosition + new Vector2(0, 100),
  Line 208: currentPosition + new Vector2(-100, 0),
  Line 209: currentPosition + new Vector2(0, -100)
  Line 215: private Vector2 GetWanderTarget(Vector2 currentPosition)
  Line 220: return currentPosition + new Vector2(
  Line 238: var distanceToPlayer = Vector2.Distance(transform.Position, _playerPosition);
  Line 283: if (direction != Vector2.Zero)
  Line 293: if (fleeDir != Vector2.Zero)
  Line 298: var objPos = new Vector2(400, 300);
  Line 300: if (objDir != Vector2.Zero)
  Line 310: if (state.TimeInState >= 2.0f || state.WanderDirection == Vector2.Zero)
  Line 313: state.WanderDirection = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
  Line 332: new Vector2(1, 0),
  Line 333: new Vector2(0, 1),
  Line 334: new Vector2(-1, 0),
  Line 335: new Vector2(0, -1)
  Line 374: WanderDirection = Vector2.Zero,
  Line 487: var reflectedVelocity = currentVelocity - 2f * Vector2.Dot(currentVelocity, normal) * normal;
  Line 541: public Vector2 WanderDirection { get; set; }
- File: .\Engine\ECS\Systems\AnimationSystem.cs
  Line 293: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
  Line 306: var position = controller.Entity.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
  Line 461: if (transformOffset != Vector2.Zero)
  Line 561: new AnimationFrame(0f, 0.1f, 0, Vector2.Zero, 0xFFFFFFFF, null)
- File: .\Engine\ECS\Systems\CollisionSystem.cs
  Line 253: var normal = Vector2.Normalize(centerB - centerA);
  Line 271: var distance = Vector2.Distance(centerA, centerB);
  Line 278: var normal = Vector2.Normalize(centerB - centerA);
  Line 301: Vector2 normal;
  Line 307: normal = boundsA.Center.X < boundsB.Center.X ? new Vector2(-1, 0) : new Vector2(1, 0);
  Line 313: normal = boundsA.Center.Y < boundsB.Center.Y ? new Vector2(0, -1) : new Vector2(0, 1);
  Line 333: var closestPoint = new Vector2(
  Line 339: var distance = Vector2.Distance(circleCenter, closestPoint);
  Line 344: var normal = distance > 0.001f ? Vector2.Normalize(closestPoint - circleCenter) : Vector2.Zero;
- File: .\Engine\ECS\Systems\CombatSystem.cs
  Line 156: var distance = Vector2.Distance(projectileTransform.Position, targetTransform.Position);
  Line 204: var distance = Vector2.Distance(transform.Position, target.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero);
  Line 249: if (application.KnockbackForce != Vector2.Zero)
  Line 277: private void ApplyKnockback(Entity target, Vector2 knockbackForce)
  Line 330: private IEnumerable<Entity> FindEntitiesInRadius(Vector2 position, float radius)
  Line 337: return transform != null && Vector2.Distance(transform.Position, position) <= radius;
  Line 553: public Vector2 KnockbackForce;
- File: .\Engine\ECS\Systems\NavigationSystem.cs
  Line 69: public event Action<uint, List<Vector2>>? OnPathCalculated;
  Line 151: public void RequestPath(uint entityId, Vector2 startPos, Vector2 endPos, int priority = 0)
  Line 361: private bool IsAgentBlocked(Entity entity, NavAgentComponent navAgent, Vector2 position)
  Line 370: var distanceToWaypoint = Vector2.Distance(position, nextWaypoint);
  Line 482: private Vector2 GetEntitySize(ColliderComponent collider)
  Line 485: return new Vector2(bounds.Width, bounds.Height);
  Line 493: private void ForceRepathForAffectedAgents(Vector2 center, Vector2 size)
  Line 521: private bool IsAgentInArea(Entity entity, Vector2 areaCenter, Vector2 areaSize)
  Line 567: public Vector2 StartPosition { get; set; }
  Line 568: public Vector2 EndPosition { get; set; }
  Line 573: public List<Vector2>? ResultPath { get; set; }
- File: .\Engine\Entities\Enemy.cs
  Line 88: public Vector2? TargetPosition { get; set; }
  Line 93: public Vector2[]? PatrolPath { get; set; }
  Line 224: var direction = Vector2.Normalize(TargetPosition.Value - transform.Position);
  Line 256: float distance = Vector2.Distance(transform.Position, targetTransform.Position);
- File: .\Engine\Input\InputEvent.cs
  Line 71: public Vector2 Position { get; set; }
  Line 96: public InputEvent(InputType type, KeyCode key, Vector2 position, float value, InputState state)
  Line 114: return new InputEvent(InputType.Keyboard, key, Vector2.Zero, 0f, state);
  Line 124: public static InputEvent MouseButton(KeyCode key, Vector2 position, InputState state)
  Line 134: public static InputEvent MouseMove(Vector2 position)
  Line 145: public static InputEvent MouseScroll(Vector2 position, float value)
- File: .\Engine\Input\InputEventTypes.cs
  Line 44: public System.Numerics.Vector2 Position { get; set; }
  Line 45: public System.Numerics.Vector2 Delta { get; set; }
  Line 48: public InputMouseMoveEvent(System.Numerics.Vector2 position, System.Numerics.Vector2 delta)
  Line 62: public System.Numerics.Vector2 Position { get; set; }
  Line 66: public InputMouseButtonEvent(KeyCode button, System.Numerics.Vector2 position, InputState state)
  Line 80: public System.Numerics.Vector2 Position { get; set; }
  Line 84: public InputMouseScrollEvent(System.Numerics.Vector2 position, float scrollDelta)
- File: .\Engine\Input\InputManager.cs
  Line 57: private Vector2 _lastMousePosition;
  Line 104: _lastMousePosition = Vector2.Zero;
  Line 464: _eventBus.Publish(new InputMouseMoveEvent(inputEvent.Position, Vector2.Zero)); // Delta would need tracking
  Line 558: public void HandleMouseWheel(float scrollValue, Vector2 position)
  Line 660: controllerEvent.Position = Vector2.Zero;
  Line 666: controllerEvent.Position = new Vector2(
- File: .\Engine\Input\InputRecorder.cs
  Line 172: var position = new System.Numerics.Vector2(
- File: .\Engine\Input\MouseInputSource.cs
  Line 14: private Vector2 _previousPosition;
  Line 15: private Vector2 _currentPosition;
  Line 38: public Vector2 Position => _currentPosition;
  Line 43: public Vector2 Delta => _currentPosition - _previousPosition;
  Line 152: private Vector2 GetMousePosition()
  Line 161: return Vector2.Zero; // Placeholder
  Line 165: return Vector2.Zero;
- File: .\Engine\Navigation\AStarPathfinder.cs
  Line 23: private readonly List<Vector2> _pathBuffer;
  Line 55: _pathBuffer = new List<Vector2>();
  Line 66: public List<Vector2> FindPath(Vector2 startWorldPos, Vector2 endWorldPos)
  Line 80: return new List<Vector2>();
  Line 89: return new List<Vector2>();
  Line 103: return new List<Vector2>(_pathBuffer);
  Line 142: return new List<Vector2>();
  Line 151: public bool HasDirectPath(Vector2 startWorldPos, Vector2 endWorldPos)
  Line 248: private List<Vector2> ReconstructPath(NavigationCell endCell)
  Line 277: return new List<Vector2>(_pathBuffer);
  Line 288: var optimized = new List<Vector2> { _pathBuffer[0] };
  Line 315: private bool CanSkipWaypoint(Vector2 prev, Vector2 current, Vector2 next)
  Line 318: var dir1 = Vector2.Normalize(current - prev);
  Line 319: var dir2 = Vector2.Normalize(next - current);
  Line 320: var dot = Vector2.Dot(dir1, dir2);
- File: .\Engine\Navigation\NavigationGrid.cs
  Line 21: private readonly Vector2 _worldOrigin;
  Line 44: public Vector2 WorldOrigin => _worldOrigin;
  Line 73: public NavigationGrid(int width, int height, float cellSize, Vector2 worldOrigin)
  Line 97: public Vector2Int WorldToGrid(Vector2 worldPosition)
  Line 110: public Vector2 GridToWorld(Vector2Int gridPosition)
  Line 121: public Vector2 GridToWorld(Vector2Int gridPosition, bool centerOfCell)
  Line 124: return _worldOrigin + new Vector2(
  Line 145: public bool IsWalkable(Vector2 worldPosition)
  Line 322: public void UpdateEntityArea(Vector2 entityPosition, Vector2 entitySize, bool isWalkable)
- File: .\Engine\Navigation\NavigationMigrationHelper.cs
  Line 129: public static Entity CreateNavAgentEntity(ECSWorld world, float speed, Vector2 position)
  Line 407: public readonly struct Vector2
  Line 412: public Vector2(float x, float y)
- File: .\Engine\Navigation\NavigationVerificationSuite.cs
  Line 149: var agent = CreateTestAgent(new Vector2(50, 50));
  Line 150: agent.GetComponent<NavAgentComponent>().MoveTo(new Vector2(450, 450));
  Line 162: if (Vector2.Distance(transform.Position, navAgent.TargetPosition) > navAgent.StoppingDistance)
  Line 164: throw new Exception($"Agent failed to reach target. Distance: {Vector2.Distance(transform.Position, navAgent.TargetPosition)}, Stopping: {navAgent.StoppingDistance}");
  Line 182: var position = new Vector2(50 + i * 20, 50 + (i % 4) * 20);
  Line 183: var target = new Vector2(450 + (i % 3) * 50, 450 + (i / 3) * 50);
  Line 223: var agent = CreateTestAgent(new Vector2(50, 50));
  Line 224: agent.GetComponent<NavAgentComponent>().MoveTo(new Vector2(450, 450));
  Line 233: var obstacle = CreateTestObstacle(new Vector2(250, 250));
  Line 264: var position = new Vector2(50 + (i % 10) * 30, 50 + (i / 10) * 30);
  Line 265: var target = new Vector2(450 + (i % 5) * 20, 450 + (i / 5) * 20);
  Line 302: var targetGrid = _navigationGrid.WorldToGrid(new Vector2(450, 450));
  Line 315: var position = new Vector2(50 + i * 30, 50);
  Line 333: var distance = Vector2.Distance(transform.Position, new Vector2(450, 450));
  Line 355: var enemy = CreateTestEnemy(new Vector2(50, 50));
  Line 361: aiSystem.PlayerPosition = new Vector2(450, 450);
  Line 372: var expectedTarget = new Vector2(450, 450);
  Line 375: if (Vector2.Distance(actualTarget, expectedTarget) > navAgent.StoppingDistance)
  Line 392: var agent = CreateTestAgent(new Vector2(50, 50));
  Line 393: agent.GetComponent<NavAgentComponent>().MoveTo(new Vector2(450, 450));
  Line 428: legacyEntity.AddComponent(new TransformComponent(new Vector2(50, 50));
  Line 474: private Entity CreateTestAgent(Vector2 position)
  Line 489: private Entity CreateTestEnemy(Vector2 position)
  Line 506: private Entity CreateTestObstacle(Vector2 position)
  Line 512: new AABBShape(Vector2.Zero, new Vector2(50, 50)),
- File: .\Engine\Pathfinding\PathfindingOptimizer.cs
  Line 38: private readonly ConcurrentDictionary<PathCacheKey, List<Vector2>> _pathCache =
  Line 39: new ConcurrentDictionary<PathCacheKey, List<Vector2>>();
  Line 72: public async Task<List<Vector2>> FindPathAsync(
  Line 73: Vector2 start,
  Line 74: Vector2 end,
  Line 114: public async Task<List<(PathRequest Request, List<Vector2> Path)>> FindPathsAsync(
  Line 118: var results = new List<(PathRequest, List<Vector2>)>();
  Line 151: private List<Vector2> ComputeAStar(
  Line 152: Vector2 start,
  Line 153: Vector2 end,
  Line 170: if (Vector2.Distance(current.Position, end) < 0.1f)
  Line 186: current.G + Vector2.Distance(current.Position, neighborPos),
  Line 200: return new List<Vector2>(); // No path found
  Line 207: private List<Vector2> ReconstructPath(PathNode node)
  Line 209: var path = new List<Vector2>();
  Line 239: private float CalculateHeuristic(Vector2 from, Vector2 to)
  Line 301: public readonly Vector2 Start;
  Line 302: public readonly Vector2 End;
  Line 304: public PathCacheKey(Vector2 start, Vector2 end)
  Line 322: public readonly Vector2 Start;
  Line 323: public readonly Vector2 End;
  Line 325: public PathRequest(Vector2 start, Vector2 end)
  Line 393: public Vector2 Position;
  Line 400: public void Reset(Vector2 pos, float g, float h, PathNode parent)
  Line 448: public IEnumerable<Vector2> GetNeighbors(Vector2 pos)
  Line 451: yield return new Vector2(pos.X + 1, pos.Y);
  Line 452: yield return new Vector2(pos.X - 1, pos.Y);
  Line 453: yield return new Vector2(pos.X, pos.Y + 1);
  Line 454: yield return new Vector2(pos.X, pos.Y - 1);
  Line 455: yield return new Vector2(pos.X + 1, pos.Y + 1);
  Line 456: yield return new Vector2(pos.X - 1, pos.Y - 1);
  Line 457: yield return new Vector2(pos.X + 1, pos.Y - 1);
  Line 458: yield return new Vector2(pos.X - 1, pos.Y + 1);
- File: .\Engine\Performance\DebugOverlay.cs
  Line 44: private Vector2 _position;
  Line 160: _position = new Vector2(10, 10);
  Line 357: var graphPosition = new Vector2(_position.X, _position.Y + 120);
  Line 358: var graphSize = new Vector2(200, 60);
  Line 373: var position = new Vector2(_position.X, _position.Y + 200);
  Line 385: var position = new Vector2(_position.X, _position.Y + 220);
  Line 402: var position = new Vector2(_position.X, _position.Y + 240);
  Line 414: var position = new Vector2(_position.X, _position.Y + 280);
  Line 421: private void RenderTextLines(SpriteBatch spriteBatch, List<string> lines, Vector2 position)
  Line 425: var linePosition = position + new Vector2(0, i * 15 * _scale);
- File: .\Engine\Physics\AABBShape.cs
  Line 17: private Vector2 _size;
  Line 22: public Vector2 Size
  Line 25: set => _size = Vector2.Max(Vector2.Zero, value);
  Line 31: public Vector2 HalfSize => _size * 0.5f;
  Line 46: public Vector2 Min => Center - HalfSize;
  Line 51: public Vector2 Max => Center + HalfSize;
  Line 73: public AABBShape(Vector2 center, Vector2 size) : base(center)
  Line 75: _size = Vector2.Max(Vector2.Zero, size);
  Line 82: public AABBShape(Vector2 size) : this(Vector2.Zero, size)
  Line 91: public AABBShape(Vector2 min, Vector2 max) : this((min + max) * 0.5f, max - min)
  Line 108: public override void Translate(Vector2 offset)
  Line 118: public override bool ContainsPoint(Vector2 point)
  Line 128: public void Set(Vector2 center, Vector2 size)
  Line 139: public void SetFromMinMax(Vector2 min, Vector2 max)
  Line 149: public Vector2[] GetCorners()
  Line 152: return new Vector2[]
  Line 154: Center + new Vector2(-halfSize.X, -halfSize.Y), // Bottom-left
  Line 155: Center + new Vector2(halfSize.X, -halfSize.Y),  // Bottom-right
  Line 156: Center + new Vector2(halfSize.X, halfSize.Y),   // Top-right
  Line 157: Center + new Vector2(-halfSize.X, halfSize.Y)   // Top-left
- File: .\Engine\Physics\CapsuleShape.cs
  Line 62: var min = Center - new Vector2(_radius, halfTotalHeight);
  Line 63: var max = Center + new Vector2(_radius, halfTotalHeight);
  Line 80: public CapsuleShape(Vector2 center, float radius, float height) : base(center)
  Line 91: public CapsuleShape(float radius, float height) : this(Vector2.Zero, radius, height)
  Line 108: public override void Translate(Vector2 offset)
  Line 118: public override bool ContainsPoint(Vector2 point)
  Line 131: var hemisphereCenter = new Vector2(0, Math.Sign(localPoint.Y) * HalfHeight);
  Line 132: var distanceToHemisphere = Vector2.Distance(localPoint, hemisphereCenter);
  Line 142: public void Set(Vector2 center, float radius, float height)
  Line 153: public Vector2 GetTopHemisphereCenter()
  Line 155: return Center + new Vector2(0, HalfHeight);
  Line 162: public Vector2 GetBottomHemisphereCenter()
  Line 164: return Center - new Vector2(0, HalfHeight);
- File: .\Engine\Physics\CircleShape.cs
  Line 45: var min = Center - new Vector2(_radius, _radius);
  Line 46: var max = Center + new Vector2(_radius, _radius);
  Line 61: public CircleShape(Vector2 center, float radius) : base(center)
  Line 70: public CircleShape(float radius) : this(Vector2.Zero, radius)
  Line 87: public override void Translate(Vector2 offset)
  Line 97: public override bool ContainsPoint(Vector2 point)
  Line 99: var distanceSquared = Vector2.DistanceSquared(point, Center);
  Line 108: public void Set(Vector2 center, float radius)
- File: .\Engine\Physics\ColliderComponent.cs
  Line 139: return new BoundingBox(Vector2.Zero, Vector2.Zero);
  Line 165: return new BoundingBox(Vector2.Zero, Vector2.Zero);
  Line 294: public bool ContainsPoint(Vector2 point)
- File: .\Engine\Physics\CollisionDebugRenderer.cs
  Line 170: var worldMin = new Vector2(-1000, -1000);
  Line 171: var worldMax = new Vector2(1000, 1000);
  Line 179: var cellMin = worldMin + new Vector2(x * cellSize, y * cellSize);
  Line 180: var cellMax = cellMin + new Vector2(cellSize, cellSize);
  Line 257: private void RenderShape(IRenderContext context, CollisionShape shape, Vector2 position, bool isTrigger)
  Line 301: private void RenderCapsule(IRenderContext context, CapsuleShape capsule, Vector2 position, uint color)
  Line 309: var rectMin = position + new Vector2(-radius, -halfHeight);
  Line 310: var rectSize = new Vector2(width, capsule.Height);
- File: .\Engine\Physics\CollisionEvent.cs
  Line 175: public readonly Vector2 Point;
  Line 181: public readonly Vector2 Normal;
  Line 192: public readonly Vector2 RelativeVelocity;
  Line 206: public ContactInfo(Vector2 point, Vector2 normal, float penetrationDepth, Vector2 relativeVelocity)
  Line 209: Normal = Vector2.Normalize(normal);
  Line 220: public ContactInfo(Vector2 point, Vector2 normal, float penetrationDepth) : this(point, normal, penetrationDepth, Vector2.Zero)
- File: .\Engine\Physics\CollisionShape.cs
  Line 17: private Vector2 _center;
  Line 23: public Vector2 Center
  Line 50: protected CollisionShape(Vector2 center)
  Line 65: public abstract void Translate(Vector2 offset);
  Line 72: public abstract bool ContainsPoint(Vector2 point);
  Line 118: public Vector2 Min;
  Line 123: public Vector2 Max;
  Line 128: public Vector2 Center => (Min + Max) * 0.5f;
  Line 133: public Vector2 Size => Max - Min;
  Line 150: public BoundingBox(Vector2 min, Vector2 max)
  Line 162: public static BoundingBox FromCenterAndSize(Vector2 center, Vector2 size)
  Line 184: public bool Contains(Vector2 point)
  Line 196: Min = Vector2.Min(Min, other.Min);
  Line 197: Max = Vector2.Max(Max, other.Max);
  Line 204: public void Encapsulate(Vector2 point)
  Line 206: Min = Vector2.Min(Min, point);
  Line 207: Max = Vector2.Max(Max, point);
- File: .\Engine\Physics\CollisionVerificationSuite.cs
  Line 140: var enemy = CreateTestEnemy(new Vector2(100, 100));
  Line 145: var projectile = CreateTestProjectile(new Vector2(50, 100), new Vector2(200, 0));
  Line 177: var trigger = CreateTestTrigger(new Vector2(0, 0), new Vector2(200, 200));
  Line 187: var enemy = CreateTestEnemy(new Vector2(-300, 0));
  Line 189: enemyMovement.SetVelocity(new Vector2(100, 0));
  Line 202: enemyMovement.SetVelocity(new Vector2(200, 0));
  Line 229: var entity = CreateTestEnemy(new Vector2(x, y));
  Line 279: private List<Vector2> CreateDeterministicTestSetup()
  Line 290: var entity = CreateTestEnemy(new Vector2(x, y));
  Line 357: var circle1 = new CircleShape(Vector2.Zero, 50f);
  Line 358: var circle2 = new CircleShape(new Vector2(50f, 0), 30f);
  Line 365: var expectedContact = new Vector2(25f, 0f);
  Line 366: var distance = Vector2.Distance(circleContact.Value.Point, expectedContact);
  Line 371: var aabb1 = new AABBShape(Vector2.Zero, new Vector2(100, 100));
  Line 372: var aabb2 = new AABBShape(new Vector2(50, 50), new Vector2(150, 150));
  Line 379: var circle3 = new CircleShape(new Vector2(75, 75), 30f);
  Line 380: var aabb3 = new AABBShape(Vector2.Zero, new Vector2(150, 150));
  Line 405: var entity1 = CreateTestEnemy(new Vector2(0, 0));
  Line 406: var entity2 = CreateTestEnemy(new Vector2(50, 0));
  Line 411: movement1.SetVelocity(new Vector2(100, 0));
  Line 412: movement2.SetVelocity(new Vector2(-50, 0));
  Line 432: movement1.SetVelocity(new Vector2(100, 0));
  Line 433: movement2.SetVelocity(new Vector2(-100, 0));
  Line 454: var player = CreateTestPlayer(new Vector2(0, 0));
  Line 455: var enemy = CreateTestEnemy(new Vector2(100, 0));
  Line 456: var projectile = CreateTestProjectile(new Vector2(50, 0), new Vector2(0, 0));
  Line 527: private Entity CreateTestEnemy(Vector2 position)
  Line 539: new CircleShape(Vector2.Zero, 25f),
  Line 553: private Entity CreateTestProjectile(Vector2 position, Vector2 velocity)
  Line 563: new CircleShape(Vector2.Zero, 10f),
  Line 579: private Entity CreateTestPlayer(Vector2 position)
  Line 589: new CircleShape(Vector2.Zero, 30f),
  Line 603: private Entity CreateTestTrigger(Vector2 position, Vector2 size)
  Line 611: new AABBShape(Vector2.Zero, size),
- File: .\Engine\Physics\SpatialPartitionGrid.cs
  Line 24: private readonly Vector2 _worldMin;
  Line 25: private readonly Vector2 _worldMax;
  Line 48: public SpatialPartitionGrid(Vector2 worldMin, Vector2 worldMax, float cellSize)
  Line 278: public IEnumerable<Entity> GetEntitiesInRadius(Vector2 center, float radius)
  Line 280: var radiusBounds = BoundingBox.FromCenterAndSize(center, new Vector2(radius * 2, radius * 2));
  Line 290: var distance = Vector2.Distance(center, entityCenter);
  Line 356: var clampedMin = Vector2.Max(bounds.Min, _worldMin);
  Line 357: var clampedMax = Vector2.Min(bounds.Max, _worldMax);
- File: .\Engine\Rendering\RenderCommandQueue.cs
  Line 179: public Vector2 Position { get; }
  Line 180: public Vector2 Size { get; }
  Line 183: public DrawRectangleCommand(Vector2 position, Vector2 size, Color color)
  Line 203: public Vector2 Position { get; }
  Line 204: public Vector2 Size { get; }
  Line 207: public DrawTextureCommand(IntPtr texture, Vector2 position, Vector2 size, Rectangle? sourceRect = null)
- File: .\Engine\Rendering\Renderer.cs
  Line 19: private Vector2 _viewportSize;
  Line 53: public Vector2 ViewportSize => _viewportSize;
  Line 172: _viewportSize = new Vector2(width, height);
  Line 339: _viewportSize = new Vector2(width, height);
- File: .\Engine\Rendering\Sprite.cs
  Line 15: private Vector2 _position;
  Line 16: private Vector2 _size;
  Line 20: private Vector2 _origin;
  Line 21: private Vector2 _scale;
  Line 41: public Vector2 Position
  Line 50: public Vector2 Size
  Line 86: public Vector2 Origin
  Line 95: public Vector2 Scale
  Line 127: public Vector2 Center => _position + _size / 2;
  Line 140: public Sprite(Texture2D texture = null, Vector2? position = null, Vector2? size = null)
  Line 143: _position = position ?? Vector2.Zero;
  Line 144: _size = size ?? Vector2.Zero;
  Line 148: _origin = Vector2.Zero;
  Line 149: _scale = Vector2.One;
  Line 170: var effectiveSize = new Vector2(
  Line 213: _size = new Vector2(width, height);
  Line 252: public void Move(Vector2 offset)
  Line 272: public void Scale(Vector2 scale)
  Line 282: public void SetSizeMaintainAspect(Vector2 targetSize)
  Line 291: Vector2 newSize;
  Line 295: newSize = new Vector2(targetSize.X, targetSize.X / aspectRatio);
  Line 300: newSize = new Vector2(targetSize.Y * aspectRatio, targetSize.Y);
  Line 325: public bool ContainsPoint(Vector2 point)
  Line 338: if (_texture != null && _size == Vector2.Zero)
  Line 341: _size = new Vector2(sourceRect.Width, sourceRect.Height);
- File: .\Engine\Rendering\SpriteBatch.cs
  Line 124: public void Draw(IntPtr texture, Vector2 position, Rectangle? sourceRect = null,
  Line 125: Color? color = null, float rotation = 0f, Vector2? origin = null,
  Line 126: Vector2? scale = null, SpriteEffects effects = SpriteEffects.None,
  Line 148: Origin = origin ?? Vector2.Zero,
  Line 149: Scale = scale ?? Vector2.One,
  Line 170: Color? color = null, float rotation = 0f, Vector2? origin = null,
  Line 179: var position = new Vector2(destinationRect.X, destinationRect.Y);
  Line 180: var scale = new Vector2(destinationRect.Width, destinationRect.Height);
  Line 198: public void DrawString(Font font, string text, Vector2 position, Color? color = null,
  Line 199: float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
  Line 291: var transformedPosition = Vector2.Transform(command.Position, _transformMatrix);
  Line 300: if (command.Scale != Vector2.One)
  Line 341: public Vector2 Position { get; set; }
  Line 345: public Vector2 Origin { get; set; }
  Line 346: public Vector2 Scale { get; set; }
- File: .\Engine\Rendering\SpriteBatchOptimizer.cs
  Line 155: public void Draw(IntPtr texture, Vector2 position, Rectangle? sourceRect = null,
  Line 156: Color? color = null, float rotation = 0f, Vector2? origin = null,
  Line 157: Vector2? scale = null, SpriteEffects effects = SpriteEffects.None,
  Line 170: Origin = origin ?? Vector2.Zero,
  Line 171: Scale = scale ?? Vector2.One,
  Line 177: command.Position = Vector2.Transform(command.Position, _transformMatrix);
  Line 366: public Vector2 Position { get; set; }
  Line 370: public Vector2 Origin { get; set; }
  Line 371: public Vector2 Scale { get; set; }
  Line 404: new Vector2(command.SourceRect.Value.Width, command.SourceRect.Value.Height) :
  Line 405: new Vector2(64, 64); // Default size
- File: .\Engine\Rendering\TextRenderer.cs
  Line 99: public void DrawText(string text, Vector2 position, DrawingColor color, float scale = 1.0f,
  Line 216: private Vector2 CalculateLinePosition(Vector2 basePosition, string line, float scale,
  Line 230: return new Vector2(x, y);
  Line 236: private void DrawTextWithContext(IRenderContext context, string text, Vector2 position, DrawingColor color, float scale)
  Line 246: private void DrawTextFallback(string text, Vector2 position, DrawingColor color, float scale)
- File: .\Engine\Scene\Entity.cs
  Line 15: private Vector2 _position;
  Line 16: private Vector2 _size;
  Line 35: public Vector2 Position
  Line 51: public Vector2 Size
  Line 141: public Vector2 Center => _position + _size / 2;
  Line 146: public event Action<Vector2> OnPositionChanged;
  Line 151: public event Action<Vector2> OnSizeChanged;
  Line 184: protected Entity(string id = null, Vector2? position = null, Vector2? size = null)
  Line 187: _position = position ?? Vector2.Zero;
  Line 188: _size = size ?? Vector2.One;
  Line 386: public void Move(Vector2 offset)
  Line 404: public void Scale(Vector2 scale)
- File: .\Engine\Scene\GameplayScene.cs
  Line 245: var playerEntity = new Entity("Player", new Vector2(400, 300), new Vector2(32, 32));
- File: .\Engine\Scene\MainMenuScene.cs
  Line 225: _mainPanel = new Panel("MainPanel", new Vector2(200, 100), new Vector2(400, 400));
  Line 232: _titleLabel = new Label("TitleLabel", "SAS ZOMBIE ASSAULT TD", null, new Vector2(250, 150));
  Line 237: _buttonPanel = new Panel("ButtonPanel", new Vector2(250, 250), new Vector2(300, 200));
  Line 257: var button = new Label(buttonIds[i], buttonNames[i], null, Vector2.Zero);
  Line 260: button.Size = new Vector2(280, 40);
  Line 289: _mainPanel.Position = new Vector2(200, 100);
- File: .\Engine\State\GameplayState.cs
  Line 142: public System.Numerics.Vector2 Direction { get; }
  Line 151: Direction = System.Numerics.Vector2.Zero;
  Line 159: public GameplayInputEvent(GameplayAction action, System.Numerics.Vector2 direction)
- File: .\Engine\Systems\ParticleSystem.cs
  Line 679: /// Range for Vector2 values with random generation support.
- File: .\Engine\Systems\UISystem.cs
  Line 792: public void ShowItemTooltip(object itemData, System.Numerics.Vector2 position)
  Line 826: public void UpdateItemTooltip(object itemData, System.Numerics.Vector2? position = null)
- File: .\Engine\Systems\Hazards\Analytics\HazardDebugOverlay.cs
  Line 96: canvasScaler.referenceResolution = new Vector2(1920, 1080);
  Line 526: private Sprite CreateRectangleSprite(Vector2 size)
  Line 540: return Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f));
  Line 663: public Vector2 IntensityBarSize { get; set; } = new Vector2(50f, 5f);
- File: .\Engine\Systems\Hazards\Visual\HazardVisualSystem.cs
  Line 842: var center = new Vector2(32, 32);
  Line 849: var distance = Vector2.Distance(new Vector2(x, y), center);
  Line 856: return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
  Line 865: var center = new Vector2(32, 32);
  Line 873: var distance = Vector2.Distance(new Vector2(x, y), center);
  Line 880: return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
- File: .\Engine\Systems\UI\ItemTooltipRenderer.cs
  Line 31: private System.Numerics.Vector2 _currentPosition;
  Line 57: public void Show(object itemData, System.Numerics.Vector2 position)
  Line 89: _currentPosition = System.Numerics.Vector2.Zero;
  Line 109: public void UpdateTooltip(object itemData, System.Numerics.Vector2? position = null)
- File: .\Engine\Systems\UI\KillFeedSystem.cs
  Line 37: public Vector2 Position { get; set; } = new Vector2(10, 100);
  Line 387: public Vector2 Position { get; set; }
- File: .\Engine\Systems\UI\ScoreDisplaySystem.cs
  Line 44: public Vector2 Position { get; set; } = new Vector2(10, 10);
  Line 169: Position = new Vector2(Position.X + 100, Position.Y),
  Line 342: public Vector2 Position { get; set; }
- File: .\Engine\UI\Label.cs
  Line 147: public Vector2 TextSize { get; private set; }
  Line 173: Vector2? position = null, Color? textColor = null)
  Line 206: public Vector2 MeasureText(string text = null)
  Line 216: return new Vector2(estimatedWidth, estimatedHeight);
  Line 247: public override void HandleInput(Vector2 inputPosition, bool isClicked)
- File: .\Engine\UI\Panel.cs
  Line 108: public Panel(string id = null, Vector2? position = null, Vector2? size = null)
  Line 255: public override void HandleInput(Vector2 inputPosition, bool isClicked)
  Line 305: child.Position = new Vector2(_borderThickness, yOffset);
  Line 323: child.Position = new Vector2(xOffset, _borderThickness);
  Line 343: child.Position = new Vector2(
- File: .\Engine\UI\UIElement.cs
  Line 15: private Vector2 _position;
  Line 16: private Vector2 _size;
  Line 32: public Vector2 Position
  Line 48: public Vector2 Size
  Line 160: public Vector2 Center => _position + _size / 2;
  Line 193: public event Action<Vector2> OnPositionChanged;
  Line 198: public event Action<Vector2> OnSizeChanged;
  Line 246: protected UIElement(string id = null, Vector2? position = null, Vector2? size = null)
  Line 249: _position = position ?? Vector2.Zero;
  Line 250: _size = size ?? Vector2.Zero;
  Line 297: public virtual void HandleInput(Vector2 inputPosition, bool isClicked)
  Line 420: protected virtual void HandleInputInternal(Vector2 inputPosition, bool isClicked)
  Line 438: _size = new Vector2(originalSize.X * 0.9f, originalSize.Y * 0.9f);
- File: .\Engine\UI\UIManager.cs
  Line 20: private Vector2 _viewportSize;
  Line 36: public Vector2 ViewportSize => _viewportSize;
  Line 124: _viewportSize = new Vector2(800, 600);
  Line 158: _viewportSize = new Vector2(viewportWidth, viewportHeight);
  Line 377: _viewportSize = new Vector2(width, height);
  Line 391: Vector2 mousePosition = Vector2.Zero; // Would get from input system
  Line 527: _viewportSize = new Vector2(
- File: .\Engine\Window\Window.cs
  Line 15: private Vector2 _size;
  Line 16: private Vector2 _position;
  Line 42: public Vector2 Size
  Line 55: public Vector2 Position
  Line 108: public event Action<Vector2> OnWindowResized;
  Line 118: public event Action<Vector2> OnWindowMoved;
  Line 126: _size = new Vector2(800, 600);
  Line 127: _position = Vector2.Zero;
  Line 144: public bool Open(string title = null, int width = 800, int height = 600, Vector2? position = null)
  Line 155: _size = new Vector2(width, height);
  Line 312: private Vector2 SimulateResizeCheck()

---

### Type: IAnimationState

Found definition(s):
- File: .\Engine\Animation\States\IAnimationState.cs
  Namespace: SASZombieAssaultTD.Engine.Animation

Referenced in:
- File: .\Engine\Animation\AnimationStateMachine.cs
  Line 23: private readonly Dictionary<string, IAnimationState> _states;
  Line 34: public IAnimationState? CurrentState { get; private set; }
  Line 40: public IAnimationState? PreviousState { get; private set; }
  Line 107: private void TransitionToState(IAnimationState newState)
  Line 154: public void SetInitialState(IAnimationState initialState)
  Line 267: public void ForceTransitionTo(IAnimationState state)
  Line 320: _states = new Dictionary<string, IAnimationState>();
  Line 333: public void RegisterState(IAnimationState state)
  Line 406: public IAnimationState? GetState(string stateName)
- File: .\Engine\Animation\States\AttackState.cs
  Line 3: Purpose: P11-17-05 - Implement AttackState using IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 13: /// Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 15: public class AttackState : IAnimationState
  Line 165: public IAnimationState? CheckTransitions()
- File: .\Engine\Animation\States\IAnimationState.cs
  Line 2: File:    IAnimationState.cs
  Line 14: public interface IAnimationState
  Line 48: IAnimationState? CheckTransitions();
- File: .\Engine\Animation\States\IdleState.cs
  Line 3: Purpose: P11-17-03 - Implement IdleState using IAnimationState with deterministic transitions and no placeholder logic.
  Line 13: /// Implements IAnimationState with explicit transition conditions and no placeholder logic.
  Line 15: public class IdleState : IAnimationState
  Line 144: public IAnimationState? CheckTransitions()
- File: .\Engine\Animation\States\JumpState.cs
  Line 3: Purpose: P11-17-05 - Implement JumpState using IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 13: /// Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 15: public class JumpState : IAnimationState
  Line 177: public IAnimationState? CheckTransitions()
- File: .\Engine\Animation\States\MoveState.cs
  Line 3: Purpose: P11-17-04 - Implement MoveState using IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 13: /// Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
  Line 15: public class MoveState : IAnimationState
  Line 167: public IAnimationState? CheckTransitions()

---

### Type: AnimationConditionOperator

No definition found in project.

Referenced in:
- File: .\Engine\Animation\AnimationParameters.cs
  Line 282: private readonly AnimationConditionOperator _operator;
  Line 285: public ParameterCondition(AnimationParameter parameter, AnimationConditionOperator op, object referenceValue)

---

### Type: IInputDevice

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\InputManager.cs
  Line 67: private readonly Dictionary<InputDeviceType, IInputDevice> _devices = new();
  Line 198: public void RegisterDevice(IInputDevice device)

---

### Type: InputDeviceType

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\InputManager.cs
  Line 67: private readonly Dictionary<InputDeviceType, IInputDevice> _devices = new();
  Line 220: public void UnregisterDevice(InputDeviceType deviceType)

---

### Type: InputEventType

No definition found in project.

Referenced in:
- File: .\BDC\Projects\SASZombieAssaultTD\Engine\Core\Input\InputModule.cs
  Line 88: Type = InputEventType.KeyDown,
  Line 100: Type = InputEventType.KeyUp,
  Line 113: Type = InputEventType.MouseMove,
  Line 126: Type = InputEventType.MouseButtonDown,
  Line 138: Type = InputEventType.MouseButtonUp,
  Line 150: Type = InputEventType.MouseWheel,
  Line 191: case InputEventType.KeyDown:
  Line 197: case InputEventType.KeyUp:
  Line 202: case InputEventType.MouseMove:
  Line 206: case InputEventType.MouseButtonDown:
  Line 212: case InputEventType.MouseButtonUp:
  Line 217: case InputEventType.MouseWheel:
  Line 336: public InputEventType Type;
  Line 347: internal enum InputEventType
- File: .\Engine\Core\Input\InputModule.cs
  Line 505: case InputEventType.KeyPressed:
  Line 512: case InputEventType.KeyReleased:
  Line 519: case InputEventType.MouseMoved:
  Line 523: case InputEventType.MouseButtonPressed:
  Line 530: case InputEventType.MouseButtonReleased:
  Line 537: case InputEventType.MouseWheel:
  Line 832: public InputEventType Type { get; set; }
  Line 883: public enum InputEventType { KeyPressed, KeyReleased, MouseMoved, MouseButtonPressed, MouseButtonReleased, MouseWheel }
- File: .\Engine\Core\Managers\InputManager.cs
  Line 77: private readonly Dictionary<InputEventType, List<IInputHandler>> _handlers = new();
  Line 244: public void RegisterHandler(InputEventType eventType, IInputHandler handler)
  Line 261: public void UnregisterHandler(InputEventType eventType, IInputHandler handler)
- File: .\Engine\Systems\InputRouter.cs
  Line 12: public enum InputEventType
  Line 24: public InputEventType Type { get; }
  Line 29: public InputEvent(InputEventType type, int keyCode = 0, int mouseX = 0, int mouseY = 0)

---

### Type: IInputHandler

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\InputManager.cs
  Line 77: private readonly Dictionary<InputEventType, List<IInputHandler>> _handlers = new();
  Line 244: public void RegisterHandler(InputEventType eventType, IInputHandler handler)
  Line 251: _handlers[eventType] = new List<IInputHandler>();
  Line 261: public void UnregisterHandler(InputEventType eventType, IInputHandler handler)

---

### Type: InputSystemInfo

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\InputManager.cs
  Line 28: //         - InputSystemInfo diagnostics
  Line 64: private readonly Dictionary<Type, InputSystemInfo> _systemInfo = new();
  Line 282: public InputSystemInfo? GetSystemInfo<T>() where T : class
  Line 294: public IEnumerable<InputSystemInfo> GetAllSystemInfo()
  Line 356: _systemInfo[type] = new InputSystemInfo

---

### Type: InputPerformanceMetrics

No definition found in project.

Referenced in:
- File: .\Engine\Core\Managers\InputManager.cs
  Line 303: public InputPerformanceMetrics GetPerformanceMetrics()
  Line 311: return new InputPerformanceMetrics

---

### Type: InputState

Found definition(s):
- File: .\Engine\Core\Input\InputModule.cs
  Namespace: SASZombieAssaultTD.Engine.Core.Input

Referenced in:
- File: .\Engine\Core\Input\InputModule.cs
  Line 319: public InputState GetInputState()
  Line 323: return new InputState
  Line 645: public class InputState
- File: .\Engine\Core\Interfaces\IGameStateMachine.cs
  Line 157: /// <param name="inputState">The current input state.</param>
  Line 158: void HandleInput(IInputState inputState);
- File: .\Engine\Core\Managers\InputManager.cs
  Line 16: //     - Maintains unified InputState for all systems
  Line 68: private readonly InputState _inputState = new();
  Line 331: public InputState GetInputState()
- File: .\Engine\Input\InputEvent.cs
  Line 35: public enum InputState
  Line 81: public InputState State { get; set; }
  Line 96: public InputEvent(InputType type, KeyCode key, Vector2 position, float value, InputState state)
  Line 112: public static InputEvent Keyboard(KeyCode key, InputState state)
  Line 124: public static InputEvent MouseButton(KeyCode key, Vector2 position, InputState state)
  Line 136: return new InputEvent(InputType.MouseMove, KeyCode.None, position, 0f, InputState.Held);
  Line 147: return new InputEvent(InputType.MouseScroll, KeyCode.None, position, value, InputState.Pressed);
- File: .\Engine\Input\InputEventTypes.cs
  Line 63: public InputState State { get; set; }
  Line 66: public InputMouseButtonEvent(KeyCode button, System.Numerics.Vector2 position, InputState state)
- File: .\Engine\Input\InputManager.cs
  Line 405: if (inputEvent.Type == InputType.MouseButton && inputEvent.State == InputState.Pressed)
  Line 414: doubleClickEvent.State = InputState.Held; // Use Held to indicate double click
  Line 422: if (inputEvent.State == InputState.Pressed)
  Line 426: else if (inputEvent.State == InputState.Released)
  Line 453: if (inputEvent.State == InputState.Pressed)
  Line 457: else if (inputEvent.State == InputState.Released)
- File: .\Engine\Input\InputRecorder.cs
  Line 177: var state = Enum.Parse<InputState>(parts[6]);
- File: .\Engine\Input\KeyboardInputSource.cs
  Line 94: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Pressed));
  Line 99: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Released));
  Line 104: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Held));
  Line 116: public InputState GetKeyState(KeyCode key)
  Line 119: return InputState.Released;
  Line 124: if (curr && !prev) return InputState.Pressed;
  Line 125: if (!curr && prev) return InputState.Released;
  Line 126: if (curr && prev) return InputState.Held;
  Line 128: return InputState.Released;
  Line 144: return GetKeyState(key) == InputState.Pressed;
  Line 152: return GetKeyState(key) == InputState.Released;
  Line 232: public enum InputState
  Line 245: public InputState State { get; }
  Line 247: private InputEvent(KeyCode key, InputState state)
  Line 253: public static InputEvent Keyboard(KeyCode key, InputState state) =>
- File: .\Engine\Input\MouseInputSource.cs
  Line 123: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Pressed));
  Line 128: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Released));
  Line 133: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Held));
  Line 215: public InputState GetButtonState(KeyCode button)
  Line 218: return InputState.Released;
  Line 224: return InputState.Pressed;
  Line 226: return InputState.Released;
  Line 228: return InputState.Held;
  Line 230: return InputState.Released;
  Line 250: return GetButtonState(button) == InputState.Pressed;
  Line 260: return GetButtonState(button) == InputState.Released;
- File: .\Engine\Systems\UI\Input\UIInputRouter.cs
  Line 32: public UIInputState InputState => _inputState;
  Line 47: /// <param name="inputState">Input state to use</param>
  Line 49: public UIInputRouter(UIInputState inputState, UIFocusManager focusManager)
  Line 51: _inputState = inputState ?? throw new ArgumentNullException(nameof(inputState));

---

### Type: InputEvent

Found definition(s):
- File: .\BDC\Projects\SASZombieAssaultTD\Engine\Core\Input\InputModule.cs
  Namespace: SASZombieAssaultTD.Engine.Core.Input
- File: .\Engine\Core\Input\InputModule.cs
  Namespace: SASZombieAssaultTD.Engine.Core.Input
- File: .\Engine\Input\InputEvent.cs
  Namespace: Engine.Input
- File: .\Engine\Input\KeyboardInputSource.cs
  Namespace: SASZombieAssaultTD.Engine.Input
- File: .\Engine\Systems\InputRouter.cs
  Namespace: SASZombieAssaultTD.Engine

Referenced in:
- File: .\BDC\Projects\SASZombieAssaultTD\Engine\Core\Input\InputModule.cs
  Line 13: private readonly Queue<InputEvent> _eventBuffer = new Queue<InputEvent>();
  Line 78: var inputEvent = _eventBuffer.Dequeue();
  Line 79: ProcessInputEvent(inputEvent);
  Line 87: _eventBuffer.Enqueue(new InputEvent
  Line 99: _eventBuffer.Enqueue(new InputEvent
  Line 112: _eventBuffer.Enqueue(new InputEvent
  Line 125: _eventBuffer.Enqueue(new InputEvent
  Line 137: _eventBuffer.Enqueue(new InputEvent
  Line 149: _eventBuffer.Enqueue(new InputEvent
  Line 188: /// <param name="inputEvent">The input event to process.</param>
  Line 189: private void ProcessInputEvent(InputEvent inputEvent)
  Line 190: switch (inputEvent.Type)
  Line 192: if (_keyStates[inputEvent.Key] != KeyState.Pressed)
  Line 193: _keyStates[inputEvent.Key] = KeyState.Pressed;
  Line 194: _keysPressedThisFrame.Add(inputEvent.Key);
  Line 198: _keyStates[inputEvent.Key] = KeyState.Released;
  Line 199: _keysReleasedThisFrame.Add(inputEvent.Key);
  Line 203: _mousePosition = new Point(inputEvent.MouseX, inputEvent.MouseY);
  Line 207: if (_mouseButtonStates[inputEvent.MouseButton] != ButtonState.Pressed)
  Line 208: _mouseButtonStates[inputEvent.MouseButton] = ButtonState.Pressed;
  Line 209: _buttonsPressedThisFrame.Add(inputEvent.MouseButton);
  Line 213: _mouseButtonStates[inputEvent.MouseButton] = ButtonState.Released;
  Line 214: _buttonsReleasedThisFrame.Add(inputEvent.MouseButton);
  Line 218: _scrollWheelValue += inputEvent.ScrollDelta;
  Line 335: internal struct InputEvent
- File: .\Engine\Core\Input\InputModule.cs
  Line 60: private readonly Queue<InputEvent> _inputBuffer = new();
  Line 496: var inputEvent = _inputBuffer.Dequeue();
  Line 497: ProcessInputEvent(inputEvent);
  Line 501: private void ProcessInputEvent(InputEvent inputEvent)
  Line 503: switch (inputEvent.Type)
  Line 506: if (_keyStates.TryGetValue(inputEvent.Key, out var keyState))
  Line 513: if (_keyStates.TryGetValue(inputEvent.Key, out keyState))
  Line 520: _mousePosition = inputEvent.MousePosition;
  Line 524: if (_mouseButtonStates.TryGetValue(inputEvent.MouseButton, out var mouseState))
  Line 531: if (_mouseButtonStates.TryGetValue(inputEvent.MouseButton, out mouseState))
  Line 538: _mouseWheelDelta += inputEvent.MouseWheelDelta;
  Line 830: public class InputEvent
- File: .\Engine\Core\Managers\InputManager.cs
  Line 76: private readonly Queue<InputEvent> _eventQueue = new();
  Line 475: private void ProcessEvent(InputEvent evt)
- File: .\Engine\Input\IInputSource.cs
  Line 13: event Action<InputEvent> OnInput;
- File: .\Engine\Input\InputEvent.cs
  Line 56: public struct InputEvent
  Line 96: public InputEvent(InputType type, KeyCode key, Vector2 position, float value, InputState state)
  Line 112: public static InputEvent Keyboard(KeyCode key, InputState state)
  Line 114: return new InputEvent(InputType.Keyboard, key, Vector2.Zero, 0f, state);
  Line 124: public static InputEvent MouseButton(KeyCode key, Vector2 position, InputState state)
  Line 126: return new InputEvent(InputType.MouseButton, key, position, 0f, state);
  Line 134: public static InputEvent MouseMove(Vector2 position)
  Line 136: return new InputEvent(InputType.MouseMove, KeyCode.None, position, 0f, InputState.Held);
  Line 145: public static InputEvent MouseScroll(Vector2 position, float value)
  Line 147: return new InputEvent(InputType.MouseScroll, KeyCode.None, position, value, InputState.Pressed);
  Line 156: return $"InputEvent(Type={Type}, Key={Key}, State={State}, Position={Position}, Value={Value}, Timestamp={Timestamp:HH:mm:ss.fff})";
- File: .\Engine\Input\InputManager.cs
  Line 68: public event Action<InputEvent> OnInput;
  Line 393: /// <param name="inputEvent">The input event to handle</param>
  Line 394: private void OnSourceInput(InputEvent inputEvent)
  Line 399: if (inputEvent.Type == InputType.Keyboard || inputEvent.Type == InputType.MouseButton)
  Line 401: inputEvent.Key = GetBoundKey(inputEvent.Key);
  Line 405: if (inputEvent.Type == InputType.MouseButton && inputEvent.State == InputState.Pressed)
  Line 407: var isDoubleClick = IsDoubleClick(inputEvent.Key);
  Line 408: _lastClickTimes[inputEvent.Key] = DateTime.UtcNow;
  Line 413: var doubleClickEvent = inputEvent;
  Line 415: inputEvent = doubleClickEvent;
  Line 420: if (inputEvent.Type == InputType.Keyboard)
  Line 422: if (inputEvent.State == InputState.Pressed)
  Line 424: _keyPressStartTimes[inputEvent.Key] = DateTime.UtcNow;
  Line 426: else if (inputEvent.State == InputState.Released)
  Line 428: _keyPressStartTimes.Remove(inputEvent.Key);
  Line 433: _inputBuffer.AddInput(inputEvent);
  Line 438: _inputRecorder.RecordInput(inputEvent);
  Line 442: Console.WriteLine($"[P20-01 DEBUG] Input Event: {inputEvent}");
  Line 445: OnInput?.Invoke(inputEvent);
  Line 450: switch (inputEvent.Type)
  Line 453: if (inputEvent.State == InputState.Pressed)
  Line 455: _eventBus.Publish(new InputKeyPressedEvent(inputEvent.Key));
  Line 457: else if (inputEvent.State == InputState.Released)
  Line 459: _eventBus.Publish(new InputKeyReleasedEvent(inputEvent.Key));
  Line 464: _eventBus.Publish(new InputMouseMoveEvent(inputEvent.Position, Vector2.Zero)); // Delta would need tracking
  Line 468: _eventBus.Publish(new InputMouseButtonEvent(inputEvent.Key, inputEvent.Position, inputEvent.State));
  Line 472: _eventBus.Publish(new InputMouseScrollEvent(inputEvent.Position, inputEvent.Value));
  Line 560: var scrollEvent = InputEvent.MouseScroll(position, scrollValue);
  Line 605: public List<InputEvent> ProcessBufferedInput()
  Line 607: var processedEvents = new List<InputEvent>();
  Line 614: foreach (var inputEvent in bufferedEvents)
  Line 617: if (inputEvent.Type == InputType.KeyPressed || inputEvent.Type == InputType.KeyReleased)
  Line 619: if (_keyBindings.TryGetValue(inputEvent.KeyCode, out var reboundKey))
  Line 621: inputEvent.KeyCode = reboundKey;
  Line 625: processedEvents.Add(inputEvent);
  Line 646: public InputEvent ProcessControllerInput(InputEvent controllerEvent)
- File: .\Engine\Input\InputPlayback.cs
  Line 13: private readonly List<InputEvent> _playbackEvents;
  Line 40: public event Action<InputEvent> OnInputPlayed;
  Line 52: _playbackEvents = new List<InputEvent>();
  Line 63: public void SetPlaybackEvents(InputEvent[] events)
- File: .\Engine\Input\InputRecorder.cs
  Line 14: private readonly List<InputEvent> _recordedEvents;
  Line 37: public event Action<InputEvent> OnInputRecorded;
  Line 44: _recordedEvents = new List<InputEvent>();
  Line 82: /// <param name="inputEvent">The input event to record</param>
  Line 83: public void RecordInput(InputEvent inputEvent)
  Line 89: var relativeEvent = inputEvent;
  Line 100: public InputEvent[] GetRecordedEvents()
  Line 179: var evt = new InputEvent(type, key, position, value, state);
- File: .\Engine\Input\KeyboardInputSource.cs
  Line 39: public event Action<InputEvent> OnInput;
  Line 94: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Pressed));
  Line 99: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Released));
  Line 104: OnInput?.Invoke(InputEvent.Keyboard(key, InputState.Held));
  Line 242: public readonly struct InputEvent
  Line 247: private InputEvent(KeyCode key, InputState state)
  Line 253: public static InputEvent Keyboard(KeyCode key, InputState state) =>
  Line 254: new InputEvent(key, state);
  Line 264: event Action<InputEvent> OnInput;
- File: .\Engine\Input\MouseInputSource.cs
  Line 23: public event Action<InputEvent> OnInput;
  Line 102: OnInput?.Invoke(InputEvent.MouseMove(_currentPosition));
  Line 123: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Pressed));
  Line 128: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Released));
  Line 133: OnInput?.Invoke(InputEvent.MouseButton(button, _currentPosition, InputState.Held));
  Line 144: OnInput?.Invoke(InputEvent.MouseScroll(_currentPosition, scrollDelta));
- File: .\Engine\Systems\InputRouter.cs
  Line 9: void OnInputEvent(InputEvent inputEvent);
  Line 22: public readonly struct InputEvent
  Line 29: public InputEvent(InputEventType type, int keyCode = 0, int mouseX = 0, int mouseY = 0)
  Line 73: public void Route(InputEvent inputEvent)
  Line 76: Debug.WriteLine($"[InputRouter] Routing event: {inputEvent.Type}");
  Line 80: listener.OnInputEvent(inputEvent);

---

