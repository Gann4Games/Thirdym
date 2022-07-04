using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArms : MonoBehaviour {
    [HideInInspector]
    Animator _anim;
    //public HingeJoint[] armsMotion;
    public HingeJoint[] LeftShoulder, RightShoulder;
    public HingeJoint LeftBicep, RightBicep, LeftElbow, RightElbow;
    public HingeJoint[] Neck;
    RagdollController _ragdoll;
    EquipmentSystem equipment;
    CharacterHealthSystem healthOp;

    public List<HingeJoint> GetUpperBodyParts()
    {
        List<HingeJoint> joints = new List<HingeJoint>
        {
            LeftShoulder[0],
            LeftShoulder[1],
            RightShoulder[0],
            RightShoulder[1],

            LeftBicep,
            RightBicep,

            LeftElbow,
            RightElbow,

            Neck[0],
            Neck[1]
        };

        return joints;
    }
    public void SetArmsSpring(bool useSpring)
    {
        foreach(HingeJoint bodypart in GetUpperBodyParts())
        {
            bodypart.useSpring = useSpring;
        }
    }
    private void Start()
    {
        _ragdoll = GetComponent<RagdollController>();
        _anim = _ragdoll.Animator;
        equipment = GetComponent<EquipmentSystem>();
        healthOp = GetComponent<CharacterHealthSystem>();
    }
    private void Update()
    {
        if (Neck[0] == null || Neck[1] == null)
        {
            Destroy(GetComponent<RagdollController>());
            Destroy(GetComponent<EquipmentSystem>());
            Destroy(GetComponent<ShootSystem>());
            Destroy(GetComponent<CharacterHealthSystem>());
            Destroy(GetComponent<PlayerCameraController>());
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            healthOp.DealDamage(healthOp.Health, Vector3.zero);
            HingeJoint[] hj = GetComponentsInChildren<HingeJoint>();
            for(int i = 0; i < hj.Length; i++)
                hj[i].useSpring = false;
            Destroy(this, 0.1f);
        }
        if (_anim)
        {
            _anim.SetBool("Disarmed", equipment.IsDisarmed);
            if (!healthOp.IsDead)
            {
                foreach (HingeJoint neckJoint in Neck)
                {
                    if (!neckJoint) return;
                    neckJoint.useSpring = true;
                }

                // High precision aiming
                if (_ragdoll.Customizator.isPlayer)
                {
                    bool isCharacterFiring = _ragdoll.InputHandler.firing;
                    bool isCharacterAiming = _ragdoll.InputHandler.aiming;
                    bool isCharacterDisarmed = equipment.IsDisarmed;
                    bool aimGun = isCharacterAiming && !isCharacterDisarmed;

                    AimWeapon(aimGun);
                    _ragdoll.SetWeaponAnimationActionState(isCharacterFiring);
                    _ragdoll.SetWeaponAnimationAimState(isCharacterAiming);
                    // _anim.SetBool("WeaponAiming", isCharacterAiming);
                    // _anim.SetBool("WeaponAction", isCharacterFiring);
                } 
                else // TODO: NEEDS PROPER DEVELOPMENT
                {
                    AimWeapon(_ragdoll.EquipmentController.IsDisarmed);
                    _ragdoll.SetWeaponAnimationActionState(_ragdoll.EquipmentController.IsDisarmed);
                    _ragdoll.SetWeaponAnimationAimState(_ragdoll.EquipmentController.IsDisarmed);
                }
            }
            else 
            {
                foreach (HingeJoint neckJoint in Neck)
                {
                    if (!neckJoint) return;
                    neckJoint.useSpring = false;
                }
            }
        }
    }

    public void AimWeapon(bool aiming)
    {
        if (IngameMenuHandler.instance.paused || !_ragdoll.EquipmentController.currentWeapon) return;

        bool allowWeaponAim = _ragdoll.EquipmentController.currentWeapon.useCameraAim;
        bool supportedByLeftHand = _ragdoll.EquipmentController.currentWeapon.leftHandSupportsWeapon;
        bool isReloading = _ragdoll.GetReloadAnimationState();

        if(aiming && allowWeaponAim && !isReloading)
        {
            RightHandLookAtScreenCenter();
        }
        else if (isReloading || supportedByLeftHand)
        {
            RightHandLookAtLeftHand();
        }
        else
        {
            RightHandToDefaultPosition();
        }
    }
    public void RightHandLookAt(Vector3 position)
    {
        _ragdoll.Customizator.baseBody.rightHand.LookAt(position);
        _ragdoll.Customizator.baseBody.rightHand.Rotate(-90, -90, 0);
    }
    void RightHandLookAtLeftHand() => RightHandLookAt(_ragdoll.Customizator.baseBody.leftHand.position);
    void RightHandLookAtScreenCenter() => RightHandLookAt(PlayerCameraController.Instance.CameraCenterPoint);
    void RightHandToDefaultPosition() => _ragdoll.Customizator.baseBody.rightHand.localRotation = Quaternion.Lerp(_ragdoll.Customizator.baseBody.rightHand.localRotation, Quaternion.identity, Time.deltaTime * 10);
}
