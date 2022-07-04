using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// The jumping state can be also be considered as the 'Air' state.
    /// </summary>
    public class PlayerJumpingState : IState
    {
        private RagdollController _context;
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.SetRootJointDamping(10);
            _context.SetGroundedAnimationState(false);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.HealthController.IsDead) _context.SetState(_context.DeadState);
            if(_context.enviroment.IsGrounded) _context.SetState(_context.GroundedState);
            if(_context.enviroment.IsSwimming) _context.SetState(_context.UnderwaterState);
            
            _context.MakeRootFollowGuide();
            _context.SetVerticalAnimationValue(_context.RelativeZVelocity);

            bool ragdolling = _context.InputHandler.ragdolling;
            _context.SetCrouchAnimationState(ragdolling);
            _context.SetRootJointRotation(ragdolling
                ?_context.MovementAxis.y*-90
                :Mathf.Clamp(_context.RelativeZVelocity,-15, 15));
        }

        public void OnExitState(StateMachine context)
        {
            _context.SetGroundedAnimationState(true);
        }
    }
}