using UnityEngine;
using System;
using Gann4Games.Thirdym.Core;
using Gann4Games.Thirdym.Events;

[RequireComponent(typeof(CollisionEvents))]
public class CharacterBodypart : MonoBehaviour, IDamageable
{

    public float damageMultiplier;
    public CharacterCustomization character;

    [Header("Optional sound effect")]
    [Tooltip("Replaces the private parameters from the character preset.")]
    [SerializeField] AudioClip sfxCollideHard, sfxCollideMedium, sfxCollideSoft;

    AudioClip _sfxCollideHard, _sfxCollideMedium, _sfxCollideSoft;
    CharacterHealthSystem _healthController;
    CollisionEvents _colliderEvents;

    public void DealDamage(float damage, DamageType damageType, Vector3 where)
    {

        character.preset.IndicateDamage(transform.position).Display(damage.ToString("F0"), Color.red);
        _healthController.DealDamage(damage, where, !character.isNPC);

        switch (damageType)
        {
            case DamageType.Blade:
                {
                    GameObject bloodfx = character.preset.BloodSplatFX();
                    bloodfx.transform.position = transform.position;
                    break;
                }

            case DamageType.Bullet:
                {
                    GameObject bloodfx = character.preset.BloodSplatFX();
                    bloodfx.transform.position = transform.position;
                    bloodfx.transform.rotation = transform.rotation;
                    break;
                }

            case DamageType.Collision:
                if(!character.isNPC)
                    MainHUDHandler.instance.ShowEffect(Color.white, damage / _healthController.MaximumHealth, 10);
                break;
        }
    }
    private void Awake()
    {
        _colliderEvents = GetComponent<CollisionEvents>();
        _colliderEvents.OnCollideHard += CollideHard;
        _colliderEvents.OnCollideMedium += CollideMedium;
        _colliderEvents.OnCollideSoft += CollideSoft;
    }
    private void Start()
    {
        character = GetComponentInParent<CharacterCustomization>();
        _healthController = character.HealthController;

        // If optional SFX parameters are set, don't use the ones stored in the character preset.
        #region Applying sound effects
        if (sfxCollideSoft) _sfxCollideSoft = sfxCollideSoft;
        else _sfxCollideSoft = character.preset.sfxCollideSoft;

        if (sfxCollideMedium) _sfxCollideMedium = sfxCollideMedium;
        else _sfxCollideMedium = character.preset.sfxCollideMedium;

        if (sfxCollideHard) _sfxCollideHard = sfxCollideHard;
        else _sfxCollideHard = character.preset.sfxCollideHard;
        #endregion
    }
    private void OnDestroy()
    {
        _colliderEvents.OnCollideHard -= CollideHard;
        _colliderEvents.OnCollideMedium -= CollideMedium;
        _colliderEvents.OnCollideSoft -= CollideSoft;
    }
    void CollideHard(object sender, CollisionEvents.CollisionArgs args)
    {
        float damage = args.collisionMagnitude * damageMultiplier;

        _healthController.HeavyHit = true;
        character.SoundSource.PlayOneShot(_sfxCollideHard);

        DealDamage(damage, DamageType.Collision, Vector3.zero);
    }
    void CollideMedium(object sender, EventArgs args) => character.SoundSource.PlayOneShot(_sfxCollideMedium);
    void CollideSoft(object sender, EventArgs args) => character.SoundSource.PlayOneShot(_sfxCollideSoft);
}
