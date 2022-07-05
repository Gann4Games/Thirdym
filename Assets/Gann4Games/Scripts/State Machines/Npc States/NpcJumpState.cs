using UnityEngine;
using Gann4Games.Thirdym.NPC;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcJumpState : IState
    {

        private NpcRagdollController _context;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetGroundedAnimationState(false);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(!_context.Ragdoll.HealthController.IsAlive) _context.SetState(_context.InjuryState);
            if(_context.Ragdoll.enviroment.IsGrounded || _context.Ragdoll.enviroment.IsSwimming) _context.SetState(_context.IdleState);
            
            _context.Ragdoll.SetVerticalAnimationValue(_context.Ragdoll.RelativeZVelocity);
        }

        public void OnExitState(StateMachine context)
        {
            _context.Ragdoll.SetGroundedAnimationState(true);
        }
    }
}