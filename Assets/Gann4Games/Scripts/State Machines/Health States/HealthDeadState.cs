using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class HealthDeadState : IState
    {
        // Injured -> [Dead]
        
        private CharacterHealthSystem _context;
        public void OnEnterState(StateMachine context)
        {
            _context = (CharacterHealthSystem)context;
        }

        public void OnUpdateState(StateMachine context) { }

        public void OnExitState(StateMachine context) { }
    }
}