using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class HealthAliveState : IState
    {
        // [Alive] -> Injured
        
        private CharacterHealthSystem _context;
        private TimerTool _timer;
        
        public void OnEnterState(StateMachine context)
        {
            _context = (CharacterHealthSystem)context;
            _timer = new TimerTool(3);
        }

        public void OnUpdateState(StateMachine context)
        {
            if(!_context.IsAlive) _context.SetState(_context.InjuredState);
            HealthRegeneration();
        }
        
        private void HealthRegeneration()
        {
            if (_context.MaxHealthReached) { _timer.Reset(); return; }
            _timer.Count();
            if(!_timer.HasFinished()) return;
            _context.AddHealth(Time.deltaTime * _context.HealthPercentage * _context.Character.preset.regeneration_rate);
        }
        
        public void OnExitState(StateMachine context) { }
    }
}