using UnityEngine;
using System;
using Gann4Games.Thirdym.Core;
using Gann4Games.Thirdym.Events;
using Gann4Games.Thirdym.Utility;

[RequireComponent(typeof(CollisionEvents))]
public class CharacterBodypart : MonoBehaviour, IDamageable
{
	public RagdollController Ragdoll { get; private set; }

	public float damageMultiplier;

	[Header("Optional sound effect")]
	[Tooltip("Replaces the private parameters from the character preset.")]
	[SerializeField] AudioClip sfxCollideHard, sfxCollideMedium, sfxCollideSoft;

	AudioClip _sfxCollideHard, _sfxCollideMedium, _sfxCollideSoft;
	CharacterHealthSystem _healthController;
	CollisionEvents _colliderEvents;

	private HingeJoint _joint;
	private float _startJointSpring;
    
	public void DealDamage(float damage, DamageType damageType, Vector3 where)
	{
		_healthController.DealDamage(damage, where, !Ragdoll.Customizator.isNPC);

		switch (damageType)
		{
			case DamageType.Blade:
			{
				GameObject bloodfx = Ragdoll.Customizator.preset.BloodSplatFX();
				bloodfx.transform.position = transform.position;
				break;
			}

			case DamageType.Bullet:
			{
				GameObject bloodfx = Ragdoll.Customizator.preset.BloodSplatFX();
				bloodfx.transform.position = transform.position;
				bloodfx.transform.rotation = transform.rotation;
				break;
			}

			case DamageType.Collision:
				if(Ragdoll.Customizator.isPlayer)
					MainHUDHandler.instance.ShowDamageEffect(Color.white);
				break;
		}
	}
	private void Awake()
	{
		_colliderEvents = GetComponent<CollisionEvents>();
		Ragdoll = GetComponentInParent<RagdollController>();
	}

	private void OnEnable()
	{
        Ragdoll.OnReady += Initialize;
        _colliderEvents.OnCollideHard += CollideHard;
		_colliderEvents.OnCollideMedium += CollideMedium;
		_colliderEvents.OnCollideSoft += CollideSoft;
	}


    private void OnDisable()
	{
		_colliderEvents.OnCollideHard -= CollideHard;
		_colliderEvents.OnCollideMedium -= CollideMedium;
		_colliderEvents.OnCollideSoft -= CollideSoft;
	}

    private void Initialize(object sender, EventArgs e)
    {
		Ragdoll.OnReady -= Initialize;

        if (TryGetComponent(out HingeJoint joint))
		{
			_joint = joint;
			_startJointSpring = _joint.spring.spring;
		}
		_healthController = Ragdoll.HealthController;

		// If optional SFX parameters are set, don't use the ones stored in the character preset.
		#region Applying sound effects
		if (sfxCollideSoft) _sfxCollideSoft = sfxCollideSoft;
		else _sfxCollideSoft = Ragdoll.Customizator.preset.sfxCollideSoft;

		if (sfxCollideMedium) _sfxCollideMedium = sfxCollideMedium;
		else _sfxCollideMedium = Ragdoll.Customizator.preset.sfxCollideMedium;

		if (sfxCollideHard) _sfxCollideHard = sfxCollideHard;
		else _sfxCollideHard = Ragdoll.Customizator.preset.sfxCollideHard;
		#endregion
        
		InvokeRepeating(nameof(UpdateJointWeight), 0, 0.5f);
    }

	private void UpdateJointWeight()
	{
		if (_joint && _joint.spring.spring != Ragdoll.LimbsJointWeight)
			_joint.spring = PhysicsTools.SetHingeJointSpring(_joint.spring, Ragdoll.LimbsJointWeight * _startJointSpring);
	}

	void CollideHard(object sender, CollisionEvents.CollisionArgs args)
	{
		float damage = args.collisionMagnitude * damageMultiplier;
		Ragdoll.AudioPlayer.PlayOneShot(_sfxCollideHard);

		DealDamage(damage, DamageType.Collision, Vector3.zero);
	}
	
	void CollideMedium(object sender, EventArgs args) => Ragdoll.AudioPlayer.PlayOneShot(_sfxCollideMedium);
	void CollideSoft(object sender, EventArgs args) => Ragdoll.AudioPlayer.PlayOneShot(_sfxCollideSoft);
}