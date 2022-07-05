using UnityEngine;
using System.Linq;

namespace Gann4Games.Thirdym.StateMachines
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected IState CurrentState;
        public string CurrentStateName => CurrentState.ToString().Split('.').Last();
        public virtual void SetState(IState newState)
        {
            if(CurrentState != null) CurrentState.OnExitState(this);
            
            CurrentState = newState;
            CurrentState.OnEnterState(this);
        }
    }
}
