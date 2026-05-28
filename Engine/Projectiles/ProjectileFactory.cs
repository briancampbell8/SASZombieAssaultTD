using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Performance;
using SASZombieAssaultTD.Engine.Projectiles;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    /// <summary>
    /// Factory for creating projectiles in SAS Zombie Assault TD.
    /// Provides methods for creating different types of projectiles with various configurations.
    /// </summary>
    public class ProjectileFactory
    {
        private readonly Dictionary<ProjectileType, ProjectileTemplate> _templates;
        private static ProjectileFactory _instance;
        private object TheType;
        private object TheMember;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static ProjectileFactory Instance => _instance ??= new ProjectileFactory();

        private ProjectileFactory()
        {
            _templates = new Dictionary<ProjectileType, ProjectileTemplate>();
            InitializeTemplates();
        }

        /// <summary>
        /// Create a projectile of the specified type.
        /// </summary>
        /// <param name="type">Type of projectile to create.</param>
        /// <param name="position">Position to create projectile at.</param>
        /// <param name="direction">Direction projectile should travel.</param>
        /// <param name="source">Source tower.</param>
        /// <param name="target">Target enemy (optional).</param>
        /// <returns>Created projectile, or null if failed.</returns>
        public Projectile CreateProjectile(ProjectileType type, Vector3 position, Vector3 direction, Tower source = null, Enemy target = null)
        {
            if (!_templates.TryGetValue(type, out var template))
            {
                System.Diagnostics.Debug.WriteLine($"No template found for projectile type: {type}");
                return null;
            }

            try
            {
                // Get projectile from pool
                var projectile = GetProjectileFromPool(type);
                if (projectile == null)
                    return null;

                // Apply template properties
                ApplyTemplate(projectile, template);

                // Override with specific parameters
                projectile.Position = position;
                projectile.Source = source;
                projectile.Target = target;
                projectile.Type = type;

                // Activate projectile
                projectile.Activate(position, direction, source, target);

                return projectile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating projectile: {ex.Message}");
                return null;
            }
        }

        private Projectile GetProjectileFromPool(ProjectileType type)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a custom projectile with specified properties.
        /// </summary>
        /// <param name="type">Base projectile type.</param>
        /// <param name="properties">Custom properties to apply.</param>
        /// <param name="position">Position to create projectile at.</param>
        /// <param name="direction">Direction projectile should travel.</param>
        /// <param name="source">Source tower.</param>
        /// <param name="target">Target enemy (optional).</param>
        /// <returns>Created projectile, or null if failed.</returns>
        public Projectile CreateCustomProjectile(ProjectileType type, ProjectileProperties properties, Vector3 position, Vector3 direction, Tower source = null, Enemy target = null)
        {
            var projectile = CreateProjectile(type, position, direction, source, target);
            if (projectile == null)
                return null;

            // Apply custom properties
            ApplyCustomProperties(projectile, properties);

            return projectile;
        }

        /// <summary>
        /// Create a burst of projectiles.
        /// </summary>
        /// <param name="type">Type of projectiles to create.</param>
        /// <param name="position">Center position.</param>
        /// <param name="count">Number of projectiles in burst.</param>
        /// <param name="spreadAngle">Spread angle in degrees.</param>
        /// <param name="source">Source tower.</param>
        /// <returns>List of created projectiles.</returns>
        public List<Projectile> CreateProjectileBurst(ProjectileType type, Vector3 position, int count, float spreadAngle, Tower source = null)
        {
            var projectiles = new List<Projectile>();

            if (count <= 0)
                return projectiles;

            try
            {
                var spreadRadians = MathF.PI * spreadAngle / 180f;
                float angleStep;
                float startAngle;

                if (count == 1)
                {
                    // Single projectile - center it
                    angleStep = 0f;
                    startAngle = 0f;
                }
                else
                {
                    angleStep = spreadRadians / (count - 1);
                    startAngle = -spreadRadians / 2;
                }

                for (int i = 0; i < count; i++)
                {
                    var angle = startAngle + (i * angleStep);
                    var direction = new Vector3(MathF.Cos(angle), MathF.Sin(angle), 0);

                    var projectile = CreateProjectile(type, position, direction, source);
                    if (projectile != null)
                        projectiles.Add(projectile);
                }

                return projectiles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating projectile burst: {ex.Message}");
                return projectiles;
            }
        }

        /// <summary>
        /// Create a ring of projectiles.
        /// </summary>
        /// <param name="type">Type of projectiles to create.</param>
        /// <param name="center">Center position of ring.</param>
        /// <param name="radius">Radius of ring.</param>
        /// <param name="count">Number of projectiles in ring.</param>
        /// <param name="source">Source tower.</param>
        /// <returns>List of created projectiles.</returns>
        public List<Projectile> CreateProjectileRing(ProjectileType type, Vector3 center, float radius, int count, Tower source = null)
        {
            var projectiles = new List<Projectile>();

            if (count <= 0)
                return projectiles;

            try
            {
                var angleStep = (2 * MathF.PI) / count;

                for (int i = 0; i < count; i++)
                {
                    var angle = i * angleStep;
                    var position = new Vector3(
                        center.X + MathF.Cos(angle) * radius,
                        center.Y + MathF.Sin(angle) * radius,
                        center.Z
                    );
                    var direction = (center - position).Normalized;

                    var projectile = CreateProjectile(type, position, direction, source);
                    if (projectile != null)
                    {
                        projectiles.Add(projectile);
                    }
                }

                return projectiles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating projectile ring: {ex.Message}");
                return projectiles;
            }
        }

        /// <summary>
        /// Create a homing projectile.
        /// </summary>
        /// <param name="type">Base projectile type.</param>
        /// <param name="position">Starting position.</param>
        /// <param name="target">Target to home towards.</param>
        /// <param name="source">Source tower.</param>
        /// <returns>Created homing projectile, or null if failed.</returns>
        public Projectile CreateHomingProjectile(ProjectileType type, Vector3 position, Enemy target, Tower source = null)
        {
            if (target == null)
                return null;

            var projectile = CreateProjectile(type, position, Vector3.Zero, source, target);
            if (projectile == null)
                return null;

            // Make projectile homing
            projectile.IsHoming = true;
            projectile.HomingStrength = 0.5f;
            projectile.HomingMaxTurnRate = MathF.PI; // 180 degrees per second

            return projectile;
        }

        /// <summary>
        /// Create a bouncing projectile.
        /// </summary>
        /// <param name="type">Base projectile type.</param>
        /// <param name="position">Starting position.</param>
        /// <param name="direction">Initial direction.</param>
        /// <param name="maxBounces">Maximum number of bounces.</param>
        /// <param name="source">Source tower.</param>
        /// <returns>Created bouncing projectile, or null if failed.</returns>
        public Projectile CreateBouncingProjectile(ProjectileType type, Vector3 position, Vector3 direction, int maxBounces, Tower source = null)
        {
            var projectile = CreateProjectile(type, position, direction, source);
            if (projectile == null)
                return null;

            // Make projectile bouncing
            projectile.IsBouncing = true;
            projectile.MaxBounces = maxBounces;
            projectile.CurrentBounces = 0;
            projectile.BounceDamping = 0.8f;

            return projectile;
        }

        /// <summary>
        /// Create a piercing projectile.
        /// </summary>
        /// <param name="type">Base projectile type.</param>
        /// <param name="position">Starting position.</param>
        /// <param name="direction">Direction to travel.</param>
        /// <param name="maxPierces">Maximum number of enemies to pierce.</param>
        /// <param name="source">Source tower.</param>
        /// <returns>Created piercing projectile, or null if failed.</returns>
        public Projectile CreatePiercingProjectile(ProjectileType type, Vector3 position, Vector3 direction, int maxPierces, Tower source = null)
        {
            var projectile = CreateProjectile(type, position, direction, source);
            if (projectile == null)
                return null;

            // Make projectile piercing
            projectile.IsPiercing = true;
            projectile.MaxPierces = maxPierces;
            projectile.CurrentPierces = 0;
            projectile.PierceDamageReduction = 0.7f; // 70% damage after first hit

            return projectile;
        }

        /// <summary>
        /// Register a custom projectile template.
        /// </summary>
        /// <param name="type">Projectile type.</param>
        /// <param name="template">Template to register.</param>
        public void RegisterTemplate(ProjectileType type, ProjectileTemplate template)
        {
            _templates[type] = template;
            System.Diagnostics.Debug.WriteLine($"Registered custom template for projectile type: {type}");
        }

        /// <summary>
        /// Get a projectile template.
        /// </summary>
        /// <param name="type">Projectile type.</param>
        /// <returns>Template for the type, or null if not found.</returns>
        public ProjectileTemplate GetTemplate(ProjectileType type)
        {
            return _templates.TryGetValue(type, out var template) ? template : null;
        }

        /// <summary>
        /// Initialize default projectile templates.
        /// </summary>
        private void InitializeTemplates()
        {
            // Bullet template
            _templates[ProjectileType.Bullet] = new ProjectileTemplate
            {
                Damage = 25,
                Speed = 15f,
                Range = 20f,
                Size = 0.2f,
                Color = Color.Yellow,
                HasSplash = false,
                Element = ElementDamageType.Physical,
                Lifetime = 3f,
                TrailEffect = null
            };

            // Rocket template
            _templates[ProjectileType.Rocket] = new ProjectileTemplate
            {
                Damage = 80,
                Speed = 8f,
                Range = 25f,
                Size = 0.4f,
                Color = Color.Red,
                HasSplash = true,
                SplashRadius = 3f,
                Element = ElementDamageType.Fire,
                Lifetime = 4f,
                TrailEffect = new TrailEffect(Color.Red, 0.3f)
            };

            // Grenade template
            _templates[ProjectileType.Grenade] = new ProjectileTemplate
            {
                Damage = 60,
                Speed = 6f,
                Range = 15f,
                Size = 0.3f,
                Color = Color.Orange,
                HasSplash = true,
                SplashRadius = 2.5f,
                Element = ElementDamageType.Physical,
                Lifetime = 3f,
                TrailEffect = null
            };

            // Laser template
            _templates[ProjectileType.Laser] = new ProjectileTemplate
            {
                Damage = 40,
                Speed = 30f,
                Range = 30f,
                Size = 0.1f,
                Color = Color.Cyan,
                HasSplash = false,
                Element = ElementDamageType.Lightning,
                Lifetime = 2f,
                TrailEffect = new TrailEffect(Color.Cyan, 0.2f)
            };

            // Plasma template
            _templates[ProjectileType.Plasma] = new ProjectileTemplate
            {
                Damage = 50,
                Speed = 12f,
                Range = 22f,
                Size = 0.35f,
                Color = Color.Purple,
                HasSplash = false,
                Element = ElementDamageType.Energy,
                Lifetime = 3f,
                TrailEffect = new TrailEffect(Color.Purple, 0.4f)
            };

            // Arrow template
            _templates[ProjectileType.Arrow] = new ProjectileTemplate
            {
                Damage = 35,
                Speed = 18f,
                Range = 28f,
                Size = 0.25f,
                Color = Color.Brown,
                HasSplash = false,
                Element = ElementDamageType.Physical,
                Lifetime = 2.5f,
                TrailEffect = null
            };

            // Magic template
            _templates[ProjectileType.Magic] = new ProjectileTemplate
            {
                Damage = 45,
                Speed = 10f,
                Range = 20f,
                Size = 0.3f,
                Color = Color.Magenta,
                HasSplash = false,
                Element = ElementDamageType.Magic,
                Lifetime = 3f,
                TrailEffect = new TrailEffect(Color.Magenta, 0.3f)
            };

            System.Diagnostics.Debug.WriteLine($"Initialized {_templates.Count} projectile templates");
        }

        /// <summary>
        /// Apply template properties to a projectile.
        /// </summary>
        private void ApplyTemplate(Projectile projectile, ProjectileTemplate template)
        {
            projectile.Damage = template.Damage;
            projectile.Speed = template.Speed;
            projectile.Range = template.Range;
            projectile.Size = template.Size;
            projectile.Color = template.Color;
            projectile.HasSplash = template.HasSplash;
            projectile.SplashRadius = template.SplashRadius;
            // Use the exposed Element property on the template (lighter than method call)
            projectile.Element = template.Element;
            projectile.Trail = template.TrailEffect;
        }

        /// <summary>
        /// Apply custom properties to a projectile.
        /// </summary>
        private void ApplyCustomProperties(Projectile projectile, ProjectileProperties properties)
        {
            if (properties.Damage.HasValue)
                projectile.Damage = properties.Damage.Value;
            if (properties.Speed.HasValue)
                projectile.Speed = properties.Speed.Value;
            if (properties.Range.HasValue)
                projectile.Range = properties.Range.Value;
            if (properties.Size.HasValue)
                projectile.Size = properties.Size.Value;
            if (properties.Color.HasValue)
                projectile.Color = properties.Color.Value;
            if (properties.HasSplash.HasValue)
                projectile.HasSplash = properties.HasSplash.Value;
            if (properties.SplashRadius.HasValue)
                projectile.SplashRadius = properties.SplashRadius.Value;
            if (properties.Element.HasValue)
                projectile.Element = properties.Element.Value;
            if (properties.Lifetime.HasValue)
                projectile.MaxLifetime = properties.Lifetime.Value;
        }

        /// <summary>
        /// Get a projectile from the appropriate pool.
        /// </summary>
        /// 
        public class ProjectileSystem
            
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string Version { get; set; }
            public string Website { get; set; }
            public string License { get; set; }
            = string.Empty;
            public string[] Tags { get; set; } = Array.Empty<string>();
            public bool IsEnabled { get; set; }
            public string[] Dependencies { get; set; } = Array.Empty<string>();
            public string[] DependenciesOptional { get; set; } = Array.Empty<string>();
            public string[] DependenciesHidden { get; set; } = Array.Empty<string>();

            public ProjectileSystem()
            {
                Name = string.Empty;
                Description = string.Empty;
                Author = string.Empty;
                Version = string.Empty;
                Website = string.Empty;
                License = string.Empty;
                Tags = Array.Empty<string>();
                IsEnabled = true;
                Dependencies = Array.Empty<string>();
                DependenciesOptional = Array.Empty<string>();
                DependenciesHidden = Array.Empty<string>();
            }
        }
      
           

                 
        
        }
    }

    /// <summary>
    /// Template for projectile properties.
    /// </summary>
    public class ProjectileTemplate
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public bool HasSplash { get; set; }
        public float SplashRadius { get; set; }

        // Expose element as an auto-property for cheaper access patterns
        public ElementDamageType Element { get; set; }

        public float Lifetime { get; set; }
        public TrailEffect TrailEffect { get; set; }
        public Sprite Sprite { get; set; }
        public string FireSound { get; set; }
        public string ImpactSound { get; set; }
    }

    /// <summary>
    /// Custom projectile properties.
    /// </summary>
    public class ProjectileProperties
    {
        public int? Damage { get; set; }
        public float? Speed { get; set; }
        public float? Range { get; set; }
        public float? Size { get; set; }
        public Color? Color { get; set; }
        public bool? HasSplash { get; set; }
        public float? SplashRadius { get; set; }
        public ElementDamageType? Element { get; set; }
        public float? Lifetime { get; set; }
        public Sprite Sprite { get; set; }
        public string FireSound { get; set; }
        public string ImpactSound { get; set; }
        public bool IsHoming { get; set; }
        public float HomingStrength { get; set; }
        public bool IsBouncing { get; set; }
        public int MaxBounces { get; set; }
        public bool IsPiercing { get; set; }
        public int MaxPierces { get; set; }
    }

