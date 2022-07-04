using Gann4Games.Thirdym.NPC;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcDeadState : IState
    {
        private NpcRagdollController _context;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetLimbsWeight(0, 0);
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        public void OnUpdateState(StateMachine context)
        {
            
        }
    }
}