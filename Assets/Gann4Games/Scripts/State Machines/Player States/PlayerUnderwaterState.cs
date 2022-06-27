using System;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// Underwater state allows for swimming logic.
    /// </summary>
    public class PlayerUnderwaterState : IState
    {
        private const float SwimForce = 2500;
        private RagdollController _context;
        public void OnEnterState(StateMachine context)
        {
            Debug.Log("Its swimming time!");
            _context = (RagdollController)context;
            _context.SetRootJointSpring(100);
            _context.SetRootJointDamping(50);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.enviroment.IsGrounded) 
                _context.SetState(_context.GroundedState);
            
            bool goingUp = PlayerInputHandler.instance.jumping;
            bool goingDown = PlayerInputHandler.instance.crouching;
            
            Vector3 input = new Vector3(
                _context.MovementAxis.x,
                Convert.ToInt32(goingUp) - Convert.ToInt32(goingDown),
                _context.MovementAxis.y);

            Vector3 force = new Vector3(
                SwimForce * input.x,
                SwimForce * input.y,
                SwimForce * input.z) * Time.deltaTime;

            Vector3 relativeForceDirection = (_context.RootJoint.transform.right * force.x) +
                                     (_context.RootJoint.transform.up * force.y) +
                                     (_context.RootJoint.transform.forward * force.z);
            
            _context.HeadRigidbody.AddForce(relativeForceDirection);
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