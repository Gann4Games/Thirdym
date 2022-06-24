using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected IState CurrentState;

        public virtual void SetState(IState newState)
        {
            if(CurrentState != null) CurrentState.ExitState(this);
            
            CurrentState = newState;
            CurrentState.EnterState(this);
        }
    }
}
