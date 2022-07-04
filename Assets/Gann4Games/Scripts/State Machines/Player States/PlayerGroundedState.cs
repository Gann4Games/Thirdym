using UnityEngine;
using UnityEngine.InputSystem;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// A player is going to have many states with each one having their own restrictions.
    /// Possible states:
    /// Grounded, air, underwater, ragdoll
    /// Possibly can complement with health system states:
    /// Grounded, air, underwater, ragdoll, alive, injured, dead
    /// or
    /// [HealthSystem]  ==  [RagdollController]
    /// Alive           ==  grounded, air, underwater
    /// Injured, Dead   ==  ragdoll
    /// or
    ///
    /// Alive (is a state, and a state machine, aka sub-state machine)
    /// so, class AliveState : StateMachine, IState {}
    ///
    /// Alive
    /// > Grounded
    /// > > Idle
    /// > > Moving
    /// > Air
    /// > Underwater
    /// Injured
    /// > Ragdoll
    /// Dead
    /// > Ragdoll
    /// </summary>
    public class PlayerGroundedState : IState
    {
        private RagdollController _context;
        
        // Sub-states
        private PlayerGrounded_AimingState AimingState = new PlayerGrounded_AimingState();
        private PlayerGrounded_RagdollingState RagdollingState = new PlayerGrounded_RagdollingState();
        
        public void OnEnterState(StateMachine context)
        {
            _context = (RagdollController)context;
            _context.SetGroundedAnimationState(true);
            _context.SetLimbsWeight(1, 0);
            _context.SetRootJointSpring(500);
            _context.SetRootJointDamping(10);
        }

        public void OnUpdateState(StateMachine context)
        {
            if (_context.HealthController.IsInjured) _context.SetState(_context.InjuredState);
            if(_context.InputHandler.aiming) _context.SetState(AimingState);
            if(_context.InputHandler.ragdolling) _context.SetState(RagdollingState);
            if(!_context.enviroment.IsGrounded) _context.SetState(_context.JumpingState);
            if(_context.enviroment.IsSwimming && !_context.enviroment.IsGrounded) _context.SetState(_context.UnderwaterState);
            
            if (PlayerInputHandler.instance.gameplayControls.Player.Jump.triggered) // Basic jump
                _context.JumpAsPlayer();

            _context.SetCrouchAnimationState(PlayerInputHandler.instance.crouching);
            
            // The magnitude is used for the left joystick (gamepad) support
            _context.SetVerticalAnimationValue(PlayerInputHandler.instance.walking
                ? .25f * _context.MovementAxis.magnitude
                : .5f * _context.MovementAxis.magnitude);

                
            // In grounded move, the player is not supposed to strafe, the value will not be zero often when coming from an aiming state (which allows strafing).
            if(_context.GetHorizontalAnimationValue() != 0) _context.SetHorizontalAnimationValue(0);
                    
            _context.SetRootJointRotation(PlayerInputHandler.instance.walking 
                ? 0 
                : -15 * _context.MovementAxis.magnitude);
                    
            _context.MakeGuideLookTowardsMovement();
            _context.MakeRootFollowGuide();
        }

        public void OnExitState(StateMachine context)
        {
        }
    }
}