﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArms : MonoBehaviour {
    [HideInInspector]
    Animator _anim;
    //public HingeJoint[] armsMotion;
    public HingeJoint[] LeftShoulder, RightShoulder;
    public HingeJoint LeftBicep, RightBicep, LeftElbow, RightElbow;
    public HingeJoint[] Neck;
    CharacterCustomization _character;
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
        _character = GetComponent<CharacterCustomization>();
        _anim = _character.Animator;
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
            healthOp.DealDamage(healthOp.CurrentHealth, Vector3.zero);
            HingeJoint[] hj = GetComponentsInChildren<HingeJoint>();
            for(int i = 0; i < hj.Length; i++)
                hj[i].useSpring = false;
            Destroy(this, 0.1f);
        }
        if (_anim)
        {
            _anim.SetBool("Disarmed", equipment.disarmed);
            if (!healthOp.IsDead)
            {
                foreach (HingeJoint neckJoint in Neck)
                {
                    if (!neckJoint) return;
                    neckJoint.useSpring = true;
                }
                #region Arms controll

                if (!_character.isNPC)
                {
                    bool isCharacterFiring = InputHandler.instance.firing;
                    bool isCharacterAiming = InputHandler.instance.aiming;
                    bool isCharacterDisarmed = equipment.disarmed;
                    bool aimGun = isCharacterAiming && !isCharacterDisarmed;

                    AimGun(aimGun);
                    _anim.SetBool("WeaponAiming", isCharacterAiming);
                    _anim.SetBool("WeaponAction", isCharacterFiring);
                }
                #endregion
            }
            else 
            {
                if(_anim.GetFloat("Arm") != 0) _anim.SetFloat("Arm", 0); // Stop swords movements
                foreach (HingeJoint neckJoint in Neck)
                {
                    if (!neckJoint) return;
                    neckJoint.useSpring = false;
                }
            }
        }
    }

    public void AimGun(bool aim)
    {
        if (IngameMenuHandler.instance.paused) return;

        bool isReloading = _anim.GetBool("WeaponReload");

        if (!isReloading)
        {
            if (aim) RightHandLookAt(PlayerCameraController.instance.CameraCenterPoint);
            else RightHandToDefaultPosition();
        }
        else
        {
            RightHandLookAt(_character.baseBody.leftHand.position);
        }
    }
    void RightHandLookAt(Vector3 position)
    {
        _character.baseBody.rightHand.LookAt(position);
        _character.baseBody.rightHand.Rotate(-90, -90, 0);
    }
    void RightHandToDefaultPosition() => _character.baseBody.rightHand.localRotation = Quaternion.Lerp(_character.baseBody.rightHand.localRotation, Quaternion.identity, Time.deltaTime * 10);
}
