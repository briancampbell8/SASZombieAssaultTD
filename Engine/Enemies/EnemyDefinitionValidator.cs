using System;
using SASZombieAssaultTD.Engine.Enemies;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Provides validation logic for enemy definitions.
    ///</summary>
    public static class EnemyDefinitionValidator
    {
        ///<summary>
        ///Validates the given enemy definition.
        ///</summary>
        ///<param name="def">The enemy definition to validate.</param>
        ///<returns>True if valid, false otherwise.</returns>
        ///<exception cref="ArgumentNullException">Thrown when the enemy definition is null.</exception>
        public static bool Validate(EnemyDefinition def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def), "Enemy definition cannot be null.");

            return !string.IsNullOrWhiteSpace(def.Id) &&
                   !string.IsNullOrWhiteSpace(def.Name) &&
                   def.MaxHealth > 0 &&
                   def.Speed > 0 &&
                   def.Reward >= 0 &&
                   !string.IsNullOrWhiteSpace(def.SpriteId);
        }
    }
}


