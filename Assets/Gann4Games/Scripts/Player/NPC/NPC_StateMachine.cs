using System;
using UnityEngine;
using Gann4Games.Thirdym.Utility;

namespace Gann4Games.Thirdym.NPC
{
    public class NPC_StateMachine : MonoBehaviour
    {
        public enum NPCState
        {
            Idle,
            Alert,
            Attacking,
            LookingForWeapons,
            Runaway,
            Dead,
        }
        public NPCState NPCStatus;
        public NPC_Ragdoll NPC;

        TimerTool _timer;
        CharacterCustomization _closestEnemy => NPC.GetClosestAliveRagdoll(NPC.character.preset.enemyTags);
        CharacterCustomization _closestAlly => NPC.GetClosestAliveRagdoll(NPC.character.preset.allyTags);
        PickupableWeapon _closestWeapon => NPC.GetClosestWeapon();

        void Awake()
        {
            _timer = new TimerTool(0);
        }
        public void RunStateMachine()
        {
            /*
             * An NPC can't go into attack mode directly.
             * An NPC will analyze a player before attacking, this action takes two seconds.
             * Once the enemy is identified, the attack state will begin.
             *      Once the enemy is beaten, the idle state will begin.
             *      
             * [Idle] _____           
             *   ^         v
             *   |_____ [Alert] ______ 
             *   |         v          |
             *   |_____ [Attacking]   |
             *   |                    v
             *   |_____ [LookingForWeapons]
             *   |         v
             *   |_____ [Runaway]
             *   
             *   
             *   Any >> Dead >> Idle
             */
            if (NPC.character.HealthController.IsDead && NPCStatus != NPCState.Dead) 
                // [Any] >> Dead
                NPCStatus = NPCState.Dead;

            switch (NPCStatus)
            {
                case NPCState.Idle:
                    if (_timer.MaxTime != 3) _timer.SetMaxTime(3);

                    if (_timer.HasFinished())
                    {
                        _timer.Reset();
                        NPC.HeadLookAt(NPC.GetRandomPlaceAround(NPC.transform.position, Vector2.one * 10));
                    } 
                    else _timer.Count();

                    NPC.SelfBalance();
                    NPC.character.Animator.SetFloat("X", 0);
                    NPC.character.Animator.SetFloat("Y", 0);

                    if (NPC.IsOnSight(_closestEnemy.transform.position))
                    {
                        // [Idle] >> Alert
                        NPCStatus = NPCState.Alert;
                        break;
                    }
                    // [Idle] >> Idle (Loop)
                    break;

                case NPCState.Alert:
                    // Idle >> [Alert]. | ALWAYS from Idle.
                    if(_timer.MaxTime != 2) _timer.SetMaxTime(2);


                    if (_timer.HasFinished())
                    {
                        _timer.Reset();
                        if (NPC.character.EquipmentController.currentWeapon == null)
                        {
                            // [Alert] >> LookingForWeapons
                            NPCStatus = NPCState.LookingForWeapons;
                            return;
                        }
                        else
                        {
                            // [Alert] >> Attacking
                            NPCStatus = NPCState.Attacking;
                            return;
                        }
                    }
                    else
                    {
                        _timer.Count();
                        if (NPC.IsOnSight(_closestEnemy.transform.position))
                        {
                            NPC.HeadLookAt(_closestEnemy.baseBody.head.position);
                            NPC.RagdollWalk2Nav();
                            NPC.RagdollBodyLookAt(_closestEnemy.transform.position);
                            NPC.GoTo(_closestEnemy.transform.position);
                        }
                    }
                    break;

                case NPCState.Attacking:
                    if(!_closestEnemy)
                    {
                        // [Attacking] >> Idle
                        NPCStatus = NPCState.Idle;
                        break;
                    }

                    if (NPC.IsOnSight(_closestEnemy.transform.position))
                    {
                        NPC.character.Animator.SetBool("WeaponAiming", true);
                        NPC.character.ArmController.AimWeapon(true);
                        NPC.character.ArmController.RightHandLookAt(_closestEnemy.transform.position);
                        NPC.HeadLookAt(_closestEnemy.baseBody.head.position);
                        NPC.RagdollWalk2Nav();
                        NPC.RagdollBodyLookAt(_closestEnemy.transform.position);
                        NPC.GoTo(_closestEnemy.transform.position);
                        NPC.character.ShootSystem.ShootAsNPC();
                    } 
                    else
                    {
                        // [Attacking] >> Idle
                        NPC.character.Animator.SetBool("WeaponAiming", false);
                        NPCStatus = NPCState.Idle;
                        break;
                    }
                    break;
                case NPCState.LookingForWeapons:
                    // Alert >> [LookingForWeapons]
                    
                    EquipAnyWeapon();
                    break;
                
                case NPCState.Runaway:  // Running away only happens when the NPC doesn't have any weapon, even in its inventory. This isually happens after death.
                    // LookingForWeapons >> [Runaway]

                    NPC.SelfBalance();
                    NPC.RagdollBody2Nav();
                    NPC.RagdollWalk2Nav();
                    NPC.HeadLookAtNav();
                    NPC.GoTo(_closestWeapon.transform.position, 0);

                    if(Vector3.Distance(transform.position, _closestWeapon.transform.position) < 0.5f)
                    {
                        EquipAnyWeapon();
                    }

                    if (NPC.character.EquipmentController.currentWeapon)
                        // Idle << [Runaway]
                        NPCStatus = NPCState.Idle;

                    break;

                case NPCState.Dead:
                    if (!NPC.character.HealthController.IsDead)
                        // [Dead] >> Idle
                        NPCStatus = NPCState.Idle;
                    break;
            }
        }
        void EquipAnyWeapon()
        {
            if (NPC.character.EquipmentController.HasWeapon(Enums.WeaponType.Pistol))
            {
                NPC.character.EquipmentController.EquipWeapon(Enums.WeaponType.Pistol);
                // Idle << [LookingForWeapons]
                NPCStatus = NPCState.Idle;
                return;
            }
            if (NPC.character.EquipmentController.HasWeapon(Enums.WeaponType.Rifle))
            {
                NPC.character.EquipmentController.EquipWeapon(Enums.WeaponType.Shotgun);
                // Idle << [LookingForWeapons]
                NPCStatus = NPCState.Idle;
                return;
            }
            if (NPC.character.EquipmentController.HasWeapon(Enums.WeaponType.Shotgun))
            {
                NPC.character.EquipmentController.EquipWeapon(Enums.WeaponType.Shotgun);
                // Idle << [LookingForWeapons]
                NPCStatus = NPCState.Idle;
                return;
            }
            if (NPC.character.EquipmentController.HasWeapon(Enums.WeaponType.Heavy))
            {
                NPC.character.EquipmentController.EquipWeapon(Enums.WeaponType.Heavy);
                // Idle << [LookingForWeapons]
                NPCStatus = NPCState.Idle;
                return;
            }
            if (NPC.character.EquipmentController.HasWeapon(Enums.WeaponType.Melee))
            {
                NPC.character.EquipmentController.EquipWeapon(Enums.WeaponType.Melee);
                // Idle << [LookingForWeapons]
                NPCStatus = NPCState.Idle;
                return;
            }
            
            // [LookingForGuns] >> Runaway
            NPCStatus = NPCState.Runaway;
        }
    }
}
