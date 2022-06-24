namespace Gann4Games.Thirdym.StateMachines
{
    /// <summary>
    /// A player is going to have many states with each one having their own restrictions.
    /// Possible states:
    /// Grounded, air, underwater, ragdoll
    /// Possibly can complement with health system states:
    /// Grounded, air, underwater, ragdoll, alive, injured, dead
    /// or
    /// [HealthSystem]  ==  [RagdollController]
    /// Alive           ==  grounded, air, underwater
    /// Injured, Dead   ==  ragdoll
    /// or
    ///
    /// Alive (is a state, and a state machine, aka sub-state machine)
    /// so, class AliveState : StateMachine, IState {}
    ///
    /// Alive
    /// > Grounded
    /// > > Idle
    /// > > Moving
    /// > Air
    /// > Underwater
    /// Injured
    /// > Ragdoll
    /// Dead
    /// > Ragdoll
    /// </summary>
    public class PlayerGroundedState : IState
    {
        public override void OnEnterState(StateMachine context)
        {
            throw new System.NotImplementedException();
        }

        public override void OnUpdateState(StateMachine context)
        {
            throw new System.NotImplementedException();
        }

        public override void OnExitState(StateMachine context)
        {
            throw new System.NotImplementedException();
        }
    }
}