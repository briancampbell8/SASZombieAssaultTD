using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Base entity class for game objects.
    /// </summary>
    public class Entity
    {
        public uint Id { get; }
        public Vector3 Position { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public Scene? Scene { get; set; }

        public Entity(uint id)
        {
            Id = id;
            Position = Vector3.Zero;
        }

        public Entity(uint id, Vector3 position)
        {
            Id = id;
            Position = position;
        }

        public virtual void Update(float deltaTime)
        {
            // Base implementation - can be overridden by derived classes
        }

        public virtual void Render(IRenderContext context)
        {
            // Base implementation - can be overridden by derived classes
        }
    }

    /// <summary>
    /// Base scene class for managing game scenes.
    /// P20-07-02: Implements scene lifecycle management.
    /// </summary>
    public abstract class Scene
    {
        private string _name;
        private bool _isLoaded;
        private bool _isActive;
        private List<Entity> _entities;
        private Dictionary<string, object> _sceneData;

        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        public string Name
        {
            get => _name;
            protected set => _name = value ?? string.Empty;
        }

        /// <summary>
        /// Gets whether the scene is loaded.
        /// </summary>
        public bool IsLoaded => _isLoaded;

        /// <summary>
        /// Gets whether the scene is active.
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// Gets the list of entities in the scene.
        /// </summary>
        public IReadOnlyList<Entity> Entities => _entities.AsReadOnly();

        /// <summary>
        /// Gets the number of entities in the scene.
        /// </summary>
        public int EntityCount => _entities.Count;

        /// <summary>
        /// Event fired when scene is loaded.
        /// </summary>
        public event Action OnSceneLoaded;

        /// <summary>
        /// Event fired when scene is unloaded.
        /// </summary>
        public event Action OnSceneUnloaded;

        /// <summary>
        /// Event fired when scene becomes active.
        /// </summary>
        public event Action OnSceneActivated;

        /// <summary>
        /// Event fired when scene becomes inactive.
        /// </summary>
        public event Action OnSceneDeactivated;

        /// <summary>
        /// Event fired when an entity is added to the scene.
        /// </summary>
        public event Action<Entity> OnEntityAdded;

        /// <summary>
        /// Event fired when an entity is removed from the scene.
        /// </summary>
        public event Action<Entity> OnEntityRemoved;

        /// <summary>
        /// Initializes a new scene.
        /// </summary>
        /// <param name="name">The name of the scene.</param>
        protected Scene(string? name = null)
        {
            _name = name ?? GetType().Name;
            _isLoaded = false;
            _isActive = false;
            _entities = new List<Entity>();
            _sceneData = new Dictionary<string, object>();

            ModernLoggingSystem.Log("DEBUG", $"Scene: Created '{_name}'");
        }

        /// <summary>
        /// Loads the scene.
        /// P20-07-02: Implements scene loading functionality.
        /// </summary>
        /// <returns>True if loading succeeded.</returns>
        public bool Load()
        {
            if (_isLoaded)
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: '{_name}' is already loaded");
                return false;
            }

            try
            {
                // Load scene-specific resources
                var success = LoadSceneData();
                if (!success)
                {
                    ModernLoggingSystem.Log("ERROR", $"Scene: Failed to load data for '{_name}'");
                    return false;
                }

                // Initialize entities
                InitializeEntities();

                _isLoaded = true;
                OnSceneLoaded?.Invoke();

                ModernLoggingSystem.Log("INFO", $"Scene: Loaded '{_name}' with {_entities.Count} entities");
                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Scene: Failed to load '{_name}' - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unloads the scene.
        /// P20-07-02: Implements scene unloading functionality.
        /// </summary>
        /// <returns>True if unloading succeeded.</returns>
        public bool Unload()
        {
            if (!_isLoaded)
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: '{_name}' is not loaded");
                return false;
            }

            try
            {
                // Deactivate if currently active
                if (_isActive)
                {
                    Deactivate();
                }

                // Clear all entities
                ClearEntities();

                // Unload scene-specific resources
                UnloadSceneData();

                _isLoaded = false;
                OnSceneUnloaded?.Invoke();

                ModernLoggingSystem.Log("INFO", $"Scene: Unloaded '{_name}'");
                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Scene: Failed to unload '{_name}' - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Activates the scene.
        /// </summary>
        public void Activate()
        {
            if (!_isLoaded)
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: Cannot activate '{_name}' - not loaded");
                return;
            }

            if (_isActive)
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: '{_name}' is already active");
                return;
            }

            _isActive = true;
            OnSceneActivated?.Invoke();

            ModernLoggingSystem.Log("DEBUG", $"Scene: Activated '{_name}'");
        }

        /// <summary>
        /// Deactivates the scene.
        /// </summary>
        public void Deactivate()
        {
            if (!_isActive)
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: '{_name}' is not active");
                return;
            }

            _isActive = false;
            OnSceneDeactivated?.Invoke();

            ModernLoggingSystem.Log("DEBUG", $"Scene: Deactivated '{_name}'");
        }

        /// <summary>
        /// Updates the scene.
        /// P20-07-02: Implements scene update functionality.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        public virtual void Update(float deltaTime)
        {
            if (!_isLoaded || !_isActive)
                return;

            try
            {
                // Update all entities
                for (int i = _entities.Count - 1; i >= 0; i--)
                {
                    var entity = _entities[i];
                    if (entity.IsActive)
                    {
                        entity.Update(deltaTime);
                    }
                }

                // Update scene-specific logic
                UpdateScene(deltaTime);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Scene: Failed to update '{_name}' - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the scene.
        /// P20-07-02: Implements scene rendering functionality.
        /// </summary>
        /// <param name="renderer">The renderer to use.</param>
        public virtual void Render(Renderer renderer)
        {
            if (!_isLoaded)
                return;

            try
            {
                // Render all entities
                foreach (var entity in _entities)
                {
                    if (entity.IsVisible)
                    {
                        entity.Render((IRenderContext)renderer);
                    }
                }

                // Render scene-specific elements
                RenderScene(renderer);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Scene: Failed to render '{_name}' - {ex.Message}");
            }
        }

        /// <summary>
        /// Adds an entity to the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>True if entity was added successfully.</returns>
        public bool AddEntity(Entity entity)
        {
            if (entity == null)
            {
                ModernLoggingSystem.Log("WARNING", "Scene: Cannot add null entity");
                return false;
            }

            if (_entities.Contains(entity))
            {
                ModernLoggingSystem.Log("WARNING", $"Scene: Entity '{entity.Id}' already exists in scene '{_name}'");
                return false;
            }

            _entities.Add(entity);
            entity.Scene = this;
            OnEntityAdded?.Invoke(entity);

            ModernLoggingSystem.Log("DEBUG", $"Scene: Added entity '{entity.Id}' to '{_name}'");
            return true;
        }

        /// <summary>
        /// Removes an entity from the scene.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if entity was removed successfully.</returns>
        public bool RemoveEntity(Entity entity)
        {
            if (entity == null)
                return false;

            var removed = _entities.Remove(entity);
            if (removed)
            {
                entity.Scene = null;
                OnEntityRemoved?.Invoke(entity);
                ModernLoggingSystem.Log("DEBUG", $"Scene: Removed entity '{entity.Id}' from '{_name}'");
            }

            return removed;
        }

        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        /// <param name="id">The ID of the entity to find.</param>
        /// <returns>The entity, or null if not found.</returns>
        public Entity GetEntity(string id)
        {
            return _entities.Find(entity => entity.Id.ToString() == id);
        }

        /// <summary>
        /// Gets all entities of a specific type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>List of entities of the specified type.</returns>
        public List<T> GetEntities<T>() where T : Entity
        {
            var result = new List<T>();
            foreach (var entity in _entities)
            {
                if (entity is T typedEntity)
                {
                    result.Add(typedEntity);
                }
            }
            return result;
        }

        /// <summary>
        /// Sets scene data.
        /// </summary>
        /// <param name="key">The data key.</param>
        /// <param name="value">The data value.</param>
        public void SetData(string key, object value)
        {
            _sceneData[key] = value;
        }

        /// <summary>
        /// Gets scene data.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="key">The data key.</param>
        /// <returns>The data value, or default if not found.</returns>
        public T GetData<T>(string key)
        {
            if (_sceneData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        /// <summary>
        /// Clears all entities from the scene.
        /// </summary>
        private void ClearEntities()
        {
            foreach (var entity in _entities)
            {
                entity.Scene = null;
                OnEntityRemoved?.Invoke(entity);
            }
            _entities.Clear();
        }

        /// <summary>
        /// Initializes entities for the scene.
        /// </summary>
        private void InitializeEntities()
        {
            // Override in derived classes to create initial entities
            ModernLoggingSystem.Log("DEBUG", $"Scene: Initializing entities for '{_name}'");
        }

        /// <summary>
        /// Scene-specific loading logic.
        /// Override in derived classes.
        /// </summary>
        /// <returns>True if loading succeeded.</returns>
        protected virtual bool LoadSceneData()
        {
            // Override in derived classes for specific loading logic
            return true;
        }

        /// <summary>
        /// Scene-specific unloading logic.
        /// Override in derived classes.
        /// </summary>
        protected virtual void UnloadSceneData()
        {
            // Override in derived classes for specific unloading logic
        }

        /// <summary>
        /// Scene-specific update logic.
        /// Override in derived classes.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        protected virtual void UpdateScene(float deltaTime)
        {
            // Override in derived classes for specific update logic
        }

        /// <summary>
        /// Scene-specific render logic.
        /// Override in derived classes.
        /// </summary>
        /// <param name="renderer">The renderer to use.</param>
        protected virtual void RenderScene(Renderer renderer)
        {
            // Override in derived classes for specific render logic
        }

        /// <summary>
        /// Gets scene information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"Scene: Name='{_name}', Loaded={_isLoaded}, " +
            $"Active={_isActive}, Entities={_entities.Count}";
        }
    }
}




