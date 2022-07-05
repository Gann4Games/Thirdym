using Gann4Games.Thirdym.NPC;
using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcAttackState : IState
    {

        private NpcRagdollController _context;
        private RagdollController _closestEnemy;
        private TimerTool _timer = new TimerTool(5);

        private Vector3 desiredPosition => _context.RandomPointAround(_closestEnemy.transform.position, 5);

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            ScanEnvironment();
        }

        public void OnUpdateState(StateMachine context)
        {
            #region State transitions
            if(!_context.Ragdoll.HealthController.IsAlive) 
                _context.SetState(_context.IdleState);

            if(!_context.IsTargetVisible(_closestEnemy.BodyRigidbody.transform) || !_closestEnemy.HealthController.IsAlive) 
                _context.SetState(_context.AlertState);
            #endregion

            if(_timer.HasFinished()) ScanEnvironment();
            _timer.Count();

            if(!_closestEnemy) return;

            _context.Ragdoll.SetWeaponAnimationAimState(!_context.Ragdoll.EquipmentController.IsDisarmed);
            _context.Ragdoll.ArmController.RightHandLookAt(_closestEnemy.transform.position);
            _context.Attack();

            // Navigation
            _context.WalkTowardsNavmeshAgent();
            _context.SetRotationTowards(_closestEnemy.transform.position);
            _context.Ragdoll.MakeRootFollowGuide();
        }

        private void ScanEnvironment()
        {
            _timer.Reset();
            _closestEnemy = _context.GetClosestEnemyAround();
            if(_closestEnemy) _context.GoTo(desiredPosition);
        }

        public void OnExitState(StateMachine context){}
    }
}