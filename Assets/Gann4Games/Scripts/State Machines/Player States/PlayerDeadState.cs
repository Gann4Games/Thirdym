using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
	public class PlayerDeadState : IState
	{
		private RagdollController _context;
		public void OnEnterState(StateMachine context)
		{
			_context = (RagdollController)context;
			_context.Character.EquipmentController.DropAllWeapons();
			_context.Character.PlayDeathSFX();
			_context.SetLimbsWeight(0, 10);

			if (_context.Character.isNPC) return;
			IngameMenuHandler.PauseAndShowMessage("You have died!");
		}

		public void OnUpdateState(StateMachine context)
		{
			_context.SetCrouchAnimationState(true);
			_context.SetVerticalAnimationValue(1);
		}

		public void OnExitState(StateMachine context)
		{
			_context.SetLimbsWeight(1, 5);
			if(_context.Character.isNPC) return;
			IngameMenuHandler.instance.SetPausedStatus(false);
		}
	}
}