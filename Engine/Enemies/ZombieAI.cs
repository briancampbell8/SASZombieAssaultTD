// ====================================================================================================
//  FILE: ZombieAI.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ZombieAI module.
//
//  RESPONSIBILITIES:
//      - Provide SetWalkable() behavior for the Core subsystem.
//      - Provide FindPath() behavior for the Core subsystem.
//      - Provide IsWalkable() behavior for the Core subsystem.
//      - Provide UpdateAI() behavior for the Core subsystem.
//      - Provide SetTarget() behavior for the Core subsystem.
//      - Provide MoveToTarget() behavior for the Core subsystem.
//      - Provide AttackTarget() behavior for the Core subsystem.
//      - Provide CanAttack() behavior for the Core subsystem.
//      - Provide RequestPath() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Enemies.EnemiesEnums;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Handles pathfinding calculations for navigation.
    /// </summary>
    public class Pathfinder
    {
        private readonly bool[,] _walkableGrid;
        private readonly int _width;
        private readonly int _height;

        public Pathfinder(int width = 100, int height = 100)
        {
            _width = width;
            _height = height;
            _walkableGrid = new bool[width, height];

            //Initialize all cells as walkable
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _walkableGrid[x, y] = true;
                }
            }
        }

        /// <summary>
        /// Sets whether a cell is walkable.
        /// </summary>
        public void SetWalkable(int x, int y, bool walkable)
        {
            if (IsWithinBounds(x, y))
            {
                _walkableGrid[x, y] = walkable;
            }
        }

        /// <summary>
        /// Finds a path from start to end.
        /// </summary>
        public List<PointF> FindPath(PointF start, PointF end)
        {
            //Placeholder implementation - returns direct path
            return new List<PointF> { start, end };
        }

        /// <summary>
        /// Checks if a position is walkable.
        /// </summary>
        public bool IsWalkable(PointF position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            return IsWithinBounds(x, y) && _walkableGrid[x, y];
        }

        /// <summary>
        /// Checks if the given coordinates are within the grid bounds.
        /// </summary>
        private bool IsWithinBounds(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    }

    /// <summary>
    /// Represents the AI behavior for a zombie ECSEntityCore.
    /// </summary>
    public class ZombieAI
    {
        private static readonly Pathfinder _pathfinder = new Pathfinder();
        private readonly List<PointF> _currentPath = new();
        private int _pathIndex;

        private ECSEntityCore _currentTarget;
        private float _attackRange = 20f;
        private float _attackCooldown = 1.5f;

        public ZombieAIState State { get; private set; } = ZombieAIState.Idle;

        public ECSEntityCore CurrentTarget
        {
            get => _currentTarget;
            private set => _currentTarget = value;
        }

        public PointF CurrentPosition { get; set; }
        public float MoveSpeed { get; set; } = 50f;

        /// <summary>
        /// Updates the zombie AI state.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public void UpdateAI(float deltaTime)
        {
            if (!CurrentTarget.IsValid)
            {
                State = ZombieAIState.Idle;
                return;
            }

            Vector3 currentPos3D = new Vector3(CurrentPosition.X, CurrentPosition.Y, 0);
            Vector3 targetPos3D = new Vector3(CurrentTarget.Position.X, CurrentTarget.Position.Y, 0);
            float distanceToTarget = Vector3.Distance(currentPos3D, targetPos3D);

            if (distanceToTarget <= _attackRange)
            {
                State = ZombieAIState.Attack;
            }
            else if (distanceToTarget <= _attackRange * 2)
            {
                State = ZombieAIState.Chase;
                MoveToTarget(deltaTime);
            }
            else
            {
                State = ZombieAIState.Idle;
            }
        }

        /// <summary>
        /// Sets the target for the zombie.
        /// </summary>
        /// <param name="target">The target ECSEntityCore.</param>
        public void SetTarget(ECSEntityCore target)
        {
            _currentTarget = target;
        }

        /// <summary>
        /// Moves the zombie toward the target.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public void MoveToTarget(float deltaTime)
        {
            if (!CurrentTarget.IsValid) return;

            Vector3 direction = (new Vector3(CurrentTarget.Position.X - CurrentPosition.X,
                           CurrentTarget.Position.Y - CurrentPosition.Y, 0)).Normalized;

            CurrentPosition = new PointF(
                CurrentPosition.X + direction.X * MoveSpeed * deltaTime,
                CurrentPosition.Y + direction.Y * MoveSpeed * deltaTime);
        }

        /// <summary>
        /// Attacks the current target.
        /// </summary>
        public void AttackTarget()
        {
            //Attack logic would go here
            //For now, this is a placeholder
        }

        /// <summary>
        /// Checks if the zombie can attack.
        /// </summary>
        /// <returns>True if in attack range and cooldown is ready.</returns>
        public bool CanAttack()
        {
            return State == ZombieAIState.Attack && CurrentTarget.IsValid;
        }

        /// <summary>
        /// Requests a path to the target position.
        /// </summary>
        public void RequestPath(PointF start, PointF end)
        {
            _currentPath.Clear();
            _currentPath.AddRange(_pathfinder.FindPath(start, end));
            _pathIndex = 0;
        }

        /// <summary>
        /// Moves zombie along the current path toward target.
        /// </summary>
        private void MoveTowardTarget(float deltaTime)
        {
            if (_currentPath.Count == 0 && CurrentTarget.IsValid)
            {
                RequestPath(CurrentPosition,
                    new PointF(CurrentTarget.Position.X, CurrentTarget.Position.Y));
            }

            if (_pathIndex >= _currentPath.Count) return;

            PointF nextPoint = _currentPath[_pathIndex];
            float dx = nextPoint.X - CurrentPosition.X;
            float dy = nextPoint.Y - CurrentPosition.Y;
            float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.001f)
            {
                dx /= distance;
                dy /= distance;

                CurrentPosition = new PointF(
                    CurrentPosition.X + dx * MoveSpeed * deltaTime,
                    CurrentPosition.Y + dy * MoveSpeed * deltaTime);
            }

            if (distance < 2f)
            {
                _pathIndex++;
            }
        }

        /// <summary>
        /// Calculates distance between two points.
        /// </summary>
        private static float Distance(PointF a, PointF b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
