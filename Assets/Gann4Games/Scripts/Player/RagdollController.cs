using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Gann4Games.Thirdym.StateMachines;
using DG.Tweening;

public class RagdollController : StateMachine {

    public CheckGround enviroment;
	public Vector2 MovementAxis => PlayerInputHandler.instance.movementAxis;

	[Header("Ragdoll Components")]
	[HideInInspector] public Transform guide;
	public Transform body;
	public Transform head;
	float AngleX
	{
		get
		{
			float ang = body.localEulerAngles.x;
			ang = (ang > 180) ? ang - 360 : ang;
			return ang;
		}
	}
	float _bodyRotation;
	float _bodySpring;
	float _bodyDamp = 10;
	public Rigidbody[] bodyParts;
	[FormerlySerializedAs("LegsMotion")] [SerializeField] List<HingeJoint> legsMotion;
	public HingeJoint RootJoint { get; private set; }

	public CharacterCustomization Character { get; private set; }
	public Rigidbody HeadRigidbody { get; private set;  }
	public Rigidbody BodyRigidbody { get; private set; }
	
	public float LimbsJointWeight { get; private set; }

	// Player states
	public PlayerGroundedState GroundedState = new PlayerGroundedState();
	public PlayerUnderwaterState UnderwaterState = new PlayerUnderwaterState();
	public PlayerJumpingState JumpingState = new PlayerJumpingState();
	public PlayerInjuredState InjuredState = new PlayerInjuredState();
	public PlayerDeadState DeadState = new PlayerDeadState();

	void Awake () {
        SetLimbsWeight(1, 0);
		Character = GetComponent<CharacterCustomization>();
		bodyParts = GetComponentsInChildren<Rigidbody>();
		HeadRigidbody = head.GetComponent<Rigidbody>();
		BodyRigidbody = body.GetComponent<Rigidbody>();

		guide = new GameObject("guide").transform;
		guide.rotation = body.rotation;
        
		RootJoint = GetComponent<HingeJoint>();
		RootJoint.autoConfigureConnectedAnchor = false;
        
		SetState(GroundedState);
	}
	void Update ()
	{
		UpdateRootJointStatus();
		if(Character.isPlayer) CurrentState.OnUpdateState(this);
	}

	public float RelativeZVelocity => body.InverseTransformDirection(BodyRigidbody.velocity).z;
	public void JumpAsPlayer()
	{
		Vector3 direction = PlayerCameraController.GetCameraTransformedDirection(MovementAxis.x, 0, MovementAxis.y);
        JumpTowards(direction);
    }

	public void JumpTowards(Vector3 direction)
	{
        direction += Vector3.up;
        float jumpForce = 5;
		SetCharacterVelocity(direction * jumpForce);
	}
	
	/// <summary>
	/// Sets the velocity of every rigidbody in the ragdoll.
	/// </summary>
	void SetCharacterVelocity(Vector3 direction) 
	{
		foreach(Rigidbody part in bodyParts)
		{
			part.velocity = direction;
		}
	}

	/// <summary>
	/// Sets the weight of every limb (HingeJoint) over time.
	/// The function does not modify joints, joints are following a value stored in this class.
	/// </summary>
	/// <param name="weight">The weight that joints are goint to be multiplied by</param>
	/// <param name="time">The time it will take to make this transition (in seconds)</param>
	public void SetLimbsWeight(float weight, float time)
	{
		DOTween.To(() => LimbsJointWeight, x => LimbsJointWeight = x, weight, time);
	}
    
// Extracted functionality from spaghetti code goes below
// Guide transform

	public void MakeGuideSetRotation(Quaternion rotation, float lerpTimeClamped)
	{
        guide.rotation = Quaternion.Lerp(guide.rotation, rotation, lerpTimeClamped);
    }
	public void MakeGuideLookTowards(Vector3 point, float lerpTimeClamped)
	{
        point.y = RootJoint.transform.position.y;
        Quaternion desiredRotation = Quaternion.LookRotation(point - RootJoint.transform.position);
        guide.rotation = Quaternion.Lerp(guide.rotation, desiredRotation, lerpTimeClamped);
    }
	public void MakeGuideLookTowardsCamera(float lerpTimeClamped = 1)
	{
		guide.rotation = Quaternion.Slerp(guide.rotation,
			Quaternion.Euler(0, PlayerCameraController.GetCameraAngle().y, 0),
			lerpTimeClamped);
	}

	public void MakeGuideLookTowardsMovement()
	{
		Vector3 direction = PlayerCameraController.GetCameraTransformedDirection(new Vector3(MovementAxis.x, 0, MovementAxis.y));
		direction.y = 0;
		guide.forward = Vector3.Lerp(guide.forward, guide.position + direction.normalized, Time.deltaTime * 10);
	}
    
// Animation
	// States
	public void SetCrouchAnimationState(bool value) => Character.Animator.SetBool("Crouch", value);
	public void SetGroundedAnimationState(bool value) => Character.Animator.SetBool("Grounded", value);
	public void SetKickingAnimationState(bool value) => Character.Animator.SetBool("Kicking", value);
	// Values
	public void SetHorizontalAnimationValue(float value) => Character.Animator.SetFloat("X", value);
	public void SetVerticalAnimationValue(float value) => Character.Animator.SetFloat("Y", value);
	public float GetHorizontalAnimationValue() => Character.Animator.GetFloat("X");
	public float GetVerticalAnimationValue() => Character.Animator.GetFloat("Y");
	public bool IsMoving() => MovementAxis != Vector2.zero;
// Root joint
	public void UpdateRootJointStatus()
	{
		JointSpring hingeSpring = RootJoint.spring;
		hingeSpring.spring = _bodySpring;
		hingeSpring.damper = _bodyDamp;
		hingeSpring.targetPosition = _bodyRotation;
		RootJoint.spring = hingeSpring;
		RootJoint.useSpring = Character.HealthController.IsAlive;
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
	public void MakeRootFollowGuide() => RootJoint.transform.rotation = guide.rotation;
}