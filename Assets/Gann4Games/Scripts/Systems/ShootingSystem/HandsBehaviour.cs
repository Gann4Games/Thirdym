using System.Linq;
using UnityEngine;

namespace Gann4Games.Thirdym.ShootSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Shooting/Hand behaviour", fileName = "Hand behaviour")]
    public class HandsBehaviour : ShootBehaviour
    {
        private RagdollController _myself;

        public override void Fire(ShootBehaviourLoader source)
        {
            if(!_myself) _myself = source.GetComponentInParent<RagdollController>();

            Collider[] colliders = Physics.OverlapSphere(source.transform.position, 1).Where(obj => obj.GetComponent<CharacterBodypart>()).ToArray();
            foreach(Collider col in colliders)
            {
                if(col.GetComponent<CharacterBodypart>().Ragdoll == _myself) continue;
                
                if(col.TryGetComponent(out Rigidbody body))
                {
                    body.velocity += (source.Ragdoll.transform.position - body.transform.position)*10;
                    return;
                }
            }
        }
    }
}
