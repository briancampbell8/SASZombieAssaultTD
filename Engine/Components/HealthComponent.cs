using System;
using SASZombieAssaultTD.Engine.ECS;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Represents the health of an entity.
    ///</summary>
    public class HealthComponent : BaseComponent
    {
    //Numeric standardization: All continuous values use double for precision
        private double _currentHealth;
        private double _maxHealth;

        ///<summary>
        ///Gets or sets the current health of the entity.
        ///</summary>
        public double CurrentHealth 
        { 
            get => _currentHealth; 
            set => _currentHealth = System.Math.Max(0.0, value); 
        }

        ///<summary>
        ///Gets or sets the maximum health of the entity.
        ///</summary>
        public double MaxHealth 
        { 
            get => _maxHealth; 
            set => _maxHealth = System.Math.Max(1.0, value); 
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
        ///Gets whether this component has a valid value.
        ///</summary>
        public bool HasValue => true;

        ///<summary>
        ///Gets the value of this component.
        ///</summary>
        public HealthComponent Value => this;

        ///<summary>
        ///Applies damage to the entity.
        ///</summary>
        ///<param name="damage">Amount of damage to apply.</param>
        public void TakeDamage(float damage)
        {
            _currentHealth = System.Math.Max(0, _currentHealth - damage);
        }

        ///<summary>
        ///Heals the entity.
        ///</summary>
        ///<param name="amount">Amount of health to restore.</param>
        public void Heal(float amount)
        {
            _currentHealth = System.Math.Min(_maxHealth, _currentHealth + amount);
        }

        ///<summary>
        ///Fully restores health to maximum.
        ///</summary>
        public void FullRestore()
        {
            _currentHealth = _maxHealth;
        }

        ///<summary>
        ///Checks if the entity is alive.
        ///</summary>
        ///<returns>True if health is greater than 0.</returns>
        public bool IsAlive()
        {
            return _currentHealth > 0;
        }

        ///<summary>
        ///Gets whether the entity is dead.
        ///</summary>
        public bool IsDead => !IsAlive();

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
