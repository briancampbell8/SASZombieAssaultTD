/*
    File:    DamageResolver.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Resolves and clamps damage values based on base damage and multipliers.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using System;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Resolves damage interactions between attackers and targets.
    /// </summary>
    public static class DamageResolver
    {
        public static int ApplyDamage(int baseDamage, float multiplier = 1f)
        {
            if (baseDamage < 0)
                baseDamage = 0;

            if (multiplier < 0f)
                multiplier = 0f;

            var result = (int)(baseDamage * multiplier);

            return result < 0 ? 0 : result;
        }
    }
}