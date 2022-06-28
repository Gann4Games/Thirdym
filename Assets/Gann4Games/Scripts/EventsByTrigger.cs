using UnityEngine;
using UnityEngine.Events;

public class EventsByTrigger : MonoBehaviour
{
    [SerializeField] protected string detectTag;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

#if UNITY_EDITOR
    BoxCollider _collider => GetComponent<BoxCollider>();

    private void OnDrawGizmos()
    {
        if (_collider)
        {
            Gizmos.color = Color.magenta * new Color(1, 1, 1, .1f);

            Vector3 cubePosition = transform.position + _collider.center;
            Vector3 cubeSize = _collider.size;
            Gizmos.DrawCube(cubePosition, cubeSize);
        }
    }
#endif

    private void OnTriggerEnter(Collider other) => OnEnter(other);
    private void OnTriggerExit(Collider other) => OnExit(other);

    public virtual void OnEnter(Collider other)
    {
        if(!other.CompareTag(detectTag)) return;
        
        onTriggerEnter.Invoke();
    }

    public virtual void OnExit(Collider other)
    {
        if (!other.CompareTag(detectTag)) return;
        
        onTriggerExit.Invoke();
    }
}
