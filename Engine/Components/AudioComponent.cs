// =====================================================================================================
//  FILE: AudioComponent.cs
//  PATH: Engine/Components/AudioComponent.cs
//  SUBSYSTEM: Components
//
//  ROLE:
//      Defines the standalone audio state objects used exclusively by the audio pipeline.
//      These objects represent active sound instances managed by CoreAudioEngine.
//
//  RESPONSIBILITIES:
//      - Store deterministic audio playback state.
//      - Provide a clean, minimal data surface for CoreAudioEngine.
//      - Remain independent from ECS, entities, and runtime systems.
//
//  NON-RESPONSIBILITIES:
//      - Entity lifecycle
//      - ECS component storage
//      - System execution
//      - Runtime orchestration
//
//  ARCHITECTURAL NOTES:
//      - AudioComponent is NOT an ECS component.
//      - AudioComponent is owned solely by CoreAudioEngine.
//      - CoreAudioEngine manages creation, update, and destruction of AudioComponent instances.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Standalone deterministic audio playback state.
    /// Used exclusively by CoreAudioEngine.
    /// </summary>
    public sealed class AudioComponent
    {
        private object _components;
        private object _activeSounds;

        public uint SoundId { get; set; }

        public Vector3 Position { get; set; }

        public float Volume { get; set; }
        public object Values { get; set; }

        public float Pitch { get; set; }

        public float Pan { get; set; }

        public bool IsLooping { get; set; }

        public bool IsPaused { get; set; }

        public float CurrentTime { get; set; }

        public float Duration { get; set; }

        public uint PlaybackCount { get; set; }
        //private List<object> GetAllActiveSounds = new();
        ///<summary>
        /// new data
        ///</summary>




    }
}
