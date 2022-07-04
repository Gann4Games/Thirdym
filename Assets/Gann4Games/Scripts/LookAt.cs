using UnityEngine;

public class LookAt : MonoBehaviour
{
    public enum LookAtUse
    {
        FollowTarget,
        CameraCenter
    }
    public LookAtUse UseMode;
    [Tooltip("Required only if 'FollowTarget' is set")]
    public Transform target;

    RagdollController _ragdoll;

    void Start()
    {
        _ragdoll = PlayerCameraController.Instance.Ragdoll;
    }
    private void Update()
    {
        if (UseMode == LookAtUse.FollowTarget)
            transform.LookAt(target);
        else if (UseMode == LookAtUse.CameraCenter)
        {
            if (!_ragdoll.HealthController.IsDead)
            {
                transform.LookAt(PlayerCameraController.Instance.CameraCenterPoint);
            }
        }
    }
}
