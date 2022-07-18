using UnityEngine;
using UnityEngine.Serialization;
using Gann4Games.Thirdym.Enums;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.ShootSystem;

namespace Gann4Games.Thirdym.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New weapon preset", menuName = "Scriptable Objects/Weapon Preset")]
    public class SO_WeaponPreset : ScriptableObject
    {
        [Header("Main Parameters")]
        public ShootBehaviour behaviour;
        public Sprite weaponIcon;
        public string weaponName = "unnamed_weapon";
        [FormerlySerializedAs("damage")] public int weaponDamage;
        public WeaponType weaponType;
        [FormerlySerializedAs("bulletType")] public SO_BulletPreset weaponBullet;
        [FormerlySerializedAs("sfxShoot")] public AudioClip[] fireSoundEffects;
        [FormerlySerializedAs("sfxReload")] public AudioClip[] reloadSoundEffects;

        [Header("Ragdoll configuration")]
        [Tooltip("The object that the player will drop, for example, on death if this weapon is found in the inventory.")]
        [FormerlySerializedAs("dropPrefab")] public GameObject objectToDrop;
        [FormerlySerializedAs("animationOverride")] public AnimatorOverrideController characterAnimationOverride;
        [Tooltip("Usually long weapons are held with both hands.")]
        [FormerlySerializedAs("supportedByLeftHand")] public bool leftHandSupportsWeapon = true;


        [Header("Ragdoll arms configuration")]
        [Tooltip("The model that will be placed in the right hand.")]
        public GameObject rightWeaponModel;
        public Vector3 rightPositionOffset;
        public Vector3 rightRotationOffset;
        [FormerlySerializedAs("rightShoulderSpring")] public bool useRightShoulder = true;
        [FormerlySerializedAs("rightElbowSpring")] public bool useRightElbow = true;
        [Space]
        [Tooltip("The model that will be placed in the left hand.")]
        public GameObject leftWeaponModel;
        public Vector3 leftPositionOffset;
        public Vector3 leftRotationOffset;
        [FormerlySerializedAs("leftShoulderSpring")] public bool useLeftShoulder = true;
        [FormerlySerializedAs("leftElbowSpring")] public bool useLeftElbow = true;


        [Header("Weapon configuration")]
        [FormerlySerializedAs("shootPoint")] public Vector3 bulletFireSource;
        [Tooltip("Bullet spawning in seconds.")]
        [FormerlySerializedAs("repeatTime")] public float bulletFireTime = 1;
        [FormerlySerializedAs("bulletCount")] public int bulletSpawnCount = 1;
        [FormerlySerializedAs("bulletSpread")] public float bulletSpreadAngle;
        [Tooltip("When the bullet will be destroyed (in seconds) after it has been spawned")]
        [FormerlySerializedAs("bulletStopTime")] public float bulletDespawnTime = 3;
        [Space]
        [Tooltip("Allows the weapon to be aimed accurately towards the center of screen.")]
        public bool useCameraAim = true;
        [Space]
        [Tooltip("This setting allows the weapon to push back the player when shooting.")]
        [FormerlySerializedAs("useRecoil")] public bool useFireRecoil = true;
        [Space]
        public bool useRicochet = true;
        public float ricochetMinAngle = .75f;
        [Space]
        public bool useReload;
        public float reloadStartDelay;
        public float reloadDuration;
        [Space]
        public GameObject muzzleFlash;
        public float muzzleFlashDisableTime = 0.05f;

        public AudioClip GetFireSFX() => AudioTools.GetRandomClip(fireSoundEffects);
        public AudioClip GetReloadSFX() => AudioTools.GetRandomClip(reloadSoundEffects);
    }
}
