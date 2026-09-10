/*
File:    HazardRegistration.cs
Path:    Engine/HazardsControl/HazardRegistration.cs
Purpose:  Adding, removing, and validating hazards.
          Manages hazard creation, destruction, and validation.

Role:     Registration manager for hazard systems.
          - Validates hazard properties and constraints
          - Assigns unique identifiers to hazards
          - Manages hazard type registration
          - Controls hazard creation/destruction

Notes:    This file owns all creation/destruction logic.
          All hazard validation logic is centralized here.
          Single responsibility: registration management.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Registration manager for hazard systems.
    ///Manages hazard creation, destruction, and validation.
    ///</summary>
    public class HazardRegistration
    {
        private readonly Dictionary<int, Hazard> _registeredHazards = new();
        private readonly Dictionary<string, HazardType> _registeredTypes = new();
        private int _nextHazardId = 1;
        private bool _isInitialized;

        ///<summary>
        ///Initializes the hazard registration system.
        ///</summary>
        public void Init()
        {
            _registeredHazards.Clear();
            _registeredTypes.Clear();
            _isInitialized = true;
        }

        ///<summary>
        ///Adds a hazard to the registration system.
        ///</summary>
        ///<param name="hazard">The hazard to add.</param>
        ///<returns>True if successfully added.</returns>
        public bool AddHazard(Hazard hazard)
        {
            if (!_isInitialized || hazard == null || _registeredHazards.ContainsKey(hazard.Id))
                return false;

            _registeredHazards[hazard.Id] = hazard;
            OnHazardRegistered?.Invoke(hazard);
            return true;
        }

        ///<summary>
        ///Removes a hazard by ID.
        ///</summary>
        ///<param name="hazardId">The ID of the hazard to remove.</param>
        ///<returns>True if successfully removed.</returns>
        public bool RemoveHazard(int hazardId)
        {
            if (!_isInitialized || !_registeredHazards.Remove(hazardId, out var hazard))
                return false;

            OnHazardUnregistered?.Invoke(hazard);
            return true;
        }

        ///<summary>
        ///Validates a hazard for registration.
        ///</summary>
        ///<param name="hazard">The hazard to validate.</param>
        ///<returns>True if hazard is valid for registration.</returns>
        public bool ValidateHazard(Hazard hazard)
        {
            if (!_isInitialized || hazard == null)
                return false;

            return !string.IsNullOrWhiteSpace(hazard.Type)
                   && hazard.Position.X >= 0
                   && hazard.Position.Y >= 0
                   && hazard.Radius > 0
                   && hazard.MaxLifetime > 0
                   && hazard.CurrentIntensity > 0
                   && ValidateHazardType(hazard.Type, hazard);
        }

        ///<summary>
        ///Assigns a unique hazard ID.
        ///</summary>
        ///<returns>A unique hazard ID, or null if unavailable.</returns>
        public int? AssignHazardId()
        {
            if (!_isInitialized)
                return null;

            while (_registeredHazards.ContainsKey(_nextHazardId))
            {
                if (_nextHazardId == int.MaxValue)
                    return null;

                _nextHazardId++;
            }

            return _nextHazardId++;
        }

        ///<summary>
        ///Registers a hazard type.
        ///</summary>
        ///<param name="type">The hazard type to register.</param>
        public void RegisterHazardType(string type)
        {
            if (!_isInitialized || string.IsNullOrWhiteSpace(type))
                return;

            if (_registeredTypes.TryGetValue(type, out var hazardType))
            {
                hazardType.UsageCount++;
            }
            else
            {
                _registeredTypes[type] = new HazardType
                {
                    Name = type,
                    RegisteredAt = DateTime.Now,
                    UsageCount = 1
                };

                OnHazardTypeRegistered?.Invoke(type);
            }
        }

        ///<summary>
        ///Gets a registered hazard by ID.
        ///</summary>
        ///<param name="hazardId">The hazard ID.</param>
        ///<returns>The hazard, or null if not found.</returns>
        public Hazard GetHazard(int hazardId)
        {
            return _isInitialized && _registeredHazards.TryGetValue(hazardId, out var hazard) ? hazard : null;
        }

        ///<summary>
        ///Gets all registered hazards.
        ///</summary>
        ///<returns>Copy of all registered hazards.</returns>
        public List<Hazard> GetAllHazards()
        {
            return _isInitialized ? new List<Hazard>(_registeredHazards.Values) : new List<Hazard>();
        }

        ///<summary>
        ///Gets a registered hazard type.
        ///</summary>
        ///<param name="type">The hazard type name.</param>
        ///<returns>The hazard type, or null if not found.</returns>
        public HazardType GetHazardType(string type)
        {
            return _isInitialized && _registeredTypes.TryGetValue(type, out var hazardType) ? hazardType : null;
        }

        ///<summary>
        ///Validates hazard type specific properties.
        ///</summary>
        ///<param name="type">The hazard type.</param>
        ///<param name="hazard">The hazard instance.</param>
        ///<returns>True if type-specific validation passes.</returns>
        private static bool ValidateHazardType(string type, Hazard hazard)
        {
            return type.ToLowerInvariant() switch
            {
                "nuke" => hazard.Radius is >= 100 and <= 1000,
                "radiation" => hazard.CurrentIntensity is >= 0.5f and <= 5f,
                "fire" => hazard.MaxLifetime is >= 5f and <= 60f,
                "chemical" => hazard.Radius is >= 50 and <= 500,
                _ => true //Unknown types pass basic validation
            };
        }

        ///<summary>
        ///Cleans up the hazard registration system.
        ///</summary>
        public void Cleanup()
        {
            _registeredHazards.Clear();
            _registeredTypes.Clear();
            _isInitialized = false;
        }

        ///<summary>
        ///Event triggered when a hazard is registered.
        ///</summary>
        public event Action<Hazard> OnHazardRegistered;

        ///<summary>
        ///Event triggered when a hazard is unregistered.
        ///</summary>
        public event Action<Hazard> OnHazardUnregistered;

        ///<summary>
        ///Event triggered when a hazard type is registered.
        ///</summary>
        public event Action<string> OnHazardTypeRegistered;
    }

    ///<summary>
    ///Information about a registered hazard type.
    ///</summary>
    public class HazardType
    {
        public string Name { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int UsageCount { get; set; }
    }
}
