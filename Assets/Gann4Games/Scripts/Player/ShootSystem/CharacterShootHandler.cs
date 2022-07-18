using System.Collections;
using UnityEngine;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.ScriptableObjects;

// TODO: Weapon base class for making different shoot behaviours that can be swapped 

public class CharacterShootHandler : MonoBehaviour {

    
    private Animator _weaponAnimator;
    private Rigidbody _handRigidbody;
    private TimerTool _timer;

    private RagdollController _ragdoll;

    private SO_WeaponPreset _weapon => _ragdoll.EquipmentController.currentWeapon;

    float _repeatTime => _weapon.bulletFireTime;
    
    private void Awake()
    {
        _ragdoll = GetComponentInParent<RagdollController>();
        _timer = new TimerTool(0);
    }
    private void Start()
    {
        _weaponAnimator = _ragdoll.Animator;
        _handRigidbody = _ragdoll.Customizator.baseBody.rightHand.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (_weapon == null) return;

        if (_timer.MaxTime != _repeatTime)
            _timer.SetMaxTime(_repeatTime);

        if(!_timer.HasFinished()) 
        {
            _timer.Count();
            if(!_ragdoll.Customizator.isNPC) MainHUDHandler.instance.crosshairImage.fillAmount = _timer.CurrentTime / _weapon.bulletFireTime;
        }
    }
    public void StartShooting()
    {
        if (_timer.HasFinished() && _weapon != null)
        {
            _timer.Reset();
            _weaponAnimator?.SetBool("Shoot", false);
            Shoot();
        }
    }
    void Shoot()
    {
        if(_ragdoll.EquipmentController.currentWeapon.bulletSpawnCount != 0)
            _ragdoll.PlayFireSFX();
        if(_ragdoll.EquipmentController.currentWeapon.useFireRecoil)
            _ragdoll.Customizator.baseBody.rightElbow.GetComponent<Rigidbody>().AddForce(PlayerCameraController.GetCameraDirection() * -(500 * (_weapon.weaponDamage / 10)));

        CreateBullets();
            
        if (_weapon.useReload) StartCoroutine(ReloadWeapon());
        _weaponAnimator?.SetBool("Shoot", true);
    }
    void CreateBullets()
    {
        if (!_weapon.weaponBullet) return;

        Vector3 shootPosition = transform.position + transform.TransformDirection(_weapon.bulletFireSource);

        for (int i = 0; i < _weapon.bulletSpawnCount; i++)
        {
            GameObject _bulletPrefab = Instantiate(_weapon.weaponBullet.bullet, shootPosition, transform.rotation, null);
            _bulletPrefab.transform.Rotate(Random.Range(-_weapon.bulletSpreadAngle, _weapon.bulletSpreadAngle), Random.Range(-_weapon.bulletSpreadAngle, _weapon.bulletSpreadAngle), 0);

            Bullet _bulletComponent = _bulletPrefab.GetComponent<Bullet>();
            _bulletComponent.weapon = _weapon;
        }
    }
    IEnumerator ReloadWeapon()
    {
        AudioClip reloadSFX = AudioTools.GetRandomClip(_weapon.reloadSoundEffects);

        yield return new WaitForSeconds(_weapon.reloadStartDelay);
        _ragdoll.Animator.SetBool("WeaponReload", true);
        _ragdoll.AudioPlayer.PlayOneShot(reloadSFX);
        _weaponAnimator?.SetBool("WeaponReload", true);

        yield return new WaitForSeconds(_weapon.reloadDuration);
        _ragdoll.Animator.SetBool("WeaponReload", false);
        _weaponAnimator?.SetBool("WeaponReload", false);
    }
}
