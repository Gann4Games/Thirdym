using UnityEngine;
using UnityEditor;
using Gann4Games.Thirdym.ScriptableObjects;
using Gann4Games.Thirdym.Utility;

namespace Gann4Games.Thirdym.Core
{
    public class ExplosionHandler : MonoBehaviour
    {
        public SO_ExplosionPreset explosiveData;

        public bool explodeOnStart;
        public bool explodeOnImpact;

        private void OnEnable()
        {
            if(explodeOnStart)
                Explode();
        }
        public void Explode()
        {
            Vector3 explosionCenter = transform.position + explosiveData.explosionOriginOffset;
            Collider[] colliders = Physics.OverlapSphere(explosionCenter, explosiveData.explosionRadius);
            foreach (Collider hit in colliders)
            {
                Rigidbody otherRigidbody = hit.GetComponent<Rigidbody>();
                CharacterBodypart otherBodypart = hit.GetComponent<CharacterBodypart>();
                if (otherRigidbody == null) continue;


                float distance = Vector3.Distance(otherRigidbody.transform.position, transform.position);
                if (distance > explosiveData.explosionRadius) continue;

                float explosionForce = explosiveData.explosionForce / distance;
                otherRigidbody.AddExplosionForce(explosionForce, explosionCenter, explosiveData.explosionRadius, 0, ForceMode.Impulse);


                if (otherBodypart == null) continue;
                otherBodypart.Ragdoll.HealthController.DealDamage(explosiveData.explosionDamage / distance, Vector3.zero);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (explodeOnImpact)
                Explode();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (explosiveData == null) return;

            float intensity = explosiveData.explosionDamage / explosiveData.explosionRadius;
            Gizmos.color = new Color(intensity, 0, 0);
            Gizmos.DrawWireSphere(transform.position + explosiveData.explosionOriginOffset, explosiveData.explosionRadius);

            Handles.Label(transform.position+Vector3.up, $"Intensity: {explosiveData.explosionForce}");
        }
#endif

    }
}
