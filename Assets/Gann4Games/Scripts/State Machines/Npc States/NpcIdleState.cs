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
            #region State transitions
            if(!_context.Ragdoll.HealthController.IsAlive) _context.SetState(_context.InjuryState);
            if(_context.Ragdoll.enviroment.IsSwimming) _context.Ragdoll.HealthController.AddHealth(-_context.Ragdoll.HealthController.Health);
            if(!_context.Ragdoll.enviroment.IsGrounded) _context.SetState(_context.JumpState);
            #endregion

            if(_timer.HasFinished()) CheckEnvironment();
            _timer.Count();

            _context.WalkTowardsNavmeshAgent();
            _context.Ragdoll.MakeRootFollowGuide();

            if (!_closestRagdoll) return;
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        private void CheckEnvironment()
        {
            _timer.Reset();
            _timer.SetMaxTime(Random.Range(5, 10));

            if(_context.IsAnyEnemyAround) _context.SetState(_context.AlertState);

            _closestRagdoll = _context.GetClosestCharacterAround();
            _lookTowards = _closestRagdoll.transform.position;
            
            // Look for weapons if it is disarmed
            if(!_context.Ragdoll.EquipmentController.HasAnyWeapon && _context.IsAnyWeaponAround) _context.SetState(_context.RunawayState);
            
        }
    }
}