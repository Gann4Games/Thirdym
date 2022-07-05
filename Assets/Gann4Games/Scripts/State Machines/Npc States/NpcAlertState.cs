using Gann4Games.Thirdym.NPC;
using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcAlertState : IState
    {

        private NpcRagdollController _context;
        private RagdollController _closestEnemy;
        private TimerTool _timer;
        private Vector3 desiredPosition => _closestEnemy.transform.position + _closestEnemy.transform.TransformDirection(Vector3.right * _choosenDirection);
        private int[] _walkDirection = new int[] { -5, 5 };
        private int _choosenDirection;

        public void OnEnterState(StateMachine context)
        {
            _timer = new TimerTool(2);
            _context = context as NpcRagdollController;
            _closestEnemy = _context.GetClosestEnemyAround();
            _choosenDirection = _walkDirection[Random.Range(0, _walkDirection.Length)];
        }

        public void OnUpdateState(StateMachine context)
        {
            if(!_context.Ragdoll.HealthController.IsAlive) _context.SetState(_context.IdleState);

            _timer.Count();
            if(_timer.HasFinished()) CheckThreat();

            if(!_closestEnemy) return;
            _context.Ragdoll.SetWeaponAnimationAimState(!_context.Ragdoll.EquipmentController.IsDisarmed);

            //Navigation
            _context.GoTo(desiredPosition, 0);
            _context.LookAt(_closestEnemy.Customizator.baseBody.head.position);
            _context.WalkTowardsNavmeshAgent(0);
            _context.SetRotationTowards(_closestEnemy.transform.position);
            _context.Ragdoll.MakeRootFollowGuide();
        }

        private void CheckThreat()
        {
            if(_context.IsAnyEnemyAround) _context.SetState(_context.AttackState);
            else _context.SetState(_context.IdleState);
        }

        public void OnExitState(StateMachine context)
        {
            
        }
    }
}