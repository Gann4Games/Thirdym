using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class PlayerGrounded_AimingState : IState
    {
        private RagdollController _context;
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;

            if (_context.Customizator.isNPC) return;
            MainHUDHandler.instance.crosshair.gameObject.SetActive(true);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(!_context.InputHandler.aiming) _context.SetState(_context.GroundedState);
            
            _context.SetHorizontalAnimationValue(_context.MovementAxis.x/2);
            _context.SetVerticalAnimationValue(_context.MovementAxis.y/2);
            
            _context.SetRootJointRotation(_context.InputHandler.walking ? 0 : _context.MovementAxis.y * -15);
            _context.MakeGuideLookTowardsCamera(Time.deltaTime * 10);
            _context.MakeRootFollowGuide();
        }

        public void OnExitState(StateMachine context)
        {
            MainHUDHandler.instance.crosshair.gameObject.SetActive(false);
        }
    }
}