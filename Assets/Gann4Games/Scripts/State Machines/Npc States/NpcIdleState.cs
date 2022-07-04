using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.NPC;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcIdleState : IState
    {
        private TimerTool _timer = new TimerTool(5);
        private NpcRagdollController _context;
        private RagdollController _closestRagdoll;
        private Vector3 _lookTowards;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetRootJointSpring(500);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(!_context.Ragdoll.HealthController.IsAlive) _context.SetState(_context.InjuryState);
            if(_context.Ragdoll.enviroment.IsSwimming) _context.Ragdoll.HealthController.AddHealth(-_context.Ragdoll.HealthController.Health);
            if(!_context.Ragdoll.enviroment.IsGrounded) _context.SetState(_context.JumpState);

            if(_timer.HasFinished()) CheckEnvironment();
            _timer.Count();

            _context.WalkTowardsNavmeshAgent();
            _context.Ragdoll.MakeRootFollowGuide();

            if (!_closestRagdoll) return;
            _context.LookAt(_closestRagdoll.transform.position);
            _context.SetRotationTowards(_lookTowards);

            // TODO: Alert state
            // TODO: Attack state
            if(!_context.IsTargetVisible(_closestRagdoll.BodyRigidbody.transform) || !_closestRagdoll.HealthController.IsAlive) return;
            _context.Ragdoll.SetWeaponAnimationAimState(!_context.Ragdoll.EquipmentController.IsDisarmed);
            _context.Ragdoll.ArmController.RightHandLookAt(_closestRagdoll.transform.position);
            _context.Ragdoll.ShootSystem.ShootAsNPC();
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        private void CheckEnvironment()
        {
            _timer.Reset();
            _timer.SetMaxTime(Random.Range(5, 10));

            _closestRagdoll = _context.GetClosestEnemyAround();
            _lookTowards = _closestRagdoll.transform.position;
            
            // Look for weapons if it is disarmed
            if(!_context.Ragdoll.EquipmentController.HasAnyWeapon && _context.IsAnyWeaponAround) _context.SetState(_context.RunawayState);
            
        }
    }
}