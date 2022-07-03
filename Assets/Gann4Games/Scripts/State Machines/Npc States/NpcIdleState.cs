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
        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetRootJointSpring(500);
            _closestCharacter = _context.FindClosestCharacterInScene();
        }

        public void OnUpdateState(StateMachine context)
        {
            _context.LookAt(_closestCharacter.transform.position);
            _context.GoTo(_closestCharacter.transform.position);
            _context.Ragdoll.SetRootJointRotation(_context.Ragdoll.GetVerticalAnimationValue() * -15);
            _context.Ragdoll.MakeRootFollowGuide();
            _context.WalkTowardsNavmeshAgent();
            _context.SetRotationLikeNavmeshAgent();
            if(_context.IsNavmeshTooFarAway(2)) _context.Ragdoll.JumpTowards((_context.navmeshAgent.transform.position - _context.transform.position).normalized);
            _context.ClampNavmeshAgent();
        }

        public void OnExitState(StateMachine context)
        {
        }
    }
}