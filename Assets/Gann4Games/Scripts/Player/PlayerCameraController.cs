using System;
using Cinemachine;
using UnityEngine;
public class PlayerCameraController : MonoBehaviour
{
    public static CinemachineBrain CameraBrain { get; private set; }
    public static PlayerCameraController Instance;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private CinemachineVirtualCamera deathCamera;

    [Header("Third person settings")]
    [SerializeField] private Vector2 sensitivity = Vector2.one;
    [SerializeField] private Vector3 offset = new Vector3(0.2f, 0.4f, -1.5f);
    [Range(0, 1)]
    [SerializeField] private float positionLerp = 0.5f;
    [SerializeField] private float rotationLerp = 1;

    private float _cameraRotationX;
    private float _cameraRotationY;

    public RagdollController Ragdoll { get; private set; }

    public Vector3 CameraCenterPoint
    {
        get
        {
            Vector3 startPosition = CameraBrain.transform.position;
            Vector3 rayDirection = CameraBrain.transform.forward;
            RaycastHit hit;
            // The ragdoll must ignore ragdolls
            LayerMask mask = ~(1 << LayerMask.NameToLayer("Ragdoll"));
            if (Physics.Raycast(startPosition, rayDirection, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                return hit.point;
            else
                return Vector3.zero;
        }
    }
    
    public static Vector3 GetCameraAngle() => CameraBrain.transform.eulerAngles;

    /// <returns>The active camera's forward direction.</returns>
    public static Vector3 GetCameraDirection() => CameraBrain.transform.forward;

    public static Vector3 GetCameraTransformedDirection(Vector3 direction) => CameraBrain.transform.TransformDirection(direction);
    public static Vector3 GetCameraTransformedDirection(float x, float y, float z) => CameraBrain.transform.TransformDirection(x, y, z);
    public static Transform GetCameraTransform() => CameraBrain.transform;


    public Vector3 ThirdPersonForward() => thirdPersonCamera.transform.forward;
    Vector2 CameraMovement => Ragdoll.InputHandler.cameraAxis;

    public void DisableAllCameras()
    {
        thirdPersonCamera?.gameObject.SetActive(false);
        firstPersonCamera?.gameObject.SetActive(false);
        aimCamera?.gameObject.SetActive(false);
        deathCamera?.gameObject.SetActive(false);
    }

    public void EnableThirdPersonCamera() 
    {
        DisableAllCameras();
        thirdPersonCamera.gameObject.SetActive(true);
    }

    public void EnableFirstPersonCamera()
    {
        DisableAllCameras();
        firstPersonCamera.gameObject.SetActive(true);
    }

    public void EnableAimCamera() 
    {
        DisableAllCameras();
        aimCamera.gameObject.SetActive(true);
    }

    public void EnableDeathCamera() 
    {
        DisableAllCameras();
        deathCamera.gameObject.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;
        Ragdoll = GetComponent<RagdollController>();
    }


    private void OnEnable() => Ragdoll.OnReady += Initialize;

    private void Initialize(object sender, EventArgs e)
    {
        Ragdoll.OnReady -= Initialize;
        CameraBrain = FindObjectOfType<CinemachineBrain>();
        _cameraRotationX = transform.eulerAngles.y;

        if(!CameraBrain) Debug.LogError("Hey Gann, i'm unable to find cinemachine brain!");
    }

    private void Update()
    {
        ThirdPersonCam();

        if(Ragdoll.HealthController.IsDead && !deathCamera.isActiveAndEnabled) EnableDeathCamera();
    }

    private void ThirdPersonCam()
    {
        // Sets the camera position based on the stablished offset
        thirdPersonCamera.transform.position = Vector3.Lerp(
            thirdPersonCamera.transform.position,
            transform.position + thirdPersonCamera.transform.TransformDirection(offset),
            positionLerp);

        // Sets the camera rotation (euler angles) based on the modified Vector3 variable
        // thirdPersonCamera.transform.eulerAngles = Vector3.Lerp(
        //     thirdPersonCamera.transform.eulerAngles,
        //     rotation,
        //     rotationLerp);

        // rotation = new Vector3(
        //     thirdPersonCamera.transform.eulerAngles.x - CameraMovement().y * sensitivity.y,
        //     thirdPersonCamera.transform.eulerAngles.y + CameraMovement().x * sensitivity.x,
        //     0);

        _cameraRotationY += CameraMovement.x;
        _cameraRotationX -= CameraMovement.y;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -89, 89);

        Vector3 finalCameraRotation = Vector3.Lerp(
            thirdPersonCamera.transform.eulerAngles,
            new Vector3(_cameraRotationX, _cameraRotationY, 0),
            rotationLerp
        );

        // Apply camera rotations
        thirdPersonCamera.transform.eulerAngles = finalCameraRotation;
        if(firstPersonCamera && firstPersonCamera.isActiveAndEnabled) firstPersonCamera.transform.eulerAngles = finalCameraRotation;

        // Swap shoulder perspective
        if (PlayerInputHandler.instance.cameraSwitch) offset = new Vector3(-offset.x, offset.y, offset.z);
    }
}
