﻿using UnityEngine;
using UnityEngine.Audio;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.Enums;

public class CharacterSkillHandler : MonoBehaviour {
	
    [Header("Main configuration")]
    [SerializeField] float energy = 100;
    [Tooltip("The amount of time it takes to start reloading in seconds.")]
    [SerializeField] float recoverDelay = 3;
    [Tooltip("The amount of points that will be consumed per second.")]
    [SerializeField] float consumePerSecond = 5;
    [Tooltip("The amount of points what will be restored per second.\nRecommended to be less than consumePerSecond.")]
    [SerializeField] float regeneratePerSecond = 2.5f;

    [Space]
    [Header("Slow Motion configuration.")]
    [SerializeField] AudioMixer soundMaster;
    [Tooltip("Unity's time scale. Recommended to set between 0.3 and 0.5 for slow-motion.")]
    [SerializeField] float slowmotionTimeScale = 0.3f;
    float _startFixedDeltaTime;

    [Header("Sprint configuration.")]
    Animator _animator;


    CharacterCustomization _customizator;
    readonly TimerTool _timer = new TimerTool();

    CharacterSkills _choosenSkill;

    bool _skillEnable;
    bool IsOutOfEnergy => energy <= 0;
    bool IsFullOfEnergy => energy >= 100;

    float SoundMasterPitch
    {
        get
        {
            soundMaster.GetFloat("PitchMaster", out float val);
            return val;
        }
    }

    private void Awake()
    {
        _customizator = GetComponent<CharacterCustomization>();
        _timer.SetTimeOut(recoverDelay);
        _startFixedDeltaTime = Time.fixedDeltaTime;

        _animator = _customizator.Animator;

        _choosenSkill = _customizator.preset.skill;
    }
    private void Update()
    {
        if (InputHandler.instance.ability) UseSkill();

        switch (_choosenSkill)
        {
            case CharacterSkills.None:
                energy = 0;
                Destroy(this);
                break;

            case CharacterSkills.Slowmotion:
                SlowMotion();
                break;

            case CharacterSkills.Sprint:
                Sprint();
                break;
        }
        // Display energy value on screen
        if(MainHUDHandler.instance.energybar.value != energy) MainHUDHandler.instance.energybar.value = energy;

        // Limit use of skill
        if (IsOutOfEnergy) OutOfEnergy();

        // Skill reloading
        if(!_skillEnable && !IsFullOfEnergy)
        {
            if (_timer.IsTimeOut()) RestoreEnergy();
            else _timer.CountTime();
        }
    }
    void OutOfEnergy() => _skillEnable = false;
    void UseSkill()
    {
        _skillEnable = !_skillEnable;
        if (!_skillEnable)
            _timer.ResetTime();
    }
    void ConsumeEnergy() => energy -= consumePerSecond * Time.deltaTime;
    void RestoreEnergy() => energy += Time.deltaTime * regeneratePerSecond;
    void SlowMotion()
    {
        if (_customizator.HealthController.IsDead || IngameMenuHandler.instance.paused) _skillEnable = false;
        
        if (SoundMasterPitch != Time.timeScale) soundMaster.SetFloat("PitchMaster", Time.timeScale);

        switch (_skillEnable)
        {
            case true:
                ConsumeEnergy();
                if (Time.timeScale != slowmotionTimeScale)
                {
                    Time.timeScale = Mathf.Lerp(Time.timeScale, slowmotionTimeScale, Time.deltaTime * 10);
                    Time.fixedDeltaTime = _startFixedDeltaTime * Time.timeScale;
                }
                break;
            case false:
                if (Time.timeScale != 1)
                {
                    Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * 10);
                    Time.fixedDeltaTime = _startFixedDeltaTime * Time.timeScale;
                }
                break;
        }
    }
    void Sprint()
    {
        if (_skillEnable) ConsumeEnergy();
        _animator.speed = _skillEnable ? 1.5f : 1;
    }
}
