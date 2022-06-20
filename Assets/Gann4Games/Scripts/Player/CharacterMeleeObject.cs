using UnityEngine;
using Gann4Games.Thirdym.Core;

public class CharacterMeleeObject : MonoBehaviour {

    CharacterCustomization _character;
    [SerializeField] GameObject impactPrefab;
    Collider _collider;
    float _bladeDamage;

    MeshRenderer LeftRenderer
    {
        get => _character.EquipmentController.LeftHandWeapon.GetComponent<MeshRenderer>();
    }
    MeshRenderer RightRenderer
    {
        get => _character.EquipmentController.RightHandWeapon.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _character = GetComponentInParent<CharacterCustomization>();
        _collider = GetComponentInChildren<Collider>();
        _bladeDamage = _character.preset.bladeDamage;
        
        LeftRenderer.material.SetColor("_MainColor", _character.preset.bladesColor);
        RightRenderer.material.SetColor("_MainColor", _character.preset.bladesColor);
    }
    private void OnTriggerEnter(Collider other)
    {
        var damageableObject = other.GetComponent<IDamageable>();
        CharacterBodypart otherBodypart = other.GetComponent<CharacterBodypart>();
        BreakableObject otherBreakable = other.GetComponent<BreakableObject>();
        Bullet otherBullet = other.GetComponent<Bullet>();
        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();

        bool canBeDamaged = otherBodypart?.character != _character;

        if (canBeDamaged || !otherBodypart)
            damageableObject?.DealDamage(_bladeDamage, DamageType.Blade, _character.transform.position);

        if (otherBreakable)
            _character.preset.IndicateDamage(transform.position).Display(_bladeDamage.ToString("F0"), Color.white);
        else
            spawnParticle(impactPrefab, transform.position);

        if(otherBullet)
        {
            Rigidbody otherBulletRigidbody = otherBullet.GetComponent<Rigidbody>();
            otherBulletRigidbody.velocity = -otherBulletRigidbody.velocity;
            _character.preset.IndicateDamage(transform.position).Display("Deflect", Color.white);
        }

        if (otherRigidbody && !otherBullet)
            otherRigidbody.AddForce(_character.transform.forward * 10000 * Time.deltaTime);

        EnableCollider(false);
    }
    public void EnableCollider(bool enable) => _collider.enabled = enable;
    void spawnParticle(GameObject particle, Vector3 pos)
    {
        GameObject prefab = Instantiate(particle);
        prefab.transform.position = pos;
    }
}
