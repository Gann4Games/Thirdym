using UnityEngine;
using Gann4Games.Thirdym.NPC;

namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// The runaway state makes a npc look for guns on the map or simply avoid fighting.
    /// </summary>
    public class NpcRunawayState : IState
    {
        private NpcRagdollController _context;
        private PickupableWeapon _closestWeapon;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _closestWeapon = _context.FindClosestVisibleWeapon();
            _context.Ragdoll.PlayAlertSFX();
        }

        public void OnUpdateState(StateMachine context)
        {
            if (_closestWeapon == null || !_context.Ragdoll.HealthController.IsAlive || _context.Ragdoll.IsUnderwater())
            {
                _context.SetState(_context.IdleState);
                return;
            }

            if(_context.DistanceBetween(_closestWeapon.transform.position) < 1) 
            {
                _closestWeapon.Interact(_context.Ragdoll);
                _context.SetState(_context.IdleState);
            }

            _context.GoTo(_closestWeapon.transform.position, 0);
            _context.LookAt(_closestWeapon.transform.position);
            _context.WalkTowardsNavmeshAgent(0);
            _context.SetRotationLikeNavmeshAgent();
            _context.Ragdoll.MakeRootFollowGuide();
        }

        public void OnExitState(StateMachine context)
        {
        }
    }
}