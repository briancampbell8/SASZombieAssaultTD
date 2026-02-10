/*
    File:    PathfindingSystem.cs
    Author:  BDC
    Created: 2026-02-09

    Purpose:
        Manages entity navigation paths and produces movement vectors
        consumed by AnimationSystem each frame.

    Notes:
        - Outputs movement vectors and MovementState per entity.
        - AnimationSystem reads movement output via GetMovement().
        - Lifecycle: Initialize → Update(deltaSeconds) → Shutdown.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Describes the movement state of a navigating entity.
    /// Consumed by <see cref="AnimationSystem"/> to derive animation state.
    /// </summary>
    public enum MovementState
    {
        Idle = 0,
        Moving,
        Arrived
    }

    /// <summary>
    /// A single waypoint in a navigation path.
    /// </summary>
    public readonly struct Waypoint
    {
        public float X { get; }
        public float Y { get; }

        public Waypoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Per-entity movement output produced by PathfindingSystem each frame.
    /// AnimationSystem reads this to update transforms and animation states.
    /// </summary>
    public sealed class MovementOutput
    {
        public float DeltaX { get; set; }
        public float DeltaY { get; set; }
        public MovementState State { get; set; }

        public MovementOutput()
        {
            DeltaX = 0f;
            DeltaY = 0f;
            State = MovementState.Idle;
        }
    }

    /// <summary>
    /// Internal tracking data for a navigating entity.
    /// </summary>
    internal sealed class NavigationAgent
    {
        public int EntityId { get; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; }
        public List<Waypoint> Path { get; }
        public int CurrentWaypointIndex { get; set; }
        public MovementState State { get; set; }

        public NavigationAgent(int entityId, float x, float y, float speed)
        {
            EntityId = entityId;
            X = x;
            Y = y;
            Speed = speed;
            Path = new List<Waypoint>();
            CurrentWaypointIndex = 0;
            State = MovementState.Idle;
        }
    }

    /// <summary>
    /// Manages entity navigation paths and produces movement vectors.
    /// Follows the standard subsystem lifecycle: Initialize, Update, Shutdown.
    /// </summary>
    public sealed class PathfindingSystem
    {
        private readonly Dictionary<int, NavigationAgent> _agents = new();
        private readonly Dictionary<int, MovementOutput> _movementOutputs = new();
        private bool _initialized;

        /// <summary>
        /// Read-only snapshot of movement outputs per entity, keyed by entity ID.
        /// AnimationSystem reads this each frame after PathfindingSystem.Update.
        /// </summary>
        public IReadOnlyDictionary<int, MovementOutput> MovementOutputs => _movementOutputs;

        /// <summary>
        /// Initializes the pathfinding system. Called once during engine startup.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _agents.Clear();
            _movementOutputs.Clear();
            _initialized = true;

            DebugLogger.Log(DebugLogger.Phase5,
                "[PathfindingSystem] Initialized.");
        }

        /// <summary>
        /// Registers a navigating entity with its starting position and speed.
        /// </summary>
        public void Register(int entityId, float x, float y, float speed)
        {
            if (speed <= 0f)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be positive.");

            if (_agents.ContainsKey(entityId))
                return;

            _agents[entityId] = new NavigationAgent(entityId, x, y, speed);
            _movementOutputs[entityId] = new MovementOutput();
        }

        /// <summary>
        /// Removes an entity from the pathfinding system.
        /// </summary>
        public void Unregister(int entityId)
        {
            _agents.Remove(entityId);
            _movementOutputs.Remove(entityId);
        }

        /// <summary>
        /// Sets a navigation path (list of waypoints) for an entity.
        /// The entity will begin moving toward the first waypoint on the next Update.
        /// </summary>
        public void SetPath(int entityId, IReadOnlyList<Waypoint> waypoints)
        {
            if (waypoints is null)
                throw new ArgumentNullException(nameof(waypoints));

            if (!_agents.TryGetValue(entityId, out NavigationAgent? agent))
                return;

            agent.Path.Clear();
            agent.Path.AddRange(waypoints);
            agent.CurrentWaypointIndex = 0;
            agent.State = waypoints.Count > 0 ? MovementState.Moving : MovementState.Idle;
        }

        /// <summary>
        /// Advances all agents along their paths and produces movement output.
        /// Called once per frame by GameRoot, before AnimationSystem.Update.
        /// </summary>
        public void Update(float deltaSeconds)
        {
            if (!_initialized)
                return;

            foreach (var kvp in _agents)
            {
                NavigationAgent agent = kvp.Value;
                MovementOutput output = _movementOutputs[agent.EntityId];

                // Reset movement output for this frame
                output.DeltaX = 0f;
                output.DeltaY = 0f;

                if (agent.State != MovementState.Moving ||
                    agent.CurrentWaypointIndex >= agent.Path.Count)
                {
                    if (agent.State == MovementState.Moving)
                        agent.State = MovementState.Arrived;

                    output.State = agent.State;
                    continue;
                }

                Waypoint target = agent.Path[agent.CurrentWaypointIndex];

                float dx = target.X - agent.X;
                float dy = target.Y - agent.Y;
                float distance = MathF.Sqrt(dx * dx + dy * dy);

                float step = agent.Speed * deltaSeconds;

                if (distance <= step)
                {
                    // Arrived at waypoint
                    output.DeltaX = dx;
                    output.DeltaY = dy;
                    agent.X = target.X;
                    agent.Y = target.Y;
                    agent.CurrentWaypointIndex++;

                    if (agent.CurrentWaypointIndex >= agent.Path.Count)
                    {
                        agent.State = MovementState.Arrived;
                    }
                }
                else
                {
                    // Move toward waypoint
                    float nx = dx / distance;
                    float ny = dy / distance;

                    output.DeltaX = nx * step;
                    output.DeltaY = ny * step;

                    agent.X += output.DeltaX;
                    agent.Y += output.DeltaY;
                }

                output.State = agent.State;
            }
        }

        /// <summary>
        /// Retrieves the movement output for a specific entity.
        /// Returns null if the entity is not registered.
        /// </summary>
        public MovementOutput? GetMovement(int entityId)
        {
            return _movementOutputs.TryGetValue(entityId, out MovementOutput? output) ? output : null;
        }

        /// <summary>
        /// Tears down the pathfinding system. Called during engine shutdown.
        /// </summary>
        public void Shutdown()
        {
            _agents.Clear();
            _movementOutputs.Clear();
            _initialized = false;

            DebugLogger.Log(DebugLogger.Phase5,
                "[PathfindingSystem] Shutdown.");
        }
    }
}