using System.Collections;
using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.ShootSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Shooting/Firearm Behaviour", fileName = "Firearm Behaviour")]
    public class FirearmBehaviour : ShootBehaviour
    {
        private Animator _weaponAnimator;
        public override void Fire(ShootBehaviourLoader source)
        {
            if(source.TryGetComponent(out Animator anim)) _weaponAnimator = anim;
            if(source.weaponData.useReload) source.StartCoroutine(ReloadWeapon(source));
            ApplyRecoil(source);
            CreateBullets(source);
        }

        private static void ApplyRecoil(ShootBehaviourLoader source)
        {
            if (source.Ragdoll.EquipmentController.currentWeapon.useFireRecoil)
                source.Ragdoll.Customizator.baseBody.rightElbow.GetComponent<Rigidbody>().AddForce(PlayerCameraController.GetCameraDirection() * -(500 * (source.weaponData.weaponDamage / 10)));
        }

        private static void CreateBullets(ShootBehaviourLoader source)
        {
            Vector3 shootPosition = source.transform.position + source.transform.TransformDirection(source.weaponData.bulletFireSource);

            for (int i = 0; i < source.weaponData.bulletSpawnCount; i++)
            {
                GameObject _bulletPrefab = Instantiate(source.weaponData.weaponBullet.bullet, shootPosition, source.transform.rotation, null);
                _bulletPrefab.transform.Rotate(Random.Range(-source.weaponData.bulletSpreadAngle, source.weaponData.bulletSpreadAngle), Random.Range(-source.weaponData.bulletSpreadAngle, source.weaponData.bulletSpreadAngle), 0);

                Bullet _bulletComponent = _bulletPrefab.GetComponent<Bullet>();
                _bulletComponent.weapon = source.weaponData;
            }
        }

        IEnumerator ReloadWeapon(ShootBehaviourLoader source)
        {
            AudioClip reloadSFX = AudioTools.GetRandomClip(source.weaponData.reloadSoundEffects);

            yield return new WaitForSeconds(source.weaponData.reloadStartDelay);
            if(_weaponAnimator) _weaponAnimator.SetBool("WeaponReload", true);
            source.Ragdoll.Animator.SetBool("WeaponReload", true);
            source.Ragdoll.PlaySFX(reloadSFX);

            yield return new WaitForSeconds(source.weaponData.reloadDuration);
            if(_weaponAnimator) _weaponAnimator.SetBool("WeaponReload", false);
            source.Ragdoll.Animator.SetBool("WeaponReload", false);
        }
    }
}
