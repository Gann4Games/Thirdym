using System.Linq;
using UnityEngine;
using UnityEngine.AI;
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

        public ObjectScanner scanner;

        // States
        public NpcIdleState IdleState = new NpcIdleState();
        public NpcJumpState JumpState = new NpcJumpState();
        public NpcAlertState AlertState = new NpcAlertState();
        public NpcAttackState AttackState = new NpcAttackState();
        public NpcRunawayState RunawayState = new NpcRunawayState();
        public NpcDeadState DeadState = new NpcDeadState();

        internal void GoTo(object desiredPosition)
        {
            throw new System.NotImplementedException();
        }

        public NpcInjuryState InjuryState = new NpcInjuryState();
        private void Awake()
        {
            Ragdoll = GetComponent<RagdollController>();
        }

        private void OnEnable() 
        {
            Ragdoll.OnReady += Initialize;
            Ragdoll.HealthController.OnDamageDealed += OnDamageDealed;
        } 

        private void OnDestroy()
        {
            Ragdoll.OnReady -= Initialize;
            Ragdoll.HealthController.OnDamageDealed -= OnDamageDealed;
        } 

        private void Initialize(object sender, System.EventArgs e)
        {
            SetState(IdleState);
        }

        private void Update()
        {
            CurrentState?.OnUpdateState(this);

            // TODO: Npc input provider or something
            // * In order to move a ragdoll as an Npc, it requires input.
            // * A player provides input to the ragdoll in order to take control over it.
            // * Probably im going to need an "InputProvider" class.
            // * PlayerInputHandler does this, but the same features are needed for Npcs.
        }

        private void OnDamageDealed(object sender, CharacterHealthSystem.OnDamageDealedArgs e)
        {
            if (!Ragdoll.HealthController.IsAlive) return;
            LookAt(e.where);
        }

        //---------------------------------------- New (updated) functionality ----------------------------------------

/*ACTIONS*/

        /// <summary>Makes the NPC look towards the vector specified</summary>
        public void LookAt(Vector3 point) => facerTransform.LookAt(point);

        /// <summary>Makes the NPC look towards its ghost navmesh agent with its head</summary>
        public void LookAtNavmeshAgent() => LookAt(navmeshAgent.transform.position);

        //public void SetRotation(Vector3 rotation) => Ragdoll.makeguide//Ragdoll.guide.transform.eulerAngles = rotation;

        /// <summary>Imitates the navmesh agent's rotation</summary>
        public void SetRotationLikeNavmeshAgent(float lerpTimeClamped = 0.1f) => Ragdoll.MakeGuideSetRotation(navmeshAgent.transform.rotation, lerpTimeClamped);

        public void SetRotationTowards(Vector3 point, float lerpTimeClamped = 0.1f) => Ragdoll.MakeGuideLookTowards(point, lerpTimeClamped);

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
        public void WalkTowardsNavmeshAgent(float stopDistance = 1) => WalkTowards(navmeshAgent.transform.position, stopDistance);

        public void Attack() => Ragdoll.ShootSystem.ShootAsNPC();

/*NAVMESH*/
        public float DistanceBetween(Vector3 position) => Vector3.Distance(transform.position, position);
        public float DistanceBetweenNavmesh() => DistanceBetween(navmeshAgent.transform.position);
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
            float product = Vector3.Dot(Ragdoll.Customizator.baseBody.head.forward, directionToPoint.normalized);
            return product > 0.25f;
        }

/*CHARACTER/RAGDOLL ANALYSIS*/
        public bool IsFriend(RagdollController character) => Ragdoll.Customizator.preset.allyTags.Contains(character.Customizator.preset.faction);
        public bool IsEnemy(RagdollController character) => Ragdoll.Customizator.preset.enemyTags.Contains(character.Customizator.preset.faction);


        /*
        CHARACTER ANALYSIS FUNCTIONALITY
        All characters (Ordered by distance)
        L All visible characters
           L All alive characters
                L Get closest (first) character (any)
                L Get all enemies
                    L Get closest (first) enemy
                L Get all allies
                    L Get closest (first) ally

        DESIRED WEAPON ANALYSIS FUNCTIONALITY
        All weapons (Ordered by disatnce)
        L All visible weapons
            L Get closest weaopn of type X
            L Get closest weapon
        */

        public bool IsAnyEnemyAround => GetAllEnemiesAround().Count() > 0;
        public bool IsAnyFriendAround => GetAllFriendsAround().Count() > 0;
        public bool IsAnyCharacterAround => AllVisibleCharactersInScene().Count() > 0;

        /// <returns>All characters in scene ordered by distance</returns>
        public IEnumerable<RagdollController> AllCharactersInScene() => FindObjectsOfType<RagdollController>().OrderBy(character => Vector3.Distance(transform.position, character.transform.position)).Where(ragdoll => ragdoll != Ragdoll);
        public IEnumerable<RagdollController> AllVisibleCharactersInScene() => AllCharactersInScene().Where(ragdoll => IsTargetVisible(ragdoll.BodyRigidbody.transform));
        public IEnumerable<RagdollController> AllAliveCharactersAround() => AllVisibleCharactersInScene().Where(ragdoll => ragdoll.HealthController.IsAlive);

        public RagdollController GetClosestCharacterAround() => AllAliveCharactersAround().FirstOrDefault();

        /// <returns>All visible enemies that are alive</returns>
        public IEnumerable<RagdollController> GetAllEnemiesAround() => AllAliveCharactersAround().Where(ragdoll => IsEnemy(ragdoll));
        
        /// <returns>The closest visible enemy that's alive</returns>
        public RagdollController GetClosestEnemyAround() => GetAllEnemiesAround().FirstOrDefault();
        
        /// <returns>All visible friends that are alive</returns>
        public IEnumerable<RagdollController> GetAllFriendsAround() => AllAliveCharactersAround().Where(ragdoll => IsFriend(ragdoll));
        
        /// <returns>The closest visible friend that's alive</returns>
        public RagdollController GetClosestFriendAround() => GetAllFriendsAround().FirstOrDefault();

        /*GLOBAL WEAPONS*/
        /// <returns>All weapons in scene ordered by distance</returns>
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

        public bool IsTargetVisible(Transform target, float distance = 50)
        {
            Ray ray = new Ray(transform.position, target.transform.position - transform.position);
            Physics.Raycast(ray, out RaycastHit hit, distance, ~LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position, hit.point, Color.cyan, 2);
            return hit.transform == target;
        }
    }
}

