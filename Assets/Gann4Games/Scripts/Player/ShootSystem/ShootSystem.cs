using System;
using Gann4Games.Thirdym.ShootSystem;
using UnityEngine;

/// <summary>
/// Loads and executes a ShooterBehaviour class
/// </summary>
public class ShootSystem : MonoBehaviour {
    public ShootBehaviourLoader shooter;
    private RagdollController _ragdoll;
    private void Awake()
    {
        _ragdoll = GetComponent<RagdollController>();
        _ragdoll.OnReady += Initialize;
    }

    private void Initialize(object sender, EventArgs e)
    {
        shooter = GetComponentInChildren<ShootBehaviourLoader>();
        shooter.OnFire += OnFire;
        _ragdoll.OnReady -= Initialize;
    }

    private void OnFire(object sender, EventArgs e) => _ragdoll.PlayFireSFX();

    private void OnDisable()
    {
        shooter.OnFire -= OnFire;
    }

    private void Update()
    {
        bool isCharacterFiring = PlayerInputHandler.instance.firing;
        bool isCharacterAiming = PlayerInputHandler.instance.aiming;
        shooter.behaviour = _ragdoll.EquipmentController.currentWeapon.behaviour;
        shooter.weaponData = _ragdoll.EquipmentController.currentWeapon;
        shooter.SetFireStatus(isCharacterFiring && isCharacterAiming);
    }

    /*
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
    */
}
