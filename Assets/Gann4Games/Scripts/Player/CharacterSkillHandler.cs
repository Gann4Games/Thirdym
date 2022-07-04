using UnityEngine;
using Gann4Games.Thirdym.Abilities;
using UnityEngine.InputSystem;
using System;

public class CharacterSkillHandler : MonoBehaviour {

    public RagdollController Ragdoll { get; private set; }
    public bool IsAbilityRunning { get; private set; }
    public bool IsEnergyOut => _energy <= 0;
    public bool IsEnergyFilled => _energy >= 100;

    private float _energy = 100;
    private Ability ability => Ragdoll.Customizator.preset.ability;

    public void Consume(float amount) => _energy -= amount * Time.deltaTime;
    public void Regenerate(float amount)
    {
        if(IsEnergyFilled) return;
        _energy += amount * Time.deltaTime;
    }

    private void Awake()
    {
        Ragdoll = GetComponent<RagdollController>();
    }

    private void OnEnable() 
    {
        Ragdoll.InputHandler.OnReady += OnInputReady;
        Ragdoll.InputHandler.OnStop += OnInputStop;
    }

    private void OnDisable()
    {
        Ragdoll.InputHandler.OnReady -= OnInputReady;
        Ragdoll.InputHandler.OnStop -= OnInputStop;
    }

    private void OnInputReady(object sender, EventArgs e) => Ragdoll.InputHandler.gameplayControls.Player.Ability.performed += OnAbilityKeyPressed;

    private void OnInputStop(object sender, EventArgs e) => Ragdoll.InputHandler.gameplayControls.Player.Ability.performed -= OnAbilityKeyPressed;

    private void OnAbilityKeyPressed(InputAction.CallbackContext obj)
    {
        IsAbilityRunning = !IsAbilityRunning;

        if(IsAbilityRunning) ability?.OnStartAbility(this);
        else ability?.OnEndAbility(this);
    }

    private void Update()
    {
        ability?.OnUpdateAbility(this);
        if(IsEnergyOut && IsAbilityRunning) StopAbilityUsage();

        // Display energy value on screen
        if(MainHUDHandler.instance.energybar.value != _energy) MainHUDHandler.instance.energybar.value = _energy;
    }

    private void StopAbilityUsage()
    {
        IsAbilityRunning = false;
        ability?.OnEndAbility(this);
    } 
}
