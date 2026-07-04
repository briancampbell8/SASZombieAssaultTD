//-----------------------------------------------------------------------------
//Animation ECS integration
//Namespace: SASZombieAssaultTD.Engine.Animation.Events
//-----------------------------------------------------------------------------
using System;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Animation.Systems;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Integration
{
    ///<summary>
    ///Game event types for animation system.
    ///</summary>
    public static class GameEvent
    {
        public static void TriggerAnimation(string animationName, Entity target)
        {
            //Animation trigger logic
        }
    }

    ///<summary>
    ///Game event data for animation system.
    ///</summary>
    public class GameEventData
    {
        public string EventName { get; set; }
        public Entity Target { get; set; }

        public GameEventData(string eventName, Entity target)
        {
            EventName = eventName;
            Target = target;
        }
    }

    ///<summary>
    ///Bridges ECS events and the animation system (subscription, dispatch, etc.).
    ///Provides a structural integration point for animation-related systems and events.
    ///</summary>
    public static class AnimationECSIntegration
    {
        ///<summary>
        ///Registers animation-related ECS systems and event handlers.
        ///</summary>
        ///<param name="world">The ECS world to register systems and handlers into.</param>
        public static void Register(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world), "ECSWorld cannot be null.");

            //TODO: Fix missing constructor arguments - AnimationTriggerSystem requires EntityManager and EventRouter
            //world.AddSystem(new AnimationTriggerSystem());
            //world.AddSystem(new AnimationUpdateSystem());
            System.Diagnostics.Debug.WriteLine("AnimationECSIntegration: Systems not registered - missing constructor arguments");

            //Example: Subscribe to ECS events
            world.EventManager.Subscribe<GameEventData>(OnGameEventReceived);

            System.Diagnostics.Debug.WriteLine("AnimationECSIntegration: Animation systems and event handlers registered.");
        }

        ///<summary>
        ///Unregisters animation-related ECS systems and event handlers.
        ///</summary>
        ///<param name="world">The ECS world to unregister systems and handlers from.</param>
        public static void Unregister(ECSWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world), "ECSWorld cannot be null.");

            //TODO: Fix generic type constraint - AnimationTriggerSystem and AnimationUpdateSystem don't implement IECSSystem
            //world.RemoveSystem<AnimationTriggerSystem>();
            //world.RemoveSystem<AnimationUpdateSystem>();

            //Example: Unsubscribe from ECS events
            world.EventManager.Unsubscribe<GameEventData>(OnGameEventReceived);

            System.Diagnostics.Debug.WriteLine("AnimationECSIntegration: Animation systems and event handlers unregistered.");
        }

        ///<summary>
        ///Handles game events and dispatches them to the animation system.
        ///</summary>
        ///<param name="gameEvent">The game event received.</param>
        private static void OnGameEventReceived(GameEventData gameEvent)
        {
            if (gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent), "GameEvent cannot be null.");

            //Example: Dispatch the event to the animation system
            System.Diagnostics.Debug.WriteLine($"AnimationECSIntegration: GameEvent received - {gameEvent.EventName}");
        }
    }
}
