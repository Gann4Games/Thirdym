using UnityEngine;
using Gann4Games.Thirdym.Abilities;
using UnityEngine.InputSystem;
using System;

public class CharacterSkillHandler : MonoBehaviour {

    public CharacterCustomization Character { get; private set; }
    public bool IsAbilityRunning { get; private set; }
    public bool IsEnergyOut => _energy <= 0;
    public bool IsEnergyFilled => _energy >= 100;

    private float _energy = 100;
    private Ability ability => Character.preset.ability;

    public void Consume(float amount) => _energy -= amount * Time.deltaTime;
    public void Regenerate(float amount) => _energy += amount * Time.deltaTime;

    private void Awake()
    {
        Character = GetComponent<CharacterCustomization>();
    }

    private void OnEnable() 
    {
        Character.InputHandler.OnReady += OnInputReady;
        Character.InputHandler.OnStop += OnInputStop;
    }

    private void OnDisable()
    {
        Character.InputHandler.OnReady -= OnInputReady;
        Character.InputHandler.OnStop -= OnInputStop;
    }

    private void OnInputReady(object sender, EventArgs e) => Character.InputHandler.gameplayControls.Player.Ability.performed += OnAbilityKeyPressed;

    private void OnInputStop(object sender, EventArgs e) => Character.InputHandler.gameplayControls.Player.Ability.performed -= OnAbilityKeyPressed;

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
