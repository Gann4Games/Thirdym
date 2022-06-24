using System.Collections;

namespace Gann4Games.Thirdym.StateMachine
{
    public abstract class State
    {
        public abstract void EnterState(StateMachine context);
        public abstract void UpdateState(StateMachine context);
    }
}
