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

    CharacterCustomization _character;

    void Start()
    {
        _character = PlayerCameraController.Instance.Character;
    }
    private void Update()
    {
        if (UseMode == LookAtUse.FollowTarget)
            transform.LookAt(target);
        else if (UseMode == LookAtUse.CameraCenter)
        {
            if (!_character.HealthController.IsDead)
            {
                transform.LookAt(PlayerCameraController.Instance.CameraCenterPoint);
            }
        }
    }
}
