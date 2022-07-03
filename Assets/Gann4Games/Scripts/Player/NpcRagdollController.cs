using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.Enums;
using Gann4Games.Thirdym.StateMachines;
using System.Collections.Generic;

namespace Gann4Games.Thirdym.NPC
{
    public class NpcRagdollController : StateMachine
    {
        [Tooltip("A transform that is used by the head to follow its rotation")]
        public Transform facerTransform;

        public NavMeshAgent navmeshAgent;

        // New code (npc update/rework)
        public RagdollController Ragdoll { get; private set; }
        public CharacterCustomization Character => Ragdoll.Character;
        public CharacterHealthSystem HealthController => Ragdoll.Character.HealthController;

        // States
        private NpcIdleState IdleState = new NpcIdleState();
        private NpcAlertState AlertState = new NpcAlertState();
        private NpcAttackState AttackState = new NpcAttackState();
        private NpcRunawayState RunawayState = new NpcRunawayState();
        private void Awake()
        {
            Ragdoll = GetComponent<RagdollController>();
            SetState(IdleState);
        }

        private void Update()
        {
            // TODO: Npc input provider or something
            // * In order to move a ragdoll as an Npc, it requires input.
            // * A player provides input to the ragdoll in order to take control over it.
            // * Probably im going to need an "InputProvider" class.
            // * PlayerInputHandler does this, but the same features are needed for Npcs.

            CurrentState?.OnUpdateState(this);
        }

        private void OnDamageDealed(object sender, CharacterHealthSystem.OnDamageDealedArgs e)
        {
            if (!HealthController.IsAlive) return;
            LookAt(e.where);
        }
        private void OnEnable() => HealthController.OnDamageDealed += OnDamageDealed;
        private void OnDestroy() => HealthController.OnDamageDealed -= OnDamageDealed;

        //---------------------------------------- New (updated) functionality ----------------------------------------

/*ACTIONS*/

        /// <summary>Makes the NPC look towards the vector specified</summary>
        public void LookAt(Vector3 point) => facerTransform.LookAt(point);

        /// <summary>Makes the NPC look towards its ghost navmesh agent with its head</summary>
        public void LookAtNavmeshAgent() => LookAt(navmeshAgent.transform.position);

        //public void SetRotation(Vector3 rotation) => Ragdoll.makeguide//Ragdoll.guide.transform.eulerAngles = rotation;

        /// <summary>Imitates the navmesh agent's rotation</summary>
        public void SetRotationLikeNavmeshAgent() => Ragdoll.MakeGuideSetRotation(navmeshAgent.transform.rotation, 0.1f);

        public void SetRotationTowards(Vector3 point)
        {
            point.y = Ragdoll.RootJoint.transform.position.y;
            //Ragdoll.RootJoint.transform.LookAt(point);
            Ragdoll.MakeGuideLookTowards(point, 0.1f);
        }

        /// <summary>Walks towards the desired point</summary>
        public void WalkTowards(Vector3 point, float stopDistance = 1)
        {
            //Direction to move feet
            Vector3 feetDirection = transform.InverseTransformDirection(point - transform.position).normalized;

            // Stop feet movement at desired distance
            if (DistanceBetweenNavmesh() < stopDistance) feetDirection = Vector3.zero;

            //Set feet movement on its Y position
            // Ragdoll.SetVerticalAnimationValue(Mathf.Lerp(Ragdoll.GetVerticalAnimationValue(), feetDirection.z, Time.deltaTime));
            Ragdoll.SetVerticalAnimationValue(feetDirection.z);
            //Set feet movement on its X position
            // Ragdoll.SetHorizontalAnimationValue(Mathf.Lerp(Ragdoll.GetHorizontalAnimationValue(), feetDirection.x, Time.deltaTime));
            Ragdoll.SetHorizontalAnimationValue(feetDirection.x);
        }
        public void WalkTowardsNavmeshAgent() => WalkTowards(navmeshAgent.transform.position);

        public void Attack() => Character.ShootSystem.ShootAsNPC();

        /*NAVMESH*/
        public float DistanceBetweenNavmesh() => Vector3.Distance(transform.position, navmeshAgent.transform.position);
        public bool HasArrived => navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance;

        public bool IsNavmeshTooFarAway(float distance = 3) => Vector3.Distance(navmeshAgent.transform.position, transform.position) > distance;

        /// <summary>Avoids the navmesh agent to go too far from the ragdoll</summary>
        /// <param name="maximumDistance">Max distance that the nav is able to travel</param>>
        public void ClampNavmeshAgent(float maximumDistance = 3)
        {
            // This thing here... makes a huge trick to help making unstable ragdoll movement fluent with navmesh. :)
            navmeshAgent.speed = maximumDistance /  DistanceBetweenNavmesh(); 
            
            if (IsNavmeshTooFarAway(maximumDistance))
                navmeshAgent.transform.position = transform.position;
        }

        public void GoTo(Vector3 point, float stopDistance = 2)
        {
            navmeshAgent.stoppingDistance = stopDistance;
            navmeshAgent.SetDestination(point);
        }

/*UTILITIES*/
        /// <returns>A random point around the specified reference point (doesn't use height)</returns>
        public Vector3 RandomPointAround(Vector3 referencePoint, float range = 1)
        {
            float x = Random.Range(-range, range);
            float y = Random.Range(-range, range);
            return referencePoint + new Vector3(x, 0, y);
        }

        /// <summary>Checks wheter the point is on the field of view or not</summary>
        public bool IsOnSight(Vector3 point)
        {
            Vector3 directionToPoint = point - transform.position;
            float product = Vector3.Dot(Ragdoll.Character.baseBody.head.forward, directionToPoint.normalized);
            return product > 0.25f;
        }

/*CHARACTER/RAGDOLL ANALYSIS*/
        public bool IsFriend(CharacterCustomization character) => Character.preset.allyTags.Contains(character.preset.faction);
        public bool IsEnemy(CharacterCustomization character) => Character.preset.enemyTags.Contains(character.preset.faction);

        public IEnumerable<CharacterCustomization> AllCharactersInScene() => FindObjectsOfType<CharacterCustomization>()
            .OrderBy(character => Vector3.Distance(transform.position, character.transform.position))
            .Where(character => character != Character);

        public CharacterCustomization FindClosestCharacterInScene() => AllCharactersInScene().FirstOrDefault();

        /*GLOBAL WEAPONS*/
        public IEnumerable<PickupableWeapon> AllWeaponsInScene() => FindObjectsOfType<PickupableWeapon>().OrderBy(weapon => Vector3.Distance(transform.position, weapon.transform.position));
        public IEnumerable<PickupableWeapon> FindWeaponsOfType(WeaponType weaponType) => AllWeaponsInScene().Where(weapon => weapon.weaponData.weaponType == weaponType);
        public PickupableWeapon FindClosestWeaponOfType(WeaponType weaponType) => FindWeaponsOfType(weaponType).FirstOrDefault();
        public PickupableWeapon FindClosestWeapon() => AllWeaponsInScene().FirstOrDefault();

/*VISIBLE WEAPONS*/
        public IEnumerable<PickupableWeapon> AllVisibleWeaponsInScene() => AllWeaponsInScene().Where(weapon => IsTargetVisible(weapon.transform));
        public IEnumerable<PickupableWeapon> FindVisibleWeaponsOfType(WeaponType weaponType) => AllVisibleWeaponsInScene().Where(weapon => weapon.weaponData.weaponType == weaponType);
        public PickupableWeapon FindClosestVisibleWeaponOfType(WeaponType weaponType) => FindVisibleWeaponsOfType(weaponType).FirstOrDefault();
        public PickupableWeapon FindClosestVisibleWeapon() => AllVisibleWeaponsInScene().FirstOrDefault();

        public bool IsAnyWeaponAround => AllVisibleWeaponsInScene().Count() > 0;

        public bool IsTargetVisible(Transform target)
        {
            Ray ray = new Ray(transform.position, target.transform.position - transform.position);
            Physics.Raycast(ray, out RaycastHit hit);
            return hit.transform == target;
        }
    }
}

