using System.Collections;
using UnityEngine;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.ScriptableObjects;

public class CharacterShootHandler : MonoBehaviour {

    
    Animator _weaponAnimator;
    Rigidbody _handRigidbody;
    TimerTool _timer;

    CharacterCustomization _character;

    SO_WeaponPreset _weapon => _character.EquipmentController.currentWeapon;

    float _repeatTime => _weapon.bulletFireTime;
    public Vector3 HitPosition
    {
        get
        {
            if (Physics.Raycast(transform.position, transform.right, out RaycastHit hit))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
                return hit.point;
            }
            else
            {
                return transform.position;
            }
        }
    }
    private void Awake()
    {
        _character = GetComponentInParent<CharacterCustomization>();
        _timer = new TimerTool(0);
    }
    private void Start()
    {
        _weaponAnimator = _character.Animator;
        _handRigidbody = _character.baseBody.rightHand.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (_weapon == null) return;

        if (_timer.MaxTime != _repeatTime)
            _timer.SetMaxTime(_repeatTime);

        if(!_timer.HasFinished()) 
        {
            _timer.Count();
            if(!_character.isNPC) MainHUDHandler.instance.crosshairImage.fillAmount = _timer.CurrentTime / _weapon.bulletFireTime;
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
        if(_character.EquipmentController.currentWeapon.bulletSpawnCount != 0)
            _character.PlayFireSFX();
        if(_character.EquipmentController.currentWeapon.useFireRecoil)
            _character.baseBody.rightElbow.GetComponent<Rigidbody>().AddForce(PlayerCameraController.instance.activeCamera.transform.forward * -(500 * (_weapon.weaponDamage / 10)));

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
            _bulletComponent.user = _character.transform;
            _bulletComponent.weapon = _weapon;
        }
    }
    IEnumerator ReloadWeapon()
    {
        AudioClip reloadSFX = AudioTools.GetRandomClip(_weapon.reloadSoundEffects);

        yield return new WaitForSeconds(_weapon.reloadStartDelay);
        _character.Animator.SetBool("WeaponReload", true);
        _character.SoundSource.PlayOneShot(reloadSFX);
        _weaponAnimator?.SetBool("WeaponReload", true);

        yield return new WaitForSeconds(_weapon.reloadDuration);
        _character.Animator.SetBool("WeaponReload", false);
        _weaponAnimator?.SetBool("WeaponReload", false);
    }
}
