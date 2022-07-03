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

            if(_timer.HasFinished()) CheckEnvironment();
            _timer.Count();

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
        }
    }
}