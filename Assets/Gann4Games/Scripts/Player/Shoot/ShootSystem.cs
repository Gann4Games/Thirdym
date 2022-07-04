using UnityEngine;
public class ShootSystem : MonoBehaviour {
    RagdollController _ragdoll;
    Transform _user;

    CharacterShootHandler _shootScript;

    private void Awake()
    {
        _shootScript = GetComponentInChildren<CharacterShootHandler>();
    }
    private void Start()
    {
        _ragdoll = GetComponent<RagdollController>();
        _user = transform;
    }
    private void Update()
    {
        bool isCharacterDead = _ragdoll.HealthController.IsDead;
        bool isCharacterDisarmed = _ragdoll.EquipmentController.IsDisarmed;
        bool isCharacterAPlayer = _ragdoll.Customizator.isPlayer;
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
        if (!_ragdoll.Customizator.isNPC && IngameMenuHandler.instance.paused) return;
        _shootScript.StartShooting();
    }
}
