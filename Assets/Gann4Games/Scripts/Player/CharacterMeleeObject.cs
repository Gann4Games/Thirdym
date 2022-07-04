using UnityEngine;
using Gann4Games.Thirdym.Core;

public class CharacterMeleeObject : MonoBehaviour {

    [SerializeField] private GameObject impactPrefab;
    private RagdollController _ragdoll;
    private Collider _collider;
    private float _bladeDamage;

    MeshRenderer LeftRenderer { get => _ragdoll.EquipmentController.LeftHandWeapon.GetComponent<MeshRenderer>(); }
    MeshRenderer RightRenderer { get => _ragdoll.EquipmentController.RightHandWeapon.GetComponent<MeshRenderer>();}

    private void Start()
    {
        _ragdoll = GetComponentInParent<RagdollController>();
        _collider = GetComponentInChildren<Collider>();
        _bladeDamage = _ragdoll.Customizator.preset.bladeDamage;
        ApplyColors();
    }

    private void ApplyColors()
    {
        LeftRenderer.material.SetColor("_MainColor", _ragdoll.Customizator.preset.bladesColor);
        RightRenderer.material.SetColor("_MainColor", _ragdoll.Customizator.preset.bladesColor);
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
            bool isDamageableMyself = (damageable as CharacterBodypart).Ragdoll == _ragdoll;
            if(!isDamageableMyself) damageable.DealDamage(_bladeDamage, DamageType.Blade, _ragdoll.transform.position);
        }

        // Add force to rigidbodies
        if(other.TryGetComponent(out Rigidbody rigidbody)) rigidbody.AddForce(_ragdoll.transform.forward * 10000 * Time.deltaTime);
    }
    
    public void EnableCollider(bool enable) => _collider.enabled = enable;
    private void SpawnParticle(GameObject particle, Vector3 pos)
    {
        GameObject prefab = Instantiate(particle);
        prefab.transform.position = pos;
    }
}
