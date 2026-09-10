// ====================================================================================================
//  FILE: TransformComponent.cs
//  PATH: Engine/Components/TransformComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing world-space position. Stores X, Y, and Z coordinates
//      and exposes a Vector3 interface used by rendering, physics, and gameplay systems.
//
//  RESPONSIBILITIES:
//      - Store world-space position (X, Y, Z)
//      - Provide a Vector3 interface for systems that consume positional data
//      - Act as a lightweight data carrier for movement, rendering, and physics systems
//
//  NON-RESPONSIBILITIES:
//      - Executing movement logic or applying transforms
//      - Managing world-level ECSEntityCore lifecycle or ECS attachment
//      - Performing physics, collision, or animation calculations
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with minimal behavioral logic
//      - Integrates with multiple systems but does not implement them
// ====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Represents the position of an ECSEntityCore.
    /// </summary>
    public class TransformComponent : BaseComponent
    {
        public TransformComponent()
        {
        }

        public TransformComponent(Vector3 worldPosition)
        {
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        /// <summary>
        /// Gets whether this component has a valid value.
        /// </summary>
        public bool HasValue => true;

        /// <summary>
        /// Gets the value of this component.
        /// </summary>
        public TransformComponent Value => this;

        /// <summary>
        /// Gets or sets the position as a Vector3.
        /// </summary>
        public Vector3 Position
        {
            get => new Vector3(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
    }
}
