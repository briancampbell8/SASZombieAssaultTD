using System;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Base class for all game events used by the state machine.
    /// P20-02-02: Provides common functionality for state machine events.
    /// </summary>
    public abstract class GameEvent
    {
        /// <summary>
        /// Timestamp when the event was created.
        /// </summary>
        public DateTime Timestamp { get; }
        
        /// <summary>
        /// Initializes a new game event with the current timestamp.
        /// </summary>
        protected GameEvent()
        {
            Timestamp = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Returns a string representation of the game event.
        /// </summary>
        /// <returns>String representation including event type and timestamp.</returns>
        public override string ToString()
        {
            return $"{GetType().Name}(Timestamp={Timestamp:HH:mm:ss.fff})";
        }
    }

    /// <summary>
    /// Event fired when an entity is killed with attribution information.
    /// P11-04-08: Extends GameEvent with kill attribution data for scoring and statistics.
    /// </summary>
    public class KillAttributedEvent : GameEvent
    {
        /// <summary>
        /// Gets the ID of the entity that was killed.
        /// </summary>
        public uint VictimId { get; }

        /// <summary>
        /// Gets the ID of the entity that caused the kill (may be 0 for environmental deaths).
        /// </summary>
        public uint KillerId { get; }

        /// <summary>
        /// Gets the type of death that occurred.
        /// </summary>
        public DeathType DeathType { get; }

        /// <summary>
        /// Gets the team affiliation of the victim.
        /// </summary>
        public string VictimTeam { get; }

        /// <summary>
        /// Gets the team affiliation of the killer.
        /// </summary>
        public string KillerTeam { get; }

        /// <summary>
        /// Gets the weapon or method used for the kill.
        /// </summary>
        public string WeaponType { get; }

        /// <summary>
        /// Gets the location where the kill occurred.
        /// </summary>
        public System.Numerics.Vector3 KillLocation { get; }

        /// <summary>
        /// Gets the amount of damage dealt in the killing blow.
        /// </summary>
        public float FinalDamage { get; }

        /// <summary>
        /// Gets whether this was a headshot or critical hit.
        /// </summary>
        public bool WasCritical { get; }

        /// <summary>
        /// Gets the distance between killer and victim at time of death.
        /// </summary>
        public float Distance { get; }

        /// <summary>
        /// Initializes a new KillAttributedEvent.
        /// </summary>
        /// <param name="victimId">ID of the victim entity.</param>
        /// <param name="killerId">ID of the killer entity.</param>
        /// <param name="deathType">Type of death.</param>
        /// <param name="victimTeam">Team affiliation of victim.</param>
        /// <param name="killerTeam">Team affiliation of killer.</param>
        /// <param name="weaponType">Weapon or method used.</param>
        /// <param name="killLocation">Location of the kill.</param>
        /// <param name="finalDamage">Damage dealt in killing blow.</param>
        /// <param name="wasCritical">Whether this was a critical hit.</param>
        /// <param name="distance">Distance between killer and victim.</param>
        public KillAttributedEvent(uint victimId, uint killerId, DeathType deathType, 
            string victimTeam, string killerTeam, string weaponType, 
            System.Numerics.Vector3 killLocation, float finalDamage, 
            bool wasCritical, float distance)
        {
            VictimId = victimId;
            KillerId = killerId;
            DeathType = deathType;
            VictimTeam = victimTeam ?? "Unknown";
            KillerTeam = killerTeam ?? "Unknown";
            WeaponType = weaponType ?? "Unknown";
            KillLocation = killLocation;
            FinalDamage = finalDamage;
            WasCritical = wasCritical;
            Distance = distance;
        }

        /// <summary>
        /// Returns a string representation of the kill attributed event.
        /// </summary>
        /// <returns>String representation including kill details.</returns>
        public override string ToString()
        {
            return $"KillAttributedEvent(Victim={VictimId}, Killer={KillerId}, Type={DeathType}, " +
                   $"Weapon={WeaponType}, Distance={Distance:F1}, Critical={WasCritical})";
        }
    }

    /// <summary>
    /// Defines the possible types of deaths in the game.
    /// Represents different death scenarios for tracking and statistics.
    /// </summary>
    public enum DeathType
    {
        Unknown,    // Death type is not specified or recognized.
        Headshot,   // Death caused by a headshot.
        Explosion,  // Death caused by an explosion.
        Bullet,     // Death caused by bullet damage.
        Fire,       // Death caused by fire damage.
        Melee,      // Death caused by melee attack.
        Poison,     // Death caused by poison damage.
        Electric,   // Death caused by electric damage.
        Fall,       // Death caused by falling.
        Drowning,   // Death caused by drowning.
        Other       // Other death types.
    }
}




