using UnityEngine;
using Gann4Games.Thirdym.Enums;

[System.Serializable] 
public class TpMode
{
    [HideInInspector] public Vector3 startOffset;
    [HideInInspector] public float start_pos_lerp;

    public Vector2 sensitivity = new Vector2(5, 5);
    public Vector3 offset;
    public Vector3 offset_aiming;
    public Vector3 position;
    public Vector3 rotation;
    [Range(0, 1)] public float pos_lerp = 1, rot_lerp = 1, aim_pos_lerp = 1;
}
[System.Serializable] public class FlyMode
{
    [Range(0, 1)] public float lerp = .2f;
    public Transform followTarget;
}
[System.Serializable] public class VehicleMode
{
    public VehicleType vType;

    public Vector2 vehicleSensitivity = new Vector3(5, 5);

    [Header("Mobile Configuration")]
    public Vector3 mobilePosOffset = new Vector3(.2f, .4f, -0.75f);
    public Vector3 mobileRotOffset;
    [Range(0, 1)] public float mobileLerp = .2f;
    public Transform mobileTransform;

    [Header("Walker Configuration")]
    public Vector3 walkerPosOffset = new Vector3(4.5f, -7, 3);
    public Vector3 walkerRotOffset = new Vector3(135, 180, 0);
    [Range(0, 1)] public float walkerLerp = .2f;
    public Transform walkerTransform;
}
[System.Serializable] public class ButtonSwitchMode
{
    public Transform target;
    public Vector3 posOffset;
    public Vector3 rotOffset;
    [Range(0, 1)]public float lerp = .2f;
}
public class PlayerCameraController : MonoBehaviour
{
    public static PlayerCameraController instance;
    public CameraMode camMode;
    public Camera activeCamera;
    public TpMode tpConfig;
    public FlyMode flyConfig;
    public VehicleMode vehicleConfig;
    public ButtonSwitchMode buttonConfig;
    
    [HideInInspector] public CharacterCustomization character;
    CharacterHealthSystem health;

    public Vector3 CameraCenterPoint
    {
        get
        {
            Vector3 startPosition = instance.activeCamera.transform.position;
            Vector3 rayDirection = instance.activeCamera.transform.forward;
            RaycastHit hit;
            LayerMask mask = ~(1 << LayerMask.NameToLayer("Ragdoll"));
            if (Physics.Raycast(startPosition, rayDirection, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                return hit.point;
            else
                return Vector3.zero;
        }
    }
    
    public static Vector3 GetCameraAngle() => instance.activeCamera.transform.eulerAngles;


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(tpConfig.offset));
        Gizmos.DrawWireSphere(transform.position+transform.TransformDirection(tpConfig.offset), .25f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.TransformDirection(tpConfig.offset), transform.position + transform.TransformDirection(tpConfig.offset_aiming));
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(tpConfig.offset_aiming), .1f);
    }
    private void Awake()
    {
        instance = this;
        character = GetComponent<CharacterCustomization>();
    }
    void Start()
    {
        health = character.HealthController;
        tpConfig.startOffset = tpConfig.offset;
        tpConfig.start_pos_lerp = tpConfig.pos_lerp;
    }
    private void Update()
    {
        switch (camMode)
        {
            case CameraMode.Player:
                if (health.IsAlive)
                {
                    ThirdPersonCam();
                    activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, tpConfig.position + activeCamera.transform.TransformDirection(tpConfig.offset), tpConfig.pos_lerp);
                    activeCamera.transform.eulerAngles = Vector3.Lerp(activeCamera.transform.eulerAngles, tpConfig.rotation, tpConfig.rot_lerp);
                }
                else DeathCamera();
                break;
            case CameraMode.FlyCam:
                activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, flyConfig.followTarget.position, flyConfig.lerp);
                activeCamera.transform.rotation = flyConfig.followTarget.rotation;
                break;
            case CameraMode.Vehicle:
                // newlines
                vehicleConfig.mobileRotOffset += new Vector3(-CameraMovement().x * vehicleConfig.vehicleSensitivity.x, CameraMovement().y * vehicleConfig.vehicleSensitivity.y, 0);
                vehicleConfig.walkerRotOffset += new Vector3(-CameraMovement().x * vehicleConfig.vehicleSensitivity.x, CameraMovement().y * vehicleConfig.vehicleSensitivity.y, 0);
                // newlines
                switch (vehicleConfig.vType)
                {
                    case VehicleType.Mobile:
                        Vector3 mobPos = vehicleConfig.mobileTransform.position + activeCamera.transform.TransformDirection(vehicleConfig.mobilePosOffset);
                        activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, mobPos, vehicleConfig.mobileLerp);
                        activeCamera.transform.eulerAngles = vehicleConfig.mobileTransform.eulerAngles + vehicleConfig.mobileRotOffset;
                        break;
                    case VehicleType.Walker:
                        Vector3 walkerPos = vehicleConfig.walkerTransform.position + vehicleConfig.walkerTransform.TransformDirection(vehicleConfig.walkerPosOffset);
                        activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, walkerPos, vehicleConfig.walkerLerp);
                        activeCamera.transform.eulerAngles = vehicleConfig.walkerTransform.eulerAngles + vehicleConfig.walkerRotOffset;
                        break;
                }
                break;
            case CameraMode.ButtonSwitch:
                Vector3 btnPos = buttonConfig.target.position + buttonConfig.target.TransformDirection(buttonConfig.posOffset);
                activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, btnPos, buttonConfig.lerp);
                activeCamera.transform.rotation = Quaternion.Lerp(activeCamera.transform.rotation, buttonConfig.target.rotation * Quaternion.Euler(buttonConfig.rotOffset), buttonConfig.lerp);
                break;
        }
    }
    Vector2 CameraMovement() => new Vector2(!IngameMenuHandler.instance.paused ? PlayerInputHandler.instance.cameraAxis.y : 0, !IngameMenuHandler.instance.paused ? PlayerInputHandler.instance.cameraAxis.x : 0);
    void ThirdPersonCam()
    {
        tpConfig.rotation = new Vector3(
            activeCamera.transform.eulerAngles.x - CameraMovement().x * tpConfig.sensitivity.y,
            activeCamera.transform.eulerAngles.y + CameraMovement().y * tpConfig.sensitivity.x,
            0);
        if (PlayerInputHandler.instance.cameraSwitch && !IngameMenuHandler.instance.paused)
        {
            tpConfig.startOffset = new Vector3(-tpConfig.startOffset.x, tpConfig.startOffset.y, tpConfig.startOffset.z);
            tpConfig.offset_aiming = new Vector3(-tpConfig.offset_aiming.x, tpConfig.offset_aiming.y, tpConfig.offset_aiming.z);
        }
        if (!PlayerInputHandler.instance.aiming || character.RagdollController.enviroment.IsDraggingBody)
        {
            tpConfig.offset = tpConfig.startOffset;
            tpConfig.position = health.transform.position;
            tpConfig.pos_lerp = Mathf.Lerp(tpConfig.pos_lerp, tpConfig.start_pos_lerp, Time.deltaTime*10);
        }
        else if(PlayerInputHandler.instance.aiming && !character.RagdollController.enviroment.IsDraggingBody)
        {
            if (!IngameMenuHandler.instance.paused)
            {
                tpConfig.position = transform.position;
                #region First person aiming
                //Vector3 weaponPosition = health.GetComponent<equipmentTest>().IK.transform.position;
                //tpConfig.offset = new Vector3(-.1f, .2f, 0);
                //tpConfig.pos_lerp = Mathf.Lerp(tpConfig.pos_lerp, 1, tpConfig.aim_pos_lerp/*Time.deltaTime*10*/);
                #endregion
                tpConfig.offset = tpConfig.offset_aiming;
            }
        }
    }
    void DeathCamera()
    {
        float height = 0.5f;
        Vector3 lookAtPoint = character.ArmController.Neck[0].transform.position;
        Vector3 desiredPosition = health.transform.position + Vector3.up*height;
        activeCamera.transform.LookAt(lookAtPoint);
        activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, desiredPosition, Time.deltaTime);
    }
}
