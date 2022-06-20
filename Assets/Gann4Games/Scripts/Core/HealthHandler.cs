using UnityEngine;
using System;

namespace Gann4Games.Thirdym.Utility
{
    public class HealthHandler : MonoBehaviour
    {
        public event EventHandler OnHealthZero;
        public event EventHandler OnHealthChanged;
        public event EventHandler OnHealthFull;
        public event EventHandler OnHealthIncrease;
        public event EventHandler OnHealthDecrease;

        [SerializeField] int health = 250;
        [SerializeField] int maximumHealth = 500;
        int CurrentHealth
        {
            get => Mathf.Clamp(health, 0, maximumHealth);
            set
            {
                health = Mathf.Clamp(value, 0, maximumHealth);
                HealthValidation();
            }
        }

        /// <summary>
        /// The percentage value based on maximum health and current health
        /// </summary>
        /// <returns>A value between 0 and 1</returns>
        public float HealthPercentage => (float)CurrentHealth / (float)maximumHealth;

        /// <summary>
        /// This function is responsible for executing events based on the health value.
        /// </summary>
        private void HealthValidation()
        {
            OnHealthChanged?.Invoke(this, EventArgs.Empty);

            if (health <= 0)
            {
                OnHealthZero?.Invoke(this, EventArgs.Empty);
            }
            else if (health >= maximumHealth)
            {
                OnHealthFull?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Executes required validation on editor for easy and intuitive field editing
        /// </summary>
        private void OnValidate() => health = CurrentHealth;

        public void IncreaseHealth(int amount)
        {
            CurrentHealth += amount;
            OnHealthIncrease?.Invoke(this, EventArgs.Empty);
        }
        public void DecreaseHealth(int amount)
        {
            CurrentHealth -= amount;
            OnHealthDecrease?.Invoke(this, EventArgs.Empty);
        }
    }
}
