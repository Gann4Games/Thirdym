using UnityEngine;

namespace Gann4Games.Thirdym.ShootSystem
{

    /// <summary>
    /// Defines the behaviour of a weapon
    /// </summary>
    public class ShootBehaviour : ScriptableObject
    {
        public virtual void Fire(ShootBehaviourLoader source)
        {
            Debug.Log("[ShootBehaviour] base.Fire()");
        }
    }
}
