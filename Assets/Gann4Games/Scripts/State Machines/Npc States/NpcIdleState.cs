using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.NPC;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcIdleState : IState
    {
        private TimerTool _timer = new TimerTool(2);
        private NpcRagdollController _context;
        private RagdollController _closestRagdoll;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetRootJointSpring(500);
            _context.Ragdoll.SetWeaponAnimationAimState(false);
        }

        public void OnUpdateState(StateMachine context)
        {
            #region State transitions
            if(_context.Ragdoll.enviroment.IsSwimming) _context.Ragdoll.HealthController.AddHealth(-_context.Ragdoll.HealthController.Health);
            if (!_context.Ragdoll.HealthController.IsAlive)
            {
                _context.SetState(_context.InjuryState);
                return;
            }
            if (!_context.Ragdoll.enviroment.IsGrounded)
            {
                _context.SetState(_context.JumpState);
                return;
            }
            #endregion

            _timer.Count();
            if(_timer.HasFinished()) OnTimerFinish();

            if(!_closestRagdoll) _context.SetRotationLikeNavmeshAgent();
            if(_closestRagdoll) _context.LookAt(_closestRagdoll.HeadRigidbody.transform.position);
            _context.Ragdoll.ArmController.AimWeapon(false);
            _context.WalkTowardsNavmeshAgent();
            _context.Ragdoll.MakeRootFollowGuide();
            _context.ClampNavmeshAgent();
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        private void OnTimerFinish()
        {
            _timer.Reset();
            _closestRagdoll = _context.GetClosestCharacterAround();
            if(_closestRagdoll) _context.SetRotationTowards(_closestRagdoll.transform.position);

            LookForWeaopns();
            LookForEnemies();
        }

        private void LookForEnemies() { if (_context.IsAnyEnemyAround && !_context.Ragdoll.EquipmentController.IsDisarmed) _context.SetState(_context.AlertState); }
        private void LookForWeaopns() { if(!_context.Ragdoll.EquipmentController.HasAnyWeapon && _context.IsAnyWeaponAround) _context.SetState(_context.RunawayState); }
    }
}