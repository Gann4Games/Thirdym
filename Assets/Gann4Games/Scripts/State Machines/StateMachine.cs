using UnityEngine;

namespace Gann4Games.Thirdym.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State State;

        public void SetState(State state)
        {
            State = state;
            State.EnterState(this);
        }
    }
}
