/*
File:    HazardCleanup.cs
Path:    Engine/HazardsControl/HazardCleanup.cs
Purpose:  Automatic removal and memory cleanup.
          Manages hazard expiration, bounds checking, and pruning.

Role:     Cleanup manager for hazard systems.
          - Removes expired hazards
          - Cleans up out-of-bounds hazards
          - Prunes inactive hazard data
          - Prevents memory leaks

Notes:    This file ensures hazards don't accumulate or leak.
          All cleanup logic is centralized here.
          Single responsibility: cleanup management.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Cleanup manager for hazard systems.
    /// Manages automatic removal and memory cleanup.
    /// </summary>
    public class HazardCleanup
    {
        private Rectangle _worldBounds = new Rectangle(0, 0, 10000, 10000); // Default world bounds
        private readonly List<int> _hazardsToRemove = new();
        private bool _isInitialized;

        /// <summary>
        /// Initializes the hazard cleanup system.
        /// </summary>
        public void Init()
        {
            _hazardsToRemove.Clear();
            _isInitialized = true;
        }

        /// <summary>
        /// Sets the world bounds for cleanup operations.
        /// </summary>
        public void SetWorldBounds(Rectangle bounds) => _worldBounds = bounds;

        /// <summary>
        /// Cleans up expired hazards.
        /// </summary>
        public int CleanupExpiredHazards(List<Hazard> hazards) =>
            CleanupHazards(hazards, ShouldRemoveExpired, OnHazardExpired);

        /// <summary>
        /// Cleans up out-of-bounds hazards.
        /// </summary>
        public int CleanupOutOfBoundsHazards(List<Hazard> hazards) =>
            CleanupHazards(hazards, IsOutOfBounds, OnHazardOutOfBounds);

        /// <summary>
        /// Cleans up inactive hazards.
        /// </summary>
        public int CleanupInactiveHazards(List<Hazard> hazards) =>
            CleanupHazards(hazards, ShouldRemoveInactive, OnHazardInactive);

        /// <summary>
        /// Prunes the hazard list for optimization.
        /// </summary>
        public int PruneHazardList(List<Hazard> hazards)
        {
            if (!_isInitialized || hazards == null) return 0;

            var originalCount = hazards.Count;

            hazards.RemoveAll(h => h == null);

            var seenIds = new HashSet<int>();
            hazards.RemoveAll(h => !seenIds.Add(h.Id));

            var prunedCount = originalCount - hazards.Count;
            if (prunedCount > 0)
            {
                OnHazardsPruned?.Invoke(prunedCount);
            }

            return prunedCount;
        }

        /// <summary>
        /// Performs a comprehensive cleanup pass.
        /// </summary>
        public int PerformComprehensiveCleanup(List<Hazard> hazards)
        {
            if (!_isInitialized || hazards == null) return 0;

            var totalRemoved = 0;
            totalRemoved += CleanupExpiredHazards(hazards);
            totalRemoved += CleanupOutOfBoundsHazards(hazards);
            totalRemoved += CleanupInactiveHazards(hazards);
            totalRemoved += PruneHazardList(hazards);

            OnComprehensiveCleanup?.Invoke(totalRemoved);
            return totalRemoved;
        }

        /// <summary>
        /// Performs cleanup on hazards.
        /// Adapts single-parameter cleanup calls to the comprehensive cleanup implementation.
        /// </summary>
        /// <param name="hazards">List of hazards to clean up.</param>
        public void PerformCleanup(List<Hazard> hazards)
        {
            PerformComprehensiveCleanup(hazards);
        }

        /// <summary>
        /// Gets cleanup statistics.
        /// </summary>
        public CleanupStatistics GetCleanupStatistics() => new()
        {
            WorldBounds = _worldBounds,
            IsInitialized = _isInitialized,
            LastCleanupTime = DateTime.Now
        };

        /// <summary>
        /// Cleans up the hazard cleanup system.
        /// </summary>
        public void Cleanup()
        {
            _hazardsToRemove.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// Event triggered when a hazard expires.
        /// </summary>
        public event Action<Hazard> OnHazardExpired;

        /// <summary>
        /// Event triggered when a hazard goes out of bounds.
        /// </summary>
        public event Action<Hazard> OnHazardOutOfBounds;

        /// <summary>
        /// Event triggered when a hazard becomes inactive.
        /// </summary>
        public event Action<Hazard> OnHazardInactive;

        /// <summary>
        /// Event triggered when hazards are removed.
        /// </summary>
        public event Action<Hazard> OnHazardRemoved;

        /// <summary>
        /// Event triggered when hazards are pruned.
        /// </summary>
        public event Action<int> OnHazardsPruned;

        /// <summary>
        /// Event triggered after comprehensive cleanup.
        /// </summary>
        public event Action<int> OnComprehensiveCleanup;

        #region Private Methods

        private int CleanupHazards(List<Hazard> hazards, Func<Hazard, bool> shouldRemove, Action<Hazard> onRemove)
        {
            if (!_isInitialized || hazards == null) return 0;

            _hazardsToRemove.Clear();

            foreach (var hazard in hazards)
            {
                if (hazard != null && shouldRemove(hazard))
                {
                    _hazardsToRemove.Add(hazard.Id);
                    onRemove?.Invoke(hazard);
                }
            }

            return RemoveMarkedHazards(hazards);
        }

        private bool ShouldRemoveExpired(Hazard hazard) =>
            hazard.State == HazardState.Expired ||
            (hazard.State == HazardState.Decaying && hazard.DecayProgress >= 1.0f);

        private bool IsOutOfBounds(Hazard hazard) =>
            hazard.Position.X < _worldBounds.X ||
            hazard.Position.Y < _worldBounds.Y ||
            hazard.Position.X > _worldBounds.X + _worldBounds.Width ||
            hazard.Position.Y > _worldBounds.Y + _worldBounds.Height;

        private bool ShouldRemoveInactive(Hazard hazard)
        {
            const float inactiveThreshold = 30f; // 30 seconds of inactivity
            return (hazard.State == HazardState.Pending && hazard.ActivationDelay > inactiveThreshold) ||
                   (hazard.State == HazardState.Active && hazard.CurrentIntensity <= 0.01f);
        }

        private int RemoveMarkedHazards(List<Hazard> hazards)
        {
            var removedCount = 0;

            for (int i = hazards.Count - 1; i >= 0; i--)
            {
                if (hazards[i] != null && _hazardsToRemove.Contains(hazards[i].Id))
                {
                    var removedHazard = hazards[i];
                    hazards.RemoveAt(i);
                    removedCount++;

                    OnHazardRemoved?.Invoke(removedHazard);
                }
            }

            return removedCount;
        }

        #endregion
    }

    /// <summary>
    /// Statistics for cleanup operations.
    /// </summary>
    public class CleanupStatistics
    {
        public Rectangle WorldBounds { get; set; }
        public bool IsInitialized { get; set; }
        public DateTime LastCleanupTime { get; set; }
    }
}
