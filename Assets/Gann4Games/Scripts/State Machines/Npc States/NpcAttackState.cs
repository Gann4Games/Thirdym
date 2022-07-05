using Gann4Games.Thirdym.NPC;
using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcAttackState : IState
    {

        private NpcRagdollController _context;
        private RagdollController _closestEnemy;

        private int _dodgeDistance;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _closestEnemy = _context.GetClosestEnemyAround();
            _dodgeDistance = Random.Range(-5, 5);
        }

        public void OnUpdateState(StateMachine context)
        {
            #region State transitions
            if(!_context.Ragdoll.HealthController.IsAlive) 
                _context.SetState(_context.IdleState);

            if(!_context.IsTargetVisible(_closestEnemy.BodyRigidbody.transform) || !_closestEnemy.HealthController.IsAlive) 
                _context.SetState(_context.AlertState);
            #endregion

            if(!_closestEnemy) return;

            _context.Ragdoll.SetWeaponAnimationAimState(!_context.Ragdoll.EquipmentController.IsDisarmed);
            _context.Ragdoll.ArmController.RightHandLookAt(_closestEnemy.transform.position);
            _context.Attack();

            // Navigation
            if(_closestEnemy) _context.GoTo((_closestEnemy.transform.right * _dodgeDistance) + _closestEnemy.transform.forward);
            _context.WalkTowardsNavmeshAgent();
            _context.SetRotationTowards(_closestEnemy.transform.position);
            _context.Ragdoll.MakeRootFollowGuide();
        }

        public void OnExitState(StateMachine context){}
    }
}