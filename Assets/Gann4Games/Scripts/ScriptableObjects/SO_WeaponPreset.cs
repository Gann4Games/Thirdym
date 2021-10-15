using UnityEngine;
using Gann4Games.Thirdym.Enums;

namespace Gann4Games.Thirdym.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New weapon preset", menuName = "Scriptable Objects/Weapon Preset")]
    public class SO_WeaponPreset : ScriptableObject
    {
        // FIND ALL OF THESE PARAMETERS IN 'SO_Bullet Preset' AND REMOVE THEM

        [Header("Visuals")]
        [Tooltip("The object that the player will drop.")]
        public GameObject dropPrefab;
        [Space]
        [Tooltip("The model that will be placed in the left hand.")]
        public GameObject leftWeaponModel;
        public Vector3 leftPositionOffset;
        public Vector3 leftRotationOffset;
        [Space]
        [Tooltip("The model that will be placed in the right hand.")]
        public GameObject rightWeaponModel;
        public Vector3 rightPositionOffset;
        public Vector3 rightRotationOffset = new Vector3(-90, 0, 0);
        [Space]
        [Tooltip("Spring to be assigned in the left hand, for extra weapon support, and also visuals.")]
        public float ikSpring = 12000;
        [Tooltip("Position in which the hand will be placed relative to the gun")]
        public Vector3 ikAnchor;
        public bool ikIsTrigger = false;
        [Space]
        [Tooltip("0 for all, 1 for pistols, 2 for swords.")]
        public int aimType;
        [Space]
        public bool leftShoulderSpring;
        public bool leftElbowSpring;
        public bool rightShoulderSpring = true;
        public bool rightElbowSpring = true;


        [Header("Stats")]
        public WeaponType weaponType;
        public float damage;
        [Header("Configuration")]
        public string weaponName = "unnamed_weapon";
        public bool ricochet;
        public float ricochetMinAngle = .75f;
        public float repeatTime;
        [Space]
        public float bulletSpread;
        public float bulletCount;

        [Header("Effects")]
        public SO_BulletPreset bulletType;
        [Space]
        public GameObject muzzleFlash;
        public float muzzleFlashDisableTime;

        [Header("Sound Effects")]
        public AudioClip sfxShoot;
        public AudioClip sfxReload;
    }
}
