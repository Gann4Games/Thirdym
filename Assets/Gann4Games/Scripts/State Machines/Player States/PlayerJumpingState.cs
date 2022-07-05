using System;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// The jumping state can be also be considered as the 'Air' state.
    /// </summary>
    public class PlayerJumpingState : IState
    {
        private RagdollController _context;
        private Vector3 _startVelocity;
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.SetRootJointDamping(10);
            _context.SetGroundedAnimationState(false);
            _startVelocity = _context.relativeVelocity;
        }

        public void OnUpdateState(StateMachine context)
        {
            #region Transitions
            if(_context.HealthController.IsDead) _context.SetState(_context.DeadState);
            if(_context.IsUnderwater()) _context.SetState(_context.UnderwaterState);
            if(_context.IsGrounded()) _context.SetState(_context.GroundedState);
            if(_context.InputHandler.jumping) WalljumpVerification();
            #endregion

            _context.MakeRootFollowGuide();
            _context.SetVerticalAnimationValue(_context.RelativeZVelocity);

            bool ragdolling = _context.InputHandler.ragdolling;
            _context.SetCrouchAnimationState(ragdolling);
            _context.SetRootJointRotation(ragdolling
                ?_context.MovementAxis.y*-90
                :Mathf.Clamp(_context.RelativeZVelocity,-15, 15));
        }

        private void WalljumpVerification()
        {
            Vector3 walljumpDirection = _startVelocity;
            
            Ray ray = new Ray(_context.transform.position, walljumpDirection);
            if(Physics.Raycast(ray, out RaycastHit hit, 0.5f, ~LayerMask.GetMask("Ragdoll"), QueryTriggerInteraction.Ignore))
            {
                walljumpDirection = Vector3.Reflect(ray.direction, hit.normal);
                _context.JumpTowards(walljumpDirection);
                _startVelocity = walljumpDirection;
                Debug.DrawRay(ray.origin, walljumpDirection, Color.green, 1);
            }
        }

        public void OnExitState(StateMachine context)
        {
            _context.SetGroundedAnimationState(true);
        }
    }
}