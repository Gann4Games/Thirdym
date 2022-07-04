using UnityEngine;

public class CharacterWalljump : MonoBehaviour
{
    public bool canWallJump => bouncePoint.y > rayStartPosition.y;
    public Vector3 bounceDirection 
    {
        get 
        {
            if(Physics.Raycast(rayStartPosition, rayDirection, out RaycastHit hit, 1, whatIsMap))
            {
                Vector3 reflection = Vector3.Reflect(rayDirection, hit.normal);
                Debug.DrawLine(hit.point, hit.point+reflection, Color.white, 1);
                return reflection;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
    public Vector3 bouncePoint
    {
        get
        {
            if(Physics.Raycast(rayStartPosition, rayDirection, out RaycastHit hit, 1, whatIsMap))
            {
                Debug.DrawLine(rayStartPosition, hit.point, Color.white, 1);
                return hit.point;
            }
            else
            {
                return transform.position - Vector3.up*1; // Lower than current transform so it doesnt allow to jump.
            }
        }
    }
    [SerializeField] LayerMask whatIsMap;
    RagdollController _ragdoll;

    Vector3 rayStartPosition => _ragdoll.enviroment.transform.position;
    Vector3 rayDirection => _ragdoll.BodyRigidbody.velocity;
    private void Awake() 
    {
        _ragdoll = GetComponent<RagdollController>();
    }
}
