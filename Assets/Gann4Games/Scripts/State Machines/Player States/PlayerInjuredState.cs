using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class PlayerInjuredState : IState
    {
        RagdollController _context;
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.SetCrouchAnimationState(true);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.Character.HealthController.IsDead) _context.SetState(_context.DeadState);
            if(_context.Character.HealthController.IsAlive) _context.SetState(_context.GroundedState);
            
            _context.SetVerticalAnimationValue(1);
            _context.SetRootJointSpring(0); 
        }

        public void OnExitState(StateMachine context) {}
    }
}