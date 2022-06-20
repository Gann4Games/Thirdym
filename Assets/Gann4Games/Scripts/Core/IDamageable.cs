using UnityEngine;

namespace Gann4Games.Thirdym.Core
{
    /// <summary>
    /// List of possible damage types.
    /// </summary>
    public enum DamageType
    {
        Bullet,
        Blade,
        Acid,
        Collision
    }

    /// <summary>
    /// Interface definition for damageable elements.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Deals damage to the object.
        /// </summary>
        /// <param name="damage">Amount of damage to deal.</param>
        /// <param name="damageType">The type of damage it has received.</param>
        /// <param name="where">The source vector of the damage (useful for NPCs)</param>
        void DealDamage(float damage, DamageType damageType, Vector3 where);
    }
}
