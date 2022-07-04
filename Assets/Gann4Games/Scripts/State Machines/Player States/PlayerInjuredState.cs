using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class PlayerInjuredState : IState
    {
        RagdollController _context;
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.EquipmentController.DropEquippedWeapon();
            _context.SetCrouchAnimationState(true);
            _context.SetRootJointSpring(0); 
            _context.SetLimbsWeight(0.01f, 0);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.HealthController.IsDead) _context.SetState(_context.DeadState);
            if(_context.HealthController.IsAlive) _context.SetState(_context.GroundedState);
            _context.SetVerticalAnimationValue(1);
        }

        public void OnExitState(StateMachine context) {}
    }
}