﻿using UnityEngine;
using Gann4Games.Thirdym.ScriptableObjects;
using Gann4Games.Thirdym.Interfaces;

public class Bullet : MonoBehaviour {
    [HideInInspector] public Transform user; //Automatically set by actionShoot.cs

    public SO_WeaponPreset weapon;

    [SerializeField] GameObject energyImpact;
    [SerializeField] GameObject bulletHole;
    [SerializeField] float bulletSpeed = 10;

    Rigidbody _rigidbody;
    TrailRenderer _trailRenderer;
    Vector3 _fireDirection;
    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

        ApplyVisuals();

        FireBullet(transform.right);
        Destroy(gameObject, weapon.bulletDespawnTime);
    }
    void ApplyVisuals()
    {
        _trailRenderer.startColor = weapon.weaponBullet.bulletColor;
        _trailRenderer.endColor = weapon.weaponBullet.bulletColor;
        _trailRenderer.startWidth = weapon.weaponBullet.bulletWidth;
        _trailRenderer.endWidth = weapon.weaponBullet.bulletWidth;
        _trailRenderer.time = weapon.weaponBullet.bulletLenght;
        _trailRenderer.material = weapon.weaponBullet.bulletMaterial;
        _trailRenderer.textureMode = weapon.weaponBullet.textureMode;
    }

    private void FireBullet(Vector3 direction)
    {
        _fireDirection = direction;
        direction.Normalize();
        transform.LookAt(transform.position + direction);
        _rigidbody.velocity = direction * bulletSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (weapon.useRicochet) Ricochet();
        else DoImpact();

        var damageableObject = other.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.DealDamage(weapon.weaponDamage, DamageType.Bullet, user.position);
        }

        HingeJointTarget ragdollJoint = other.gameObject.GetComponent<HingeJointTarget>();
        if (ragdollJoint)
        {
            if (ragdollJoint.CanBeDismembered) 
                ragdollJoint.hj.breakForce -= 100 * weapon.weaponDamage;
        }

        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
        if (otherRigidbody)
        {
            if (other.transform.gameObject.HasTag("Breakable"))
                other.transform.GetComponent<Rigidbody>().isKinematic = false;
            other.transform.GetComponent<Rigidbody>().AddForce(transform.forward * (50 * weapon.weaponDamage));
        }
        else
            CreateParticle(weapon.weaponBullet.onHitPrefab, transform.position, transform.rotation);

        if (other.transform.gameObject.HasTag("Energy"))
        {
            CreateParticle(energyImpact, transform.position, transform.rotation, other.transform);
        }
    }
    void Ricochet()
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
                FireBullet(bounceDirection);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    void BulletHole(Vector3 where, Vector3 surfaceNormal, float normalOffset = 0.01f)
    {
        GameObject newBulletHole = Instantiate(bulletHole, where+surfaceNormal*normalOffset, Quaternion.LookRotation(-surfaceNormal));
        MeshRenderer renderer = newBulletHole.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", weapon.weaponBullet.bulletColor);

        newBulletHole.transform.localScale = Vector3.one*(weapon.weaponBullet.bulletWidth/2);
    }
    public void CreateParticle(GameObject prefab, Vector3 startPos, Quaternion startRot, Transform parent = null)
    {
        GameObject particle = Instantiate(prefab, startPos, startRot, parent);
        particle.GetComponent<ParticleSystem>().Play();
    }
    public void DoImpact()
    {
        CreateParticle(weapon.weaponBullet.onHitPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    public void Deflect()
    {
        FireBullet(-_rigidbody.velocity);
    }
}
