using Gann4Games.Thirdym.NPC;

namespace Gann4Games.Thirdym.StateMachines
{
    public class NpcDeadState : IState
    {
        private NpcRagdollController _context;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
        }

        public void OnExitState(StateMachine context)
        {
            
        }

        public void OnUpdateState(StateMachine context)
        {
            
        }
    }
}