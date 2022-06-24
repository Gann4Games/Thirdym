using System.Collections;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public abstract class IState
    {
        public abstract void EnterState(StateMachine context);
        public abstract void UpdateState(StateMachine context);
        public abstract void ExitState(StateMachine context);
    }
}
