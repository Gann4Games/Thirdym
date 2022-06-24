using UnityEngine;
public class ShootSystem : MonoBehaviour {
    CharacterCustomization _character;
    Transform _user;

    CharacterShootHandler _shootScript;

    private void Awake()
    {
        _shootScript = GetComponentInChildren<CharacterShootHandler>();
    }
    private void Start()
    {
        _character = GetComponent<CharacterCustomization>();
        _user = transform;
    }
    private void Update()
    {
        bool isCharacterDead = _character.HealthController.IsDead;
        bool isCharacterDisarmed = _character.EquipmentController.disarmed;
        bool isCharacterAPlayer = _character.isPlayer;
        bool isGamePaused = IngameMenuHandler.instance.paused;
        bool isCharacterFiring = PlayerInputHandler.instance.firing;
        bool isCharacterAiming = PlayerInputHandler.instance.aiming;
        bool canShoot = !isCharacterDead && !isCharacterDisarmed && !isGamePaused && isCharacterAPlayer;

        if (canShoot && isCharacterFiring && isCharacterAiming)
        {
            _shootScript.StartShooting();
        }
    }

    /// <summary>
    /// The function is executed only if the character is a NPC
    /// </summary>
    public void ShootAsNPC()
    {
        if (!_character.isNPC && IngameMenuHandler.instance.paused) return;
        _shootScript.StartShooting();
    }
}
