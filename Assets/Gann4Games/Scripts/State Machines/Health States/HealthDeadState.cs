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
            if(_context.Character.isPlayer) MainHUDHandler.SetDamageEffectColor(Color.black, 1, 10);
        }

        public void OnUpdateState(StateMachine context) { }

        public void OnExitState(StateMachine context) { }
    }
}