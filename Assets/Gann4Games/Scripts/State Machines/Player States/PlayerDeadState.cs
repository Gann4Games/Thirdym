namespace Gann4Games.Thirdym.StateMachines
{
	public class PlayerDeadState : IState
	{
		private RagdollController _context;
		public void OnEnterState(StateMachine context)
		{
			_context = context as RagdollController;
			_context.EquipmentController.DropAllWeapons();
			_context.PlayDeathSFX();
			_context.SetLimbsWeight(0, 0);

            if (_context.Customizator.isNPC) return;
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
			if(_context.Customizator.isNPC) return;
			IngameMenuHandler.instance.SetPausedStatus(false);
		}
	}
}