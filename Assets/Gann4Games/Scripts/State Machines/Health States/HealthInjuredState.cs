using Gann4Games.Thirdym.Utility;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class HealthInjuredState : IState
    {
        // Alive <- [Injured] -> Dead
        
        private CharacterHealthSystem _context;
        private TimerTool _timer = new TimerTool(5);
        
        public override void OnEnterState(StateMachine context)
        {
            _context = (CharacterHealthSystem)context;
            _context.Character.PlayInjurySFX();
        }

        public override void OnUpdateState(StateMachine context)
        {
            if(_context.IsDead) _context.SetState(_context.deadState);
            else if (_context.IsAlive) _context.SetState(_context.aliveState);
            
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

        public override void OnExitState(StateMachine context) { }
    }
}