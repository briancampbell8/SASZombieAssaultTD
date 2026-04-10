using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Represents the position of an entity.
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
