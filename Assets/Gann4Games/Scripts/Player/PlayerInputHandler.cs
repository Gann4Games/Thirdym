using UnityEngine;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    public event EventHandler OnReady;
    public event EventHandler OnStop;

    public static PlayerInputHandler instance;
    public GameplayInput gameplayControls;

    [SerializeField] Vector2 cameraSensitivity = Vector2.one*4;

    /// <summary>
    /// Returns a simplified version of an input action's string.
    /// Example:
    /// Input: "Player/Use[/Keyboard/LeftArrow]"
    /// Returns: "LeftArrow"
    /// </summary>
    /// <param name="action">The input action to simplify.</param>
    /// <returns>The key used to perfom the action as string.</returns>
    public static string InputAsString(InputAction action) => Regex.Match(action.ToString(), @"(?<=\/)(\w+)(?=\])").Value;

    public Vector2 cameraAxis => gameplayControls.Player.Camera.ReadValue<Vector2>() * cameraSensitivity;
    public Vector2 movementAxis => gameplayControls.Player.Movement.ReadValue<Vector2>();

    public bool aiming => gameplayControls.Player.Aim.ReadValue<float>() > 0;
    public bool firing => gameplayControls.Player.Fire.ReadValue<float>() > 0;
    public bool ability => gameplayControls.Player.Ability.triggered;
    public bool use => gameplayControls.Player.Use.triggered;
    public bool walking => gameplayControls.Player.Walk.ReadValue<float>()>0;
    public bool jumping => gameplayControls.Player.Jump.ReadValue<float>()>0;
    public bool ragdolling => gameplayControls.Player.Ragdoll.ReadValue<float>()>0;
    public bool crouching => gameplayControls.Player.Crouch.ReadValue<float>()>0;
    public bool kicking => gameplayControls.Player.Kick.ReadValue<float>()>0;
    public bool cameraSwitch => gameplayControls.Player.CameraSwitch.triggered;
    public bool pause => gameplayControls.Player.Pause.triggered;
    public bool dropWeapon => gameplayControls.Player.DropItem.triggered;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        gameplayControls = new GameplayInput();
        gameplayControls.Enable();
        OnReady?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable()
    {
        gameplayControls.Disable();
        OnStop?.Invoke(this, EventArgs.Empty);
    }
}
