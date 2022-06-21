using UnityEngine;
using UnityEditor;
using Gann4Games.Thirdym.Localization; // Maybe another code smell ... ?
using Gann4Games.Thirdym.ScriptableObjects;
using Gann4Games.Thirdym.Events;
using Gann4Games.Thirdym.Interactables;

[RequireComponent(typeof(CollisionEvents))]
[RequireComponent(typeof(Rigidbody))]
public class PickupableWeapon : InteractableObject
{
    public SO_WeaponPreset weaponData;

    public AudioClip onPickupSFX;
    public AudioClip collisionSoftSFX;
    public AudioClip collisionMediumSFX;

    CharacterCustomization _character;
    AudioSource _auSource;

    CollisionEvents _collisionEvents;

    private void Awake()
    {
        _collisionEvents = GetComponent<CollisionEvents>();
    }
    private void Start()
    {
        _auSource = gameObject.AddComponent<AudioSource>();
        _auSource.spatialBlend = 1;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 shootPoint = transform.position + transform.TransformDirection(weaponData.bulletFireSource);

        Gizmos.color = Color.red * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(shootPoint, 0.025f);
        Handles.Label(shootPoint, "Shoot source");

        Gizmos.color = Color.green * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.025f);
        Handles.Label(transform.position, "Right Hand Position (origin)");
    }

    void OnCollideSoft(object sender, CollisionEvents.CollisionArgs args)
    {
        _auSource.PlayOneShot(collisionSoftSFX);
    }

    void OnCollideMedium(object sender, CollisionEvents.CollisionArgs args)
    {
        _auSource.PlayOneShot(collisionMediumSFX);
    }

    public void EquipWeapon(SO_WeaponPreset weapon)
    {
        if (_character.EquipmentController.HasWeapon(weapon.weaponType))
        {
            AlreadyEquippedAlert(weapon);
            return;
        }

        _character.EquipmentController.EquipWeapon(weapon);
        OnPickup();
    }
    private void OnEnable()
    {
        _collisionEvents.OnCollideSoft += OnCollideSoft;
        _collisionEvents.OnCollideMedium += OnCollideMedium;
    }
    private void OnDisable()
    {
        _collisionEvents.OnCollideSoft -= OnCollideSoft;
        _collisionEvents.OnCollideMedium -= OnCollideMedium;
    }
    void OnPickup()
    {
        if (_character.isPlayer)
        {
            switch (LanguagePrefs.Language)
            {
                case AvailableLanguages.English:
                    NotificationHandler.Notify("Picked up " + weaponData.weaponName, 2, 2, false);
                    break;
                case AvailableLanguages.Español:
                    NotificationHandler.Notify("Agarraste " + weaponData.weaponName, 2, 2, false);
                    break;
            }
        }

        _character.PlaySFX(onPickupSFX);
        Destroy(gameObject);
    }

    void AlreadyEquippedAlert(SO_WeaponPreset weapon)
    {
        if (_character.isPlayer)
        {
            switch (LanguagePrefs.Language)
            {
                case AvailableLanguages.English:
                    NotificationHandler.Notify("You already have a weapon of type " + weaponData.weaponType.ToString() + ".", 2, 2, false);
                    break;
                case AvailableLanguages.Español:
                    NotificationHandler.Notify("Ya tienes un arma de tipo " + weaponData.weaponType.ToString() + ".", 2, 2, false);
                    break;
            }
        }
    }

    public override void Interact(CharacterCustomization character)
    {
        _character = character;
        if (_character) EquipWeapon(weaponData);
    }
    public override string Hint => $"Press {UseKey.ToUpper()} to equip {weaponData.name}";
}
