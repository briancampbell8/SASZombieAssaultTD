using System;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Represents the movement capabilities of an entity.
    ///</summary>
    public class MovementComponent : BaseComponent
    {
    //Numeric standardization: All continuous values use double for precision
        private Vector3 _velocity;
        private double _speed;
        private Vector3 _direction;

        ///<summary>
        ///Gets or sets the velocity of the entity.
        ///</summary>
        public Vector3 Velocity 
        { 
            get => _velocity; 
            set => _velocity = value; 
        }

        ///<summary>
        ///Gets or sets the speed of the entity.
        ///</summary>
        public double Speed 
        { 
            get => _speed; 
            set => _speed = System.Math.Max(0.0, value); 
        }

        ///<summary>
        ///Gets or sets the direction of the entity.
        ///</summary>
        public Vector3 Direction 
        { 
            get => _direction; 
            set => _direction = value; 
        }

        ///<summary>
        ///Gets or sets the entity that owns this component.
        ///</summary>
        public Entity Owner { get; set; }

        ///<summary>
        ///Gets or sets whether the component is active.
        ///</summary>
        public bool IsActive { get; set; } = true;

        ///<summary>
        ///Moves the entity in the specified direction.
        ///</summary>
        ///<param name="direction">Direction to move (Z component ignored for 2D).</param>
        ///<param name="deltaTime">Time since last frame.</param>
        public void Move(Vector3 direction, float deltaTime)
        {
            _direction = new Vector3(direction.X, direction.Y, 0);
            _velocity = new Vector3((float)(direction.X * _speed), (float)(direction.Y * _speed), 0);
        }

        ///<summary>
        ///Stops the entity's movement.
        ///</summary>
        public void Stop()
        {
            _velocity = Vector3.Zero;
            _direction = Vector3.Zero;
        }

        ///<summary>
        ///Called when the component is added to an entity.
        ///</summary>
        public void OnAdded()
        {
            //Component added logic
        }

        ///<summary>
        ///Called when the component is removed from an entity.
        ///</summary>
        public void OnRemoved()
        {
            //Component removed logic
        }
    }
}
