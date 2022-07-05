namespace Gann4Games.Thirdym.StateMachines
{
    public interface IState
    {
        public void OnEnterState(StateMachine context);
        public void OnUpdateState(StateMachine context);
        public void OnExitState(StateMachine context);
    }
}
