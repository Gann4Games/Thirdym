namespace Gann4Games.Thirdym.StateMachines
{
	public class PlayerGrounded_RagdollingState : IState
	{
		RagdollController _context;
		public void OnEnterState(StateMachine context)
		{
			_context = (RagdollController)context;
			_context.SetLimbsWeight(0, 0);
			_context.SetRootJointDamping(0);
			_context.SetRootJointSpring(0);
		}

		public void OnUpdateState(StateMachine context)
		{
			if(!_context.InputHandler.ragdolling && (_context.enviroment.IsGrounded || _context.enviroment.IsSwimming)) _context.SetState(_context.GroundedState);
			if(!_context.HealthController.IsAlive) _context.SetState(_context.InjuredState);
		}

		public void OnExitState(StateMachine context)
		{
		}
	}
}