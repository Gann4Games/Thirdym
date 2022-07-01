using System;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// Underwater state allows for swimming logic.
    /// </summary>
    public class PlayerUnderwaterState : IState
    {
        private const float SwimSpeed = 0.5f;
        private RagdollController _context;
        
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.SetRootJointSpring(100);
            _context.SetRootJointDamping(50);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.enviroment.IsGrounded) 
                _context.SetState(_context.GroundedState);

            int upDirection = (_context.Character.InputHandler.jumping ? 1 : 0) + 
                              (_context.Character.InputHandler.crouching ? -1 : 0);
            
            Vector3 input = new Vector3(_context.MovementAxis.x, upDirection, _context.MovementAxis.y) * SwimSpeed;

            _context.HeadRigidbody.velocity += _context.transform.TransformDirection(input);
            
            _context.SetCrouchAnimationState(false);
            _context.SetRootJointRotation((input.y * 45) - (90 * input.z));
            _context.SetVerticalAnimationValue(input.z);
            _context.MakeGuideLookTowardsCamera();
            _context.MakeRootFollowGuide();
        }

        public void OnExitState(StateMachine context)
        {
            
        }
    }
}