using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class HealthInjuredState : IState
    {
        // Alive <- [Injured] -> Dead
        
        private CharacterHealthSystem _context;
        private TimerTool _timer = new TimerTool(5);
        
        public void OnEnterState(StateMachine context)
        {
            _context = (CharacterHealthSystem)context;
            _context.Character.PlayInjurySFX();
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.IsDead) _context.SetState(_context.DeadState);
            else if (_context.IsAlive) _context.SetState(_context.AliveState);
            
            Bleedout();
        }

        void Bleedout()
        {
            _timer.Count();
            if (_timer.HasFinished())
            {
                _context.DealDamage(20, Vector3.zero, true);
                _timer.Reset();
            }
        }

        public void OnExitState(StateMachine context) { }
    }
}