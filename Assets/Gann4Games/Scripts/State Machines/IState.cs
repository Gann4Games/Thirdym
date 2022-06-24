using System.Collections;
using UnityEngine;

namespace Gann4Games.Thirdym.StateMachines
{
    public abstract class IState
    {
        public abstract void OnEnterState(StateMachine context);
        public abstract void OnUpdateState(StateMachine context);
        public abstract void OnExitState(StateMachine context);
    }
}
