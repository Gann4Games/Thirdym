using UnityEngine;
using Gann4Games.Thirdym.ScriptableObjects;
using Gann4Games.Thirdym.Core;
using System.Collections;

public class Bullet : MonoBehaviour {
    public SO_WeaponPreset weapon;

    [SerializeField] private GameObject energyImpact;
    [SerializeField] private GameObject bulletHole;
    [SerializeField] private float bulletSpeed = 10;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private TrailRenderer _trailRenderer;
    private Vector3 _fireDirection;

    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        LoadVisualsFrom(weapon);

        SetBulletDirection(transform.right);
        Destroy(gameObject, weapon.bulletDespawnTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Deal damage if it is damageable
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.DealDamage(weapon.weaponDamage, DamageType.Bullet, transform.position);

        // Apply force if it has dynamics (a rigidbody)
        if(other.gameObject.TryGetComponent(out Rigidbody otherRigidbody))
            otherRigidbody.AddForce(transform.forward * (50 * weapon.weaponDamage));

        if (weapon.useRicochet) DoRicochet();
        else DoImpact();

        // TODO: Delegate particle spawning to breakable and/or damageable objects.
    }

    /// <summary>
    /// Loads visuals from a weapon's scriptable object parameters.
    /// </summary>
    /// <param name="preset">The weapon's scriptable object.</param>
    private void LoadVisualsFrom(SO_WeaponPreset preset)
    {
        _trailRenderer.startColor = preset.weaponBullet.bulletColor;
        _trailRenderer.endColor = preset.weaponBullet.bulletColor;
        _trailRenderer.startWidth = preset.weaponBullet.bulletWidth;
        _trailRenderer.endWidth = preset.weaponBullet.bulletWidth;
        _trailRenderer.time = preset.weaponBullet.bulletLenght;
        _trailRenderer.material = preset.weaponBullet.bulletMaterial;
        _trailRenderer.textureMode = preset.weaponBullet.textureMode;
    }

    private void SetBulletDirection(Vector3 direction)
    {
        _fireDirection = direction;
        direction.Normalize();
        transform.LookAt(transform.position + direction);
        _rigidbody.velocity = direction * bulletSpeed * Time.deltaTime;
    }

    public void InvertDirection() => SetBulletDirection(-_rigidbody.velocity);

    private void DoRicochet()
    {
        Vector3 incomingDirection = _fireDirection.normalized;
        float ricochetAngle = 0;
        if(Physics.Raycast(transform.position, incomingDirection, out RaycastHit hit))
        {
            //if(hit.transform.CompareTag("Map")) BulletHole(hit.point, hit.normal);
            
            Vector3 bounceDirection = Vector3.Reflect(incomingDirection, hit.normal).normalized;
            ricochetAngle = Vector3.Dot(incomingDirection, bounceDirection);
            if(ricochetAngle > weapon.ricochetMinAngle)
            {
                SetBulletDirection(bounceDirection);
            }
            else
            {
                StartCoroutine(DisablePhysicsCoroutine());
                //Destroy(gameObject);
            }
        }
    }
    private void BulletHole(Vector3 where, Vector3 surfaceNormal, float normalOffset = 0.01f)
    {
        GameObject newBulletHole = Instantiate(bulletHole, where+surfaceNormal*normalOffset, Quaternion.LookRotation(-surfaceNormal));
        MeshRenderer renderer = newBulletHole.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", weapon.weaponBullet.bulletColor);

        newBulletHole.transform.localScale = Vector3.one*(weapon.weaponBullet.bulletWidth/2);
    }
    
    private void CreateParticle(GameObject prefab, Vector3 startPos, Quaternion startRot, Transform parent = null)
    {
        GameObject particle = Instantiate(prefab, startPos, startRot, parent);
        particle.GetComponent<ParticleSystem>().Play();
    }

    private void DoImpact()
    {
        CreateParticle(weapon.weaponBullet.onHitPrefab, transform.position, transform.rotation);
        StartCoroutine(DisablePhysicsCoroutine());
    }

    private IEnumerator DisablePhysicsCoroutine(float delay = 3)
    {
        // For now, its the trail who actually destroys the object.
        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        // But we don't want to wait too much for the trail to destroy the bullet...
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
