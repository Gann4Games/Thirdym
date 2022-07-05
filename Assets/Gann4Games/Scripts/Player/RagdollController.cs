using System;
using UnityEngine;
using Gann4Games.Thirdym.StateMachines;
using Gann4Games.Thirdym.NPC;
using DG.Tweening;
using Gann4Games.Thirdym.Utility;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class RagdollController : StateMachine {

    public event EventHandler OnReady;
    public CheckGround enviroment;
	public Vector2 MovementAxis => PlayerInputHandler.instance.movementAxis;

    [Header("Settings")]
	[Tooltip("The time it takes to update all joints weight (in seconds)")]
    [SerializeField] private float jointWeightUpdateRate = 1;

    [Header("Ragdoll Components")]
    [SerializeField] private Animator animator;

    public Rigidbody[] bodyParts;
	
	public HingeJoint RootJoint { get; private set; }

	public Rigidbody HeadRigidbody { get; private set;  }
	public Rigidbody BodyRigidbody { get; private set; }
	
	public float LimbsJointWeight { get; private set; }

/*RAGDOLL CORE COMPONENTS*/
	public CharacterCustomization Customizator { get; private set; }
	public AudioSource AudioPlayer { get; private set; }
    public CharacterHealthSystem HealthController { get; private set; }
    public ShootSystem ShootSystem { get; private set; }
    public EquipmentSystem EquipmentController { get; private set; }
    public CharacterArms ArmController { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public NpcRagdollController Npc { get; private set; }
    public CharacterWalljump WalljumpController { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public CharacterMeleeHandler MeleeHandler { get; private set; }
    public CharacterInteractor Interactor { get; private set; }
    public Animator Animator => animator;

    // Player states
    public PlayerGroundedState GroundedState = new PlayerGroundedState();
	public PlayerUnderwaterState UnderwaterState = new PlayerUnderwaterState();
	public PlayerJumpingState JumpingState = new PlayerJumpingState();
	public PlayerInjuredState InjuredState = new PlayerInjuredState();
	public PlayerDeadState DeadState = new PlayerDeadState();

    private HingeJointTarget[] _ragdollJoints;

    private float _bodyRotation;
	private float _bodySpring;
	private float _bodyDamp = 10;
	private Transform _guide;

    private Tween _limbsTweener;

    public float RelativeZVelocity => BodyRigidbody.transform.InverseTransformDirection(BodyRigidbody.velocity).z;

	void Awake () {
        SetLimbsWeight(1, 0);

        GetCoreComponents();
        GetBodyparts();
        ConfigureComponents();

        SetState(GroundedState);
	}

	private void Start() => OnReady?.Invoke(this, EventArgs.Empty);

	private void GetCoreComponents()
	{
		RootJoint = GetComponent<HingeJoint>();

		AudioPlayer = GetComponent<AudioSource>();
		HealthController = GetComponent<CharacterHealthSystem>();
		ShootSystem = GetComponent<ShootSystem>();
		EquipmentController = GetComponent<EquipmentSystem>();
		ArmController = GetComponent<CharacterArms>();
		CameraController = GetComponent<PlayerCameraController>();
		Npc = GetComponent<NpcRagdollController>();
		WalljumpController = GetComponent<CharacterWalljump>();
		InputHandler = GetComponent<PlayerInputHandler>();
		MeleeHandler = GetComponent<CharacterMeleeHandler>();
		Interactor = GetComponent<CharacterInteractor>();
		Customizator = GetComponent<CharacterCustomization>();
	}

	private void GetBodyparts()
	{
		bodyParts = GetComponentsInChildren<Rigidbody>();
		HeadRigidbody = Customizator.baseBody.head.GetComponent<Rigidbody>();
		BodyRigidbody = Customizator.baseBody.body.GetComponent<Rigidbody>();

        _ragdollJoints = GetComponentsInChildren<HingeJointTarget>();
    }

	private void ConfigureComponents()
	{
		RootJoint.autoConfigureConnectedAnchor = false;
		_guide = new GameObject("guide").transform;

        StartCoroutine(UpdateJointsWeightCoroutine());
    }

	private void Update ()
	{
		UpdateRootJointStatus();
		if(Customizator.isPlayer) CurrentState.OnUpdateState(this);
	}
	public void JumpAsPlayer()
	{
		Vector3 direction = PlayerCameraController.GetCameraTransformedDirection(MovementAxis.x, 0, MovementAxis.y);
        JumpTowards(direction);
    }

	public void JumpTowards(Vector3 direction)
	{
        direction.Normalize();
        direction.y = 1;
		
        float jumpForce = 5;
		SetCharacterVelocity(direction * jumpForce);
	}
	
	/// <summary>
	/// Sets the velocity of every rigidbody in the ragdoll.
	/// </summary>
	private void SetCharacterVelocity(Vector3 direction) 
	{
		foreach(Rigidbody part in bodyParts)
		{
			part.velocity = direction;
		}
	}

	private IEnumerator UpdateJointsWeightCoroutine()
	{
        yield return new WaitForSeconds(jointWeightUpdateRate);
        foreach(HingeJointTarget joint in _ragdollJoints) joint.SetJointWeight(LimbsJointWeight);
        StartCoroutine(UpdateJointsWeightCoroutine());
    }

	/// <summary>
	/// Sets the weight of every limb (HingeJoint) over time.
	/// The function does not modify joints, joints are following a value stored in this class.
	/// </summary>
	/// <param name="weight">The weight that joints are goint to be multiplied by</param>
	/// <param name="time">The time it will take to make this transition (in seconds)</param>
	public void SetLimbsWeight(float weight, float time)
	{
        _limbsTweener?.Kill();
        _limbsTweener = DOTween.To(() => LimbsJointWeight, x => LimbsJointWeight = x, weight, time);
        _limbsTweener.Play();
    }
    
// Extracted functionality from spaghetti code goes below
// Guide transform

	public void MakeGuideSetRotation(Quaternion rotation, float lerpTimeClamped)
	{
        _guide.rotation = Quaternion.Lerp(_guide.rotation, rotation, lerpTimeClamped);
    }
	public void MakeGuideLookTowards(Vector3 point, float lerpTimeClamped)
	{
        point.y = RootJoint.transform.position.y;
        Quaternion desiredRotation = Quaternion.LookRotation(point - RootJoint.transform.position);
        _guide.rotation = Quaternion.Lerp(_guide.rotation, desiredRotation, lerpTimeClamped);
    }
	public void MakeGuideLookTowardsCamera(float lerpTimeClamped = 1)
	{
		_guide.rotation = Quaternion.Slerp(_guide.rotation,
			Quaternion.Euler(0, PlayerCameraController.GetCameraAngle().y, 0),
			lerpTimeClamped);
	}

	public void MakeGuideLookTowardsMovement()
	{
		Vector3 direction = PlayerCameraController.GetCameraTransformedDirection(new Vector3(MovementAxis.x, 0, MovementAxis.y));
		direction.y = 0;
		_guide.forward = Vector3.Lerp(_guide.forward, _guide.position + direction.normalized, Time.deltaTime * 10);
	}

    /*ANIMATION*/
    // States
    public void SetWeaponAnimationActionState(bool value) => Animator.SetBool("WeaponAction", value);
	public void SetWeaponAnimationAimState(bool value) => Animator.SetBool("WeaponAiming", value);
    public void SetCrouchAnimationState(bool value) => Animator.SetBool("Crouch", value);
	public void SetGroundedAnimationState(bool value) => Animator.SetBool("Grounded", value);
	public void SetKickingAnimationState(bool value) => Animator.SetBool("Kicking", value);
    public bool GetReloadAnimationState() => Animator.GetBool("WeaponReload");
    // Values
    public void SetHorizontalAnimationValue(float value) => Animator.SetFloat("X", value);
	public void SetVerticalAnimationValue(float value) => Animator.SetFloat("Y", value);
	public float GetHorizontalAnimationValue() => Animator.GetFloat("X");
	public float GetVerticalAnimationValue() => Animator.GetFloat("Y");
	public bool IsMoving() => MovementAxis != Vector2.zero;

/*ROOT JOINT*/
	private void UpdateRootJointStatus()
	{
		JointSpring hingeSpring = RootJoint.spring;
		hingeSpring.spring = _bodySpring;
		hingeSpring.damper = _bodyDamp;
		hingeSpring.targetPosition = _bodyRotation;
		RootJoint.spring = hingeSpring;
		RootJoint.useSpring = HealthController.IsAlive;
	}

	/// <summary>
    /// Note: a negative value will tilt the body forward, and a positive value will do the opposite (duh)
    /// </summary>
    /// <param name="angle"></param>
	public void SetRootJointRotation(float angle) => _bodyRotation = angle;
	public void SetRootJointSpring(float spring) => _bodySpring = spring;
	public void SetRootJointDamping(float damping) => _bodyDamp = damping;

	/// <summary>
    /// If this function is not running, the ragdoll will never rotate.
    /// Its like an attachment that allows for smooth physics rotation.
    /// Its open to changes so it doesn't rely on an external transform...
    /// </summary>
	public void MakeRootFollowGuide() => RootJoint.transform.rotation = _guide.rotation;

/*AUDIO / SOUNDS*/
	public void PlaySFX(AudioClip sfx) => AudioPlayer.PlayOneShot(sfx);
    public void PlayFireSFX() => AudioPlayer.PlayOneShot(EquipmentController.currentWeapon.GetFireSFX());
    public void PlayEnemyDownSFX() => PlaySFX(AudioTools.GetRandomClip(Customizator.preset.enemyDownSFX));
    public void PlayAlertSFX() => PlaySFX(AudioTools.GetRandomClip(Customizator.preset.alertSFX));
    public void PlayPainSFX() => PlaySFX(AudioTools.GetRandomClip(Customizator.preset.painSFX));
    public void PlayInjurySFX() => PlaySFX(AudioTools.GetRandomClip(Customizator.preset.injuryStateSFX));
    public void PlayDeathSFX()
    {
        PlaySFX(Customizator.preset.forcedDeathSFX);
        PlaySFX(AudioTools.GetRandomClip(Customizator.preset.deathSFX));
    }
}