using UnityEngine;
using UnityEngine.Events;

namespace Gann4Games.Thirdym.Core
{
    /// <summary>
    /// Base class for breakable objects.
    /// Allows to manage object's health, on death event, damage dealing and model/effect swapping when broken.
    /// </summary>
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        [Tooltip("The object that will be enabled on death.")]
        [SerializeField] GameObject brokenEffect;

        [Tooltip("The object that will be disabled on death.")]
        [SerializeField] GameObject defaultObject;

        //[Tooltip("Use zero or less to define an unbreakable object.")]
        float startHealth;
        [Space]
        public UnityEvent onDeath;

        Rigidbody _rigidbody;
        Collider _collider;
        bool _unbreakable => startHealth <= 0;
        float _currentHealth;

        /// <summary>
        /// The current health of the object.
        /// </summary>
        public float Health => _currentHealth;

        /// <summary>
        /// Sets the health and maximum health of the object.
        /// Use zero or less to define an unbreakable object.
        /// </summary>
        /// <param name="value">The health to set</param>
        public void SetHealth(float value)
        {
            startHealth = value;
            _currentHealth = startHealth;
        }

        public virtual void Initialize()
        {
            if (TryGetComponent(out Rigidbody rb)) _rigidbody = rb;
            if (TryGetComponent(out Collider col)) _collider = col;
        }

        public void DealDamage(float damage, DamageType damageType, Vector3 where) {
            if (_unbreakable) return;

            _currentHealth -= damage;
            if (_currentHealth < 0) OnDeath();
        }

        public virtual void OnDeath() {
            onDeath.Invoke();
            Destroy(_rigidbody);
            Destroy(_collider);
            brokenEffect.SetActive(true);
            defaultObject.SetActive(false);
        }
    }
}
