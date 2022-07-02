using UnityEngine;
using Gann4Games.Thirdym.Core;

public class CharacterMeleeObject : MonoBehaviour {

    [SerializeField] private GameObject impactPrefab;
    private CharacterCustomization _character;
    private Collider _collider;
    private float _bladeDamage;

    MeshRenderer LeftRenderer { get => _character.EquipmentController.LeftHandWeapon.GetComponent<MeshRenderer>(); }
    MeshRenderer RightRenderer { get => _character.EquipmentController.RightHandWeapon.GetComponent<MeshRenderer>();}

    private void Start()
    {
        _character = GetComponentInParent<CharacterCustomization>();
        _collider = GetComponentInChildren<Collider>();
        _bladeDamage = _character.preset.bladeDamage;
        ApplyColors();
    }

    private void ApplyColors()
    {
        LeftRenderer.material.SetColor("_MainColor", _character.preset.bladesColor);
        RightRenderer.material.SetColor("_MainColor", _character.preset.bladesColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Disabling the collider allows to deal damage just once.
        EnableCollider(false);
        // TODO: Delegate particle spawning to breakable or damageable objects
        SpawnParticle(impactPrefab, transform.position);

        if(other.TryGetComponent(out Bullet bullet))
        {
            bullet.InvertDirection();
            // If a bullet is found, we don't need to check for anything else.
            return;
        }

        if(other.TryGetComponent(out IDamageable damageable))
        {
            bool isDamageableMyself = (damageable as CharacterBodypart).character == _character;
            if(!isDamageableMyself) damageable.DealDamage(_bladeDamage, DamageType.Blade, _character.transform.position);
        }

        // Add force to rigidbodies
        if(other.TryGetComponent(out Rigidbody rigidbody)) rigidbody.AddForce(_character.transform.forward * 10000 * Time.deltaTime);

        //bool isNotMe = otherBodypart?.character != _character;

        //var damageableObject = other.GetComponent<IDamageable>();
        //CharacterBodypart otherBodypart = other.GetComponent<CharacterBodypart>();
        //BreakableObject otherBreakable = other.GetComponent<BreakableObject>();
        // Bullet otherBullet = other.GetComponent<Bullet>();
        // Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();


        // if (isNotMe || !otherBodypart)
        //     damageableObject?.DealDamage(_bladeDamage, DamageType.Blade, _character.transform.position);

        // if(otherBullet)
        // {
        //     Rigidbody otherBulletRigidbody = otherBullet.GetComponent<Rigidbody>();
        //     otherBulletRigidbody.velocity = -otherBulletRigidbody.velocity;
        // }

        // if (otherRigidbody && !otherBullet)
        //     otherRigidbody.AddForce(_character.transform.forward * 10000 * Time.deltaTime);

  
    }
    public void EnableCollider(bool enable) => _collider.enabled = enable;
    private void SpawnParticle(GameObject particle, Vector3 pos)
    {
        GameObject prefab = Instantiate(particle);
        prefab.transform.position = pos;
    }
}
