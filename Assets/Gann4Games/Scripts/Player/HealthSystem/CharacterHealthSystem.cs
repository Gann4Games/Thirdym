using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gann4Games.Thirdym.Utility;

public class CharacterHealthSystem : MonoBehaviour {
    public enum HealthStatus
    {
        Alive, Injuried, Unconcious, Dead
    }
    [HideInInspector] public HealthStatus playerLifeState;
    public List<CharacterBodypart> bodyParts;
    public bool HeavyHit;
    public bool Unconcious;
    public bool Dead;

    public EventHandler<OnDamageDealedArgs> OnDamageDealed;
    public class OnDamageDealedArgs
    {
        public Vector3 where;
    }

    CharacterCustomization _character;

    [SerializeField] UnityEvent OnInjury;
    [SerializeField] UnityEvent OnDeath;
    [SerializeField] UnityEvent OnResurrect;

    TimerTool _timer = new TimerTool();

    float _health;
    float _maxHealth;
    float _injuryLevel;

    public float MaximumHealth { get => _maxHealth; }
    public float CurrentHealth { get => _health; }
    public float InjuryLevel { get => _injuryLevel; }

    private void Awake()
    {
        _character = GetComponent<CharacterCustomization>();
        _timer.SetTimeOut(3);
        bodyParts.AddRange(GetComponentsInChildren<CharacterBodypart>());
        /*foreach (PartCollision _partCollision in bodyParts)
            _partCollision.customizator.healthController = this;*/
    }
    private void Start()
    {
        _maxHealth = _character.preset.maximumHealth;
        _injuryLevel = _character.preset.injuryLevel;

        if (Dead) _health = 0;
        else _health = _maxHealth;

        if (!_character.isNPC) MainHUDHandler.instance.healthbar.maxValue = _maxHealth;
    }

    /// <summary>
    /// Returns true if the health is higher than required. This can be a value over-increasing by self healing, defibrilators, or hacking, who knows.
    /// </summary>
    bool IsOverHealed => _health >= _maxHealth;

    /// <summary>
    /// Returns true if the health is higher or equals to the injury level configured.
    /// </summary>
    public bool IsFullyAlive => _health >= _injuryLevel;

    /// <summary>
    /// Returns true if the health is lower or equals than the injury level configured.
    /// </summary>
    public bool IsInjuried => _health <= _injuryLevel;

    /// <summary>
    /// Returns true if the health is less or equals to zero.
    /// </summary>
    public bool IsDead => _health <= 0;
    
    private void Update()
    {
        // Health system rewrite (newest)
        
        //switch (playerLifeState)
        //{
        //    case HealthStatus.Alive:
        //        break;
        //    case HealthStatus.Injuried:
        //        break;
        //    case HealthStatus.Dead:
        //        break;
        //}


        _character.RagdollController.isRagdollState = !IsFullyAlive;
        AnimateArms(!IsDead);
        if (!_character.isNPC)
        {
            MainHUDHandler.instance.mainAlpha = 1-(_health / _maxHealth);
            MainHUDHandler.instance.healthbar.value = _health;
        }
        if (IsFullyAlive && !IsOverHealed)
        {// Health regeneration if not injuried or dead state
            if (_timer.IsTimeOut())
                _health += Time.deltaTime * ((_health/_maxHealth)* _character.preset.regeneration_rate);
            else
                _timer.CountTime();
        }
        if (IsDead)
        { 
            _health = 0;        // Limit health
            if (!_character.isNPC) IngameMenuHandler.PauseAndShowMessage("You have died!");        // Show death screen
        }
        if (IsOverHealed) // Avoid health being higher than the max value set
            _health = _maxHealth;
        if (IsInjuried)
        {
            _health -= 1 * Time.deltaTime;
            if (!_character.isNPC)
                _character.RagdollController.isRagdollState = true;
            if (!Unconcious)
            {
                _character.SoundSource.PlayOneShot(AudioTools.GetRandomClip(_character.preset.injuryStateSFX));
                OnInjury.Invoke();
            }
            Unconcious = true;
            if (IsDead)
            {
                if (!_character.isNPC)
                {
                    _character.GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                    _character.GetComponent<BoxCollider>().enabled = false;
                    BackgroundMusic.EnableMusic(false);
                }
                if (Dead == false)
                {
                    Dead = true;
                    _character.PlayDeathSFX();
                    _character.EquipmentController.DropAllWeapons();
                    OnDeath.Invoke();
                }
            }
            else
            {
                if (Dead)
                    OnResurrect.Invoke();
                Dead = false;
                _character.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }
    void AnimateArms(bool animate)
    {
        //HingeJoint[] hj = GetComponentsInChildren<HingeJoint>();            //Requires optimization
        HingeJoint[] hj = _character.ArmController.GetUpperBodyParts().ToArray();
        foreach (HingeJoint hinge in hj)
        {
            if (hinge == null) continue;
            float smoothVal = Mathf.Lerp(hinge.spring.spring, (animate)?500:0, Time.deltaTime);
            hinge.spring = PhysicsTools.SetHingeJointSpring(hinge.spring, smoothVal);
        }
    }
    public void PlayPainSound()
    {
        if (Dead)
            return;
        _timer.ResetTime();
        int randomChance = UnityEngine.Random.Range(0, 4);
        if (randomChance == 0) _character.PlayPainSFX();
    }
    public void DealDamage(float amount, Vector3 damageSource, bool showScreenBlood = true)
    {
        _health -= amount;
        PlayPainSound();

        if (damageSource != Vector3.zero) OnDamageDealed.Invoke(this, new OnDamageDealedArgs { where = damageSource });

        if (!_character.isNPC && showScreenBlood)
            MainHUDHandler.instance.ShowEffect(Color.red, amount/_maxHealth, 10);
    }
}
