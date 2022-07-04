using System;
using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.StateMachines;
using DG.Tweening;

public class CharacterHealthSystem : StateMachine
{
    public List<CharacterBodypart> bodyParts;

    public event EventHandler<OnDamageDealedArgs> OnDamageDealed;
    public class OnDamageDealedArgs
    {
        public Vector3 where;
    }

    public RagdollController Ragdoll { get; private set; }

    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public float InjuryLevel { get; private set; }
    public float HealthPercentage => Health / MaxHealth;
    
    public HealthAliveState AliveState = new HealthAliveState();
    public HealthInjuredState InjuredState = new HealthInjuredState();
    public HealthDeadState DeadState = new HealthDeadState();
    
    public bool IsAlive => Health > InjuryLevel;
    public bool IsInjured => Health < InjuryLevel;
    public bool IsDead => Health < 0;
    public bool MaxHealthReached => Health >= MaxHealth;

    private void Awake()
    {
        Ragdoll = GetComponent<RagdollController>();
        Ragdoll.OnReady += Initialize;
        bodyParts.AddRange(GetComponentsInChildren<CharacterBodypart>());
    }

    private void Initialize(object sender, System.EventArgs args)
    {
        Ragdoll.OnReady -= Initialize;

        MaxHealth = Ragdoll.Customizator.preset.maximumHealth;
        InjuryLevel = Ragdoll.Customizator.preset.injuryLevel;
        Health = MaxHealth;

        if (!Ragdoll.Customizator.isNPC) MainHUDHandler.instance.healthbar.maxValue = MaxHealth;
        UpdateHealthbarUI(1);

        SetState(AliveState);
    }

    private void Update()
    {
        CurrentState?.OnUpdateState(this);
    }

    public void PlayPainSound()
    {
        if (IsDead) return;
        int randomChance = UnityEngine.Random.Range(0, 4);
        if (randomChance == 0) Ragdoll.PlayPainSFX();
    }

    public void DealDamage(float amount, Vector3 damageSource, bool showScreenBlood = true)
    {
        Health -= amount;
        // MainHUDHandler.instance.healthbar.value = Health;
        PlayPainSound();

        //if (damageSource != Vector3.zero) OnDamageDealed?.Invoke(this, new OnDamageDealedArgs { where = damageSource });
        OnDamageDealed?.Invoke(this, new OnDamageDealedArgs { where = damageSource });

        if (Ragdoll.Customizator.isPlayer) MainHUDHandler.instance.ShowDamageEffect(Color.red);
        UpdateHealthbarUI(1);
    }

    public void AddHealth(float amount)
    {
        Health += amount;
        UpdateHealthbarUI(0);
    }

    private void UpdateHealthbarUI(float transitionTime)
    {
        if(!Ragdoll.Customizator.isPlayer) return;
        MainHUDHandler.SetHealthValue(Health, transitionTime);
    }
}
