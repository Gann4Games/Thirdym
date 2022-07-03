using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.NPC;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcIdleState : IState
    {
        private TimerTool _timer = new TimerTool(5);
        private NpcRagdollController _context;
        private CharacterCustomization _closestCharacter;
        private Vector3 _lookTowards;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetRootJointSpring(500);
        }

        public void OnUpdateState(StateMachine context)
        {
            // TODO: Find weapons state
            // TODO: Polish idle state
            // TODO: Wander state
            // TODO: Alert state
            // TODO: Attack state
            // TODO: Injury state
            // TODO: Dead state

            if(!_context.HealthController.IsAlive) _context.SetState(_context.InjuryState);
            if(_context.Ragdoll.enviroment.IsSwimming) _context.HealthController.AddHealth(-_context.HealthController.Health);
            if(!_context.Ragdoll.enviroment.IsGrounded) _context.SetState(_context.JumpState);

            if(_timer.HasFinished()) CheckEnvironment();
            _timer.Count();

            _context.WalkTowardsNavmeshAgent();
            _context.Ragdoll.MakeRootFollowGuide();

            if (!_closestCharacter) return;
            _context.LookAt(_closestCharacter.transform.position);
            _context.SetRotationTowards(_lookTowards);
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        private void CheckEnvironment()
        {
            _timer.Reset();
            _timer.SetMaxTime(Random.Range(5, 10));

            _closestCharacter = _context.FindClosestCharacterInScene();
            _lookTowards = _closestCharacter.transform.position;
            
            // Look for weapons if it is disarmed
            if(!_context.Character.EquipmentController.HasAnyWeapon && _context.IsAnyWeaponAround) _context.SetState(_context.RunawayState);
            
        }
    }
}