using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    /// <summary>
    /// Level up controller for SAS Zombie Assault TD.
    /// Manages the level up process, animations, and UI feedback.
    /// </summary>
    public class LevelUpController
    {
        private readonly List<LevelUpAnimation> _animations;
        private readonly Dictionary<string, LevelUpEffect> _effects;
        private bool _isLevelUpInProgress;
        private bool _isInitialized;
        private static LevelUpController _instance;
        // gpte test




      


         

        // Events
        public event Action<int, int, List<PlayerReward>> OnLevelUpStarted;
        public event Action<int, int, List<PlayerReward>> OnLevelUpCompleted;
        public event Action<LevelUpEffect> OnEffectTriggered;
        public event Action OnLevelUpSequenceCompleted;

        // Properties
        public bool IsInitialized => _isInitialized;
        public bool IsLevelUpInProgress => _isLevelUpInProgress;
        public int CurrentAnimationCount => _animations.Count(a => a.IsActive);
        public static LevelUpController Instance => _instance ??= new LevelUpController();
        // gpte test
        // HTTP 429 (insufficient_quota: insufficient_quota)
        // You exceeded your current quota, please check your plan and billing details. For more information on this error, read the docs: https://platform.openai.com/docs/guides/error-codes/api-errors.

        // To use this extension, you need an OpenAi account and provide it's API key to this extension
        // https://platform.openai.com/account/api-keys
        // Set API key in options, Tools->Options->Comment2GPT->Authentication->OpenAI(ChatGPT)->OpenAI API Key

        // Singleton
        private LevelUpController()
        {
            _animations = new List<LevelUpAnimation>();
            _effects = new Dictionary<string, LevelUpEffect>();
            _isLevelUpInProgress = false;
            _isInitialized = false;

            InitializeAnimations();
            InitializeEffects();
        }

        /// <summary>
        /// Initialize the level up controller.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Level Up Controller");

            try
            {
                // Subscribe to player level events
                if (PlayerLevel.Instance != null)
                {
                    PlayerLevel.Instance.OnLevelUp += OnPlayerLevelUp;
                }

                _isInitialized = true;
                Console.WriteLine("Level Up Controller initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Level Up Controller: {ex.Message}");
                throw;
            }
        }
        // gpte Rewrite this method to fix the CS0120 error by making Deserialize static. Only modify the Deserialize method.
        // HTTP 401 (invalid_request_error: invalid_issuer)
        // Your authentication token is not from a valid issuer.

        // To use this extension, you need an OpenAi account and provide it's API key to this extension
        // https://platform.openai.com/account/api-keys
        // Set API key in options, Tools->Options->Comment2GPT->Authentication->OpenAI(ChatGPT)->OpenAI API Key


        /// <summary>
        /// Start a level up sequence.
        /// </summary>
        /// <param name="fromLevel">Current level.</param>
        /// <param name="toLevel">New level.</param>
        /// <param name="rewards">Rewards for leveling up.</param>
        /// <returns>True if level up sequence was started.</returns>
        public bool StartLevelUp(int fromLevel, int toLevel, List<PlayerReward> rewards = null)
        {
            if (_isLevelUpInProgress) return false;

            try
            {
                _isLevelUpInProgress = true;

                // Get rewards if not provided
                if (rewards == null)
                {
                    rewards = (List<PlayerReward>)PlayerLevel.Instance.GetAvailableRewards(toLevel);
                }

                // Trigger start event
                OnLevelUpStarted?.Invoke(fromLevel, toLevel, rewards);

                // Start level up animation
                var animation = GetLevelUpAnimation(fromLevel, toLevel);
                if (animation != null)
                {
                    animation.Start();
                    _animations.Add(animation);
                }

                // Start level up effects
                StartLevelUpEffects(fromLevel, toLevel);

                Console.WriteLine($"Level up started: {fromLevel} → {toLevel}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting level up: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Complete the current level up sequence.
        /// </summary>
        /// <returns>True if level up was completed successfully.</returns>
        public bool CompleteLevelUp()
        {
            if (!_isLevelUpInProgress) return false;

            try
            {
                var activeAnimation = _animations.FirstOrDefault(a => a.IsActive);
                if (activeAnimation == null) return false;

                // Complete animation
                activeAnimation.Complete();

                // Get level up rewards
                var rewards = activeAnimation.Rewards;
                var fromLevel = activeAnimation.FromLevel;
                var toLevel = activeAnimation.ToLevel;

                // Grant rewards
                foreach (var reward in rewards)
                {
                    GrantReward(reward);
                }

                // Convert ProgressionReward to PlayerReward for event
                var playerRewards = ConvertToPlayerRewards(rewards);

                // Trigger completion event
                OnLevelUpCompleted?.Invoke(fromLevel, toLevel, playerRewards);

                // Clean up
                _animations.RemoveAll(a => a.IsActive);
                _isLevelUpInProgress = false;

                // Trigger sequence completion event
                if (_animations.Count == 0)
                {
                    OnLevelUpSequenceCompleted?.Invoke();
                }

                Console.WriteLine($"Level up completed: {fromLevel} → {toLevel}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completing level up: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cancel current level up sequence.
        /// </summary>
        /// <returns>True if level up was cancelled.</returns>
        public bool CancelLevelUp()
        {
            if (!_isLevelUpInProgress) return false;

            try
            {
                // Stop all animations
                foreach (var animation in _animations.Where(a => a.IsActive))
                {
                    animation.Stop();
                }

                // Stop all effects
                StopAllLevelUpEffects();

                // Clean up
                _animations.RemoveAll(a => a.IsActive);
                _isLevelUpInProgress = false;

                Console.WriteLine("Level up cancelled");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling level up: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get level up animation for level transition.
        /// </summary>
        /// <param name="fromLevel">Starting level.</param>
        /// <param name="toLevel">Target level.</param>
        /// <returns>Level up animation, or null if not found.</returns>
        public LevelUpAnimation GetLevelUpAnimation(int fromLevel, int toLevel)
        {
            return _animations.FirstOrDefault(a => a.FromLevel == fromLevel && a.ToLevel == toLevel);
        }

        /// <summary>
        /// Add a custom level up animation.
        /// </summary>
        /// <param name="animation">Animation to add.</param>
        /// <returns>True if animation was added.</returns>
        public bool AddAnimation(LevelUpAnimation animation)
        {
            if (animation == null) return false;
            if (_animations.Any(a => a.FromLevel == animation.FromLevel && a.ToLevel == animation.ToLevel)) return false;

            try
            {
                _animations.Add(animation);
                Console.WriteLine($"Added level up animation: {animation.FromLevel} → {animation.ToLevel}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding level up animation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Add a level up effect.
        /// </summary>
        /// <param name="effect">Effect to add.</param>
        /// <returns>True if effect was added.</returns>
        public bool AddEffect(LevelUpEffect effect)
        {
            if (effect == null) return false;
            if (_effects.ContainsKey(effect.Id)) return false;

            try
            {
                _effects[effect.Id] = effect;
                Console.WriteLine($"Added level up effect: {effect.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding level up effect: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Start level up effects.
        /// </summary>
        /// <param name="fromLevel">Starting level.</param>
        /// <param name="toLevel">Target level.</param>
        private void StartLevelUpEffects(int fromLevel, int toLevel)
        {
            var effects = GetLevelUpEffects(fromLevel, toLevel);
            foreach (var effect in effects)
            {
                effect.Start();
                OnEffectTriggered?.Invoke(effect);
            }
        }

        /// <summary>
        /// Stop all level up effects.
        /// </summary>
        private void StopAllLevelUpEffects()
        {
            foreach (var effect in _effects.Values)
            {
                if (effect.IsActive)
                {
                    effect.Stop();
                }
            }
        }

        /// <summary>
        /// Get level up effects for level transition.
        /// </summary>
        /// <param name="fromLevel">Starting level.</param>
        /// <param name="toLevel">Target level.</param>
        /// <returns>List of level up effects.</returns>
        private List<LevelUpEffect> GetLevelUpEffects(int fromLevel, int toLevel)
        {
            return _effects.Values.Where(e => e.FromLevel() <= toLevel && e.ToLevel() >= fromLevel).ToList();
        }

        /// <summary>
        /// Converts ProgressionReward list to PlayerReward list.
        /// </summary>
        /// <param name="progressionRewards">The progression rewards to convert.</param>
        /// <returns>Converted player rewards.</returns>
        private List<PlayerReward> ConvertToPlayerRewards(List<ProgressionReward> progressionRewards)
        {
            return progressionRewards.Select(pr => new PlayerReward
            {
                Type = pr.Type,
                Amount = pr.Amount,
                Description = $"{pr.Type} reward"
            }).ToList();
        }

        /// <summary>
        /// Get level up controller statistics.
        /// </summary>
        /// <returns>Level up controller statistics.</returns>
        public LevelUpControllerStatistics GetStatistics()
        {
            return new LevelUpControllerStatistics
            {
                TotalAnimations = _animations.Count,
                ActiveAnimations = _animations.Count(a => a.IsActive),
                TotalEffects = _effects.Count,
                ActiveEffects = _effects.Values.Count(e => e.IsActive),
                IsLevelUpInProgress = _isLevelUpInProgress,
                TotalLevelUpsCompleted = CalculateTotalLevelUpsCompleted(),
                AverageLevelUpTime = CalculateAverageLevelUpTime()
            };
        }

        /// <summary>
        /// Update all active animations and effects.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            try
            {
                // Update animations
                foreach (var animation in _animations.Where(a => a.IsActive))
                {
                    animation.Update(deltaTime);
                }

                // Update effects
                foreach (var effect in _effects.Values.Where(e => e.IsActive))
                {
                    effect.Update(deltaTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating level up controller: {ex.Message}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Initialize animations.
        /// </summary>
        private void InitializeAnimations()
        {
            // Level 1 → 2 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_1_to_2",
                Name = "Rank Up: Recruit to Private",
                FromLevel = 1,
                ToLevel = 2,
                Duration = 2.0f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 25 }
                }
            });

            // Level 2 → 3 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_2_to_3",
                Name = "Rank Up: Private to Corporal",
                FromLevel = 2,
                ToLevel = 3,
                Duration = 2.5f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 50 }
                }
            });

            // Level 3 → 4 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_3_to_4",
                Name = "Rank Up: Corporal to Sergeant",
                FromLevel = 3,
                ToLevel = 4,
                Duration = 3.0f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 75 }
                }
            });

            // Level 4 → 5 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_4_to_5",
                Name = "Rank Up: Sergeant to Lieutenant",
                FromLevel = 4,
                ToLevel = 5,
                Duration = 3.5f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 100 }
                }
            });

            // Level 5 → 6 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_5_to_6",
                Name = "Rank Up: Lieutenant to Captain",
                FromLevel = 5,
                ToLevel = 6,
                Duration = 4.0f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.TowerSlot, Amount = 1 }
                }
            });

            // Level 6 → 7 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_6_to_7",
                Name = "Rank Up: Captain to Major",
                FromLevel = 6,
                ToLevel = 7,
                Duration = 4.5f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Ability, Amount = 1 }
                }
            });

            // Level 7 → 8 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_7_to_8",
                Name = "Rank Up: Major to Colonel",
                FromLevel = 7,
                ToLevel = 8,
                Duration = 5.0f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.UpgradeDiscount, Amount = 10 }
                }
            });

            // Level 8 → 9 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_8_to_9",
                Name = "Rank Up: Colonel to Brigadier",
                FromLevel = 8,
                ToLevel = 9,
                Duration = 5.5f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 200 }
                }
            });

            // Level 9 → 10 animation
            _animations.Add(new LevelUpAnimation
            {
                Id = "level_9_to_10",
                Name = "Rank Up: Brigadier to General",
                FromLevel = 9,
                ToLevel = 10,
                Duration = 6.0f,
                AnimationType = AnimationType.LevelUp,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 500 },
                    new ProgressionReward { Type = RewardType.TowerSlot, Amount = 2 }
                }
            });
        }

        /// <summary>
        /// Initialize effects.
        /// </summary>
        private void InitializeEffects()
        {
            // Level up effects
            _effects.Add("level_up_glow", new LevelUpEffect
            {
                Id = "level_up_glow",
                Name = "Level Up Glow",
                Description = "Golden glow effect around player",
                Type = EffectType.Visual,
                Duration = 3.0f,
                Color = System.Drawing.Color.Gold,
                Intensity = 1.0f
            });

            _effects.Add("level_up_particles", new LevelUpEffect
            {
                Id = "level_up_particles",
                Name = "Level Up Particles",
                Description = "Particle effects celebrating level up",
                Type = EffectType.Particle,
                Duration = 2.5f,
                ParticleCount = 50,
                ParticleColor = System.Drawing.Color.Yellow
            });

            _effects.Add("level_up_sound", new LevelUpEffect
            {
                Id = "level_up_sound",
                Name = "Level Up Sound",
                Description = "Sound effect for level up",
                Type = EffectType.Audio,
                Duration = 1.0f,
                SoundPath = "Audio/LevelUp.wav"
            });

            _effects.Add("level_up_screen_shake", new LevelUpEffect
            {
                Id = "level_up_screen_shake",
                Name = "Screen Shake",
                Description = "Screen shake effect for dramatic level ups",
                Type = EffectType.Screen,
                Duration = 0.5f,
                Intensity = 0.3f
            });
        }

        /// <summary>
        /// Grant a reward.
        /// </summary>
        private void GrantReward(ProgressionReward reward)
        {
            // This would integrate with appropriate systems
            // gpte 
            // HTTP 401 (invalid_request_error: invalid_issuer)
            // Your authentication token is not from a valid issuer.

            switch (reward.Type)
            {
                case RewardType.Cash:
                    EconomyManager.Instance.AddCash(reward.Amount);
                    break;
                case RewardType.TowerSlot:
                    // Grant tower slot through tower system
                    break;
                case RewardType.UpgradeDiscount:
                    // Apply upgrade discount through tower system
                    break;
                case RewardType.Ability:
                    // Grant ability through player system
                    break;
                case RewardType.Experience:
                    // Grant experience through player level system
                    if (PlayerLevel.Instance != null)
                    {
                        PlayerLevel.Instance.AddExperience(reward.Amount, "Level Up Reward");
                    }
                    break;
            }
        }

        /// <summary>
        /// Calculate total level ups completed.
        /// </summary>
        private int CalculateTotalLevelUpsCompleted()
        {
            // This would be calculated from saved data
            // For now, return a reasonable default based on current level
            return PlayerLevel.Instance?.CurrentLevel ?? 1;
        }

        /// <summary>
        /// Calculate average level up time.
        /// </summary>
        private float CalculateAverageLevelUpTime()
        {
            // This would be calculated from saved data
            // For now, return a reasonable default
            return 3.2f; // Average 3.2 seconds per level up
        }

        #endregion

        // Corrected OnPlayerLevelUp method signature to match Action<int> delegate.
        private void OnPlayerLevelUp(int newLevel)
        {
            Console.WriteLine($"Player leveled up to {newLevel}");
            StartLevelUp(PlayerLevel.Instance.CurrentLevel - 1, newLevel);
        }
    }

    /// <summary>
    /// Level up animation for player advancement.
    /// </summary>
    public class LevelUpAnimation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int FromLevel { get; set; }
        public int ToLevel { get; set; }
        public float Duration { get; set; }
        public AnimationType AnimationType { get; set; }
        public List<ProgressionReward> Rewards { get; set; }
        public bool IsActive { get; set; }
        public float Progress { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Start()
        {
            IsActive = true;
            Progress = 0f;
            StartTime = DateTime.Now;
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            Progress += deltaTime / Duration;
            if (Progress >= 1.0f)
            {
                Progress = 1.0f;
                EndTime = DateTime.Now;
            }
        }

        public void Complete()
        {
            IsActive = false;
            Progress = 1.0f;
            EndTime = DateTime.Now;
        }

        public object Serialize()
        {
            return new { Id, Name, FromLevel, ToLevel, Duration, AnimationType, Rewards };
        }

        public void Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return;

            if (dict.TryGetValue("Id", out var id)) Id = (string)id;
            if (dict.TryGetValue("Name", out var name)) Name = (string)name;
            if (dict.TryGetValue("FromLevel", out var fromLevel)) FromLevel = (int)fromLevel;
            if (dict.TryGetValue("ToLevel", out var toLevel)) ToLevel = (int)toLevel;
            if (dict.TryGetValue("Duration", out var duration)) Duration = duration != null ? (float)duration : 0f;
            if (dict.TryGetValue("AnimationType", out var animationType)) AnimationType = (AnimationType)Enum.Parse<AnimationType>(animationType.ToString());
            if (dict.TryGetValue("Rewards", out var rewardsData) && rewardsData is List<object>)
            {
                Rewards = [.. ((List<object>)rewardsData)
    .Select(r =>
    {
        var reward = new ProgressionReward();
        reward.Deserialize(r);
        return reward;
    })];
            }
        }
    }

    /// <summary>
    /// Level up effect for visual and audio feedback.
    /// </summary>
    public class LevelUpEffect
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EffectType Type { get; set; }
        public float Duration { get; set; }
        public bool IsActive { get; set; }
        public float Progress { get; set; }

        // Visual effect properties
        public System.Drawing.Color Color { get; set; }
        public float Intensity { get; set; }

        // Particle effect properties
        public int ParticleCount { get; set; }
        public System.Drawing.Color ParticleColor { get; set; }

        // Audio effect properties
        public string SoundPath { get; set; }

        public void Start()
        {
            IsActive = true;
            Progress = 0f;
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            Progress += deltaTime / Duration;
            if (Progress >= 1.0f)
            {
                Progress = 1.0f;
            }
        }

        /// <summary>
        /// Gets the level this effect applies to.
        /// Adapts parameterless level conversion calls to the effect system.
        /// </summary>
        /// <returns>The level this effect targets.</returns>
        public int ToLevel()
        {
            // Extract level from the effect ID or name
            // This is a placeholder implementation - in a full system,
            // this would parse the level from the effect data
            if (int.TryParse(Id?.Split('_').LastOrDefault(), out int level))
            {
                return level;
            }
            return 1; // Default fallback
        }

        /// <summary>
        /// Gets the level this effect starts from.
        /// Adapts parameterless level conversion calls to the effect system.
        /// </summary>
        /// <returns>The level this effect starts from.</returns>
        public int FromLevel()
        {
            // Extract starting level from the effect ID or name
            // This is a placeholder implementation - in a full system,
            // this would parse the starting level from the effect data
            if (int.TryParse(Id?.Split('_').FirstOrDefault(), out int level))
            {
                return level;
            }
            return 1; // Default fallback
        }

        public void Stop()
        {
            IsActive = false;
            Progress = 0f;
        }

        public object Serialize()
        {
            return new { Id, Name, Description, Type, Duration, Color, Intensity, ParticleCount, ParticleColor, SoundPath };
        }

        public void Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return;

            if (dict.TryGetValue("Id", out var id)) Id = (string)id;
            if (dict.TryGetValue("Name", out var name)) Name = (string)name;
            if (dict.TryGetValue("Description", out var description)) Description = (string)description;
            if (dict.TryGetValue("Type", out var type)) Type = (EffectType)Enum.Parse<EffectType>(type.ToString());
            if (dict.TryGetValue("Duration", out var duration)) Duration = duration != null ? (float)duration : 0f;
            if (dict.TryGetValue("Color", out var color)) Color = color != null ? System.Drawing.Color.FromArgb((int)color) : System.Drawing.Color.White;
            if (dict.TryGetValue("Intensity", out var intensity)) Intensity = intensity != null ? (float)intensity : 0f;
            if (dict.TryGetValue("ParticleCount", out var particleCount)) ParticleCount = particleCount != null ? (int)particleCount : 0;
            if (dict.TryGetValue("ParticleColor", out var particleColor)) ParticleColor = particleColor != null ? System.Drawing.Color.FromArgb((int)particleColor) : System.Drawing.Color.White;
            if (dict.TryGetValue("SoundPath", out var soundPath)) SoundPath = (string)soundPath;
        }
    }

    /// <summary>
    /// Level up controller statistics.
    /// </summary>
    public class LevelUpControllerStatistics
    {
        public int TotalAnimations { get; set; }
        public int ActiveAnimations { get; set; }
        public int TotalEffects { get; set; }
        public int ActiveEffects { get; set; }
        public bool IsLevelUpInProgress { get; set; }
        public int TotalLevelUpsCompleted { get; set; }
        public float AverageLevelUpTime { get; set; }
    }

    /// <summary>
    /// Animation type enumeration.
    /// </summary>
    public enum AnimationType
    {
        LevelUp,
        Visual,
        Particle,
        Audio,
        Screen
    }

    /// <summary>
    /// Effect type enumeration.
    /// </summary>
    public enum EffectType
    {
        Visual,
        Particle,
        Audio,
        Screen
    }
}

namespace SASZombieAssaultTD.Engine.Managers
{
    public class EconomyManager
    {
        private static readonly EconomyManager _instance = new EconomyManager();
        internal int CurrentCash;

        public static EconomyManager Instance => _instance;
        private EconomyManager() { }
        public void AddCash(int amount) { /* Implementation */ }

        internal void Earn(int refundAmount)
        {
            throw new NotImplementedException();
        }
    }
}
