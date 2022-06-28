using UnityEngine;
using UnityEngine.Events;

namespace Gann4Games
{
    public class EventsByTriggerTimed : EventsByTrigger
    {
        [SerializeField] private float afterTriggerTime = 5;
        [SerializeField] private UnityEvent afterEnterCallback;
        [SerializeField] private UnityEvent afterExitCallback;

        public override void OnEnter(Collider other)
        {
            base.OnEnter(other);
            if(other.CompareTag(detectTag))
                Invoke(nameof(AfterEnter), afterTriggerTime);
        }

        void AfterEnter() => afterEnterCallback.Invoke();

        public override void OnExit(Collider other)
        {
            base.OnExit(other);
            if(other.CompareTag(detectTag))
                Invoke(nameof(AfterExit), afterTriggerTime);
        }

        void AfterExit() => afterExitCallback.Invoke();
    }
}
