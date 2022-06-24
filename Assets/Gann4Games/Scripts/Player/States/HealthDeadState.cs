using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public class HealthDeadState : IState
    {
        // Injured -> [Dead]
        
        private CharacterHealthSystem _context;
        public override void EnterState(StateMachine context)
        {
            _context = (CharacterHealthSystem)context;
            _context.Character.EquipmentController.DropAllWeapons();
            _context.Character.PlayDeathSFX();

            if (_context.Character.isNPC) return;
            IngameMenuHandler.PauseAndShowMessage("You have died!");
        }

        public override void UpdateState(StateMachine context) { }

        public override void ExitState(StateMachine context) { }
    }
}