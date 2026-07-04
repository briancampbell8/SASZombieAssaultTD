/*
File:    EnemyTypes.cs
Purpose: Shared namespace bridge to resolve circular dependencies.
Features: Defines common enemy-related types that can be referenced by both Enemy and Wave systems.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Shared
{
    ///<summary>
    ///Common enemy behavior modifier interface to avoid circular dependencies.
    ///</summary>
    public interface IEnemyBehaviorModifier
    {
        string Type { get; set; }
        float Magnitude { get; set; }
        float Duration { get; set; }
        void Apply(Enemy enemy);
    }

    ///<summary>
    ///Common visual effect interface to avoid circular dependencies.
    ///</summary>
    public interface IVisualEffect
    {
        string EffectType { get; set; }
        float Duration { get; set; }
        void Apply(Enemy enemy);
    }

    ///<summary>
    ///Forward declaration of Enemy for namespace resolution.
    ///</summary>
    public class Enemy
    {
        //This will be implemented in Enemy.cs
        //This is just a namespace bridge
    }
}
