/*
File:    AIController.cs
Path:    Engine/AI/AIController.cs
Purpose: P11-09-01 - Executes AI behaviors using shared or external AIContext.
         Maintains behavior list and dispatches Tick calls to registered behaviors.

Role:    AI subsystem controller.
         - Stores behavior instances
         - Maintains shared AIContext
         - Dispatches Tick calls
         - Supports external context evaluation

Notes:   Duplicate AIContext and IAIBehavior definitions removed.
         Authoritative versions exist in AIContext.cs and IAIBehavior.cs.
         All logic in this file is deterministic and context-driven.
*/

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.AI
{
    ///<summary>
    ///Executes AI behaviors using shared or external AIContext instances.
    ///Maintains behavior list and dispatches Tick calls.
    ///</summary>
    public class AIController
    {
        //Stores registered behaviors
        private readonly List<IAIBehavior> _behaviors = new();

        //Shared context instance for internal updates
        private readonly AIContext _context = new AIContext();

        //Reference to enemy system
        private EnemySystem? _enemySystem;

        ///<summary>
        ///Sets enemy system reference.
        ///</summary>
        public void SetEnemySystem(EnemySystem enemySystem)
        {
            _enemySystem = enemySystem;
        }

        ///<summary>
        ///Adds behavior if not already present.
        ///</summary>
        public void AddBehavior(IAIBehavior behavior)
        {
            if (behavior != null && !_behaviors.Contains(behavior))
                _behaviors.Add(behavior);
        }

        ///<summary>
        ///Sets target in shared context.
        ///</summary>
        public void SetTarget(object? target)
        {
            _context.Target = target;
        }

        ///<summary>
        ///Clears target in shared context.
        ///</summary>
        public void AcquireTargets()
        {
            _context.Target = null;
        }

        ///<summary>
        ///Updates shared context and evaluates behaviors.
        ///</summary>
        public void Update(float deltaTime)
        {
            _context.DeltaTime = deltaTime;
            EvaluateBehaviors();
        }

        ///<summary>
        ///Executes Tick on all behaviors using shared context.
        ///</summary>
        public void EvaluateBehaviors()
        {
            for (int i = 0; i < _behaviors.Count; i++)
                _behaviors[i].Tick(_context);
        }

        ///<summary>
        ///Executes Tick on all behaviors using external context.
        ///</summary>
        public void Update(AIContext context)
        {
            foreach (var behavior in _behaviors)
                behavior.Tick(context);
        }
    }

    //Duplicate AIContext and IAIBehavior definitions removed.
}
