using Gann4Games.Thirdym.Core;
using UnityEngine;

namespace Gann4Games.Thirdym.ShootSystem
{
    /// <summary>
    /// Defines the behaviour of a weapon that deals damage around a certain area
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Shooting/Healer Behaviour", fileName = "Defibrilator Behaviour")]
    public class DefibrilatorBehaviour : ShootBehaviour
    {
        public override void Fire(ShootBehaviourLoader source)
        {
            Debug.Log("Defibrilator getting executed");
            Collider[] cols = Physics.OverlapSphere(source.transform.position, 2);
            foreach(Collider col in cols)
            {   
                if(col.TryGetComponent(out RagdollController ragdoll))
                {
                    if(!ragdoll.HealthController.IsInjured) continue;
                    ragdoll.HealthController.AddHealth(source.weaponData.weaponDamage);
                    return;
                }
            }
            base.Fire(source);
        }
    }
}
