using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.AI
{
    public class AIController
    {
        private readonly List<IAIBehavior> _behaviors = new();
        private readonly AIContext _context = new AIContext();
        private EnemySystem? _enemySystem;

        public void SetEnemySystem(EnemySystem enemySystem)
        {
            _enemySystem = enemySystem;
        }

        public void AddBehavior(IAIBehavior behavior)
        {
            if (behavior != null && !_behaviors.Contains(behavior))
                _behaviors.Add(behavior);
        }

        public void SetTarget(object? target)
        {
            _context.Target = target;
        }

        public void AcquireTargets()
        {
            _context.Target = null;
        }

        public void Update(float deltaTime)
        {
            _context.DeltaTime = deltaTime;
            EvaluateBehaviors();
        }

        public void EvaluateBehaviors()
        {
            for (int i = 0; i < _behaviors.Count; i++)
                _behaviors[i].Tick(_context);
        }

        public void Update(AIContext context)
        {
            foreach (var behavior in _behaviors)
                behavior.Tick(context);
        }
    }

    public class AIContext
    {
        public float DeltaTime { get; set; }
        public object? Owner { get; set; }
        public object? Target { get; set; }
    }

    public interface IAIBehavior
    {
        void Tick(AIContext context);
    }
}




