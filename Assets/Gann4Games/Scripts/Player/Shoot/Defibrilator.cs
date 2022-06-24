using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Utility;

public class Defibrilator : MonoBehaviour
{
    [SerializeField] float repeatTime = 1.2f;
    [SerializeField] GameObject electroParticle;
    [SerializeField] AudioClip healSFX;
    [SerializeField] AudioClip shockSFX;
    [SerializeField] AudioClip switchSFX;
    List<CharacterBodypart> _otherBodyparts = new List<CharacterBodypart>();

    TimerTool _timer;
    CharacterCustomization _user;
    AudioSource _soundSource;

    bool _isDangerous;
    private void Start()
    {
        //Invoke("Electrocute", repeatTime);
        _user = GetComponentInParent<CharacterCustomization>();
        _timer = new TimerTool(repeatTime);
        _soundSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        electroParticle.SetActive(false);
    }
    private void Update()
    {
        _timer.Count();
        if (!_user.isNPC)
        {
            if (PlayerInputHandler.instance.firing) Electrocute();
            if (PlayerInputHandler.instance.firing)
            {
                //canElectrocute = true;
                if (_isDangerous)
                {
                    _soundSource.loop = true;
                    _soundSource.clip = shockSFX;
                    _soundSource.Play();
                    electroParticle.SetActive(true);
                }
            }
            else if (!PlayerInputHandler.instance.firing)
            {
                //canElectrocute = false;
                _soundSource.loop = false;
                _soundSource.clip = null;
                if (_isDangerous)
                    _soundSource.Stop();
                electroParticle.SetActive(false);
            }

            if (PlayerInputHandler.instance.walking)
            {
                ChangeMode();
            }
        }
    }
    public void ChangeMode()
    {
        _soundSource.PlayOneShot(switchSFX);
        _isDangerous = !_isDangerous;
        if (_isDangerous)
            repeatTime = 0.1f;
        else
            repeatTime = 1.2f;
        _timer.SetMaxTime(repeatTime);
    }
    public void Electrocute()
    {
        if (_timer.HasFinished() && gameObject.activeInHierarchy)
        {
            _timer.Reset();
            if (_isDangerous)
            {
                for (int i = 0; i < _otherBodyparts.Count; i++)
                {
                    CharacterCustomization otherCharacter = _otherBodyparts[i].character;
                    otherCharacter.HealthController.DealDamage(15, Vector3.zero, true);
                }
            }
            else
            {
                _soundSource.PlayOneShot(healSFX);
                for (int i = 0; i < _otherBodyparts.Count; i++)
                {
                    CharacterCustomization otherCharacter = _otherBodyparts[i].character;
                    float currentHealth = otherCharacter.HealthController.Health;
                    float injuryLevel = otherCharacter.HealthController.InjuryLevel;
                    if(currentHealth < injuryLevel)
                    {
                        otherCharacter.HealthController.DealDamage(-25, Vector3.zero, false);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CharacterBodypart otherBodypart = other.GetComponent<CharacterBodypart>();
        if(otherBodypart)
        {
            if (!_otherBodyparts.Contains(otherBodypart))
                _otherBodyparts.Add(otherBodypart);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CharacterBodypart otherBodypart = other.GetComponent<CharacterBodypart>();
        if(otherBodypart)
        {
            if (_otherBodyparts.Contains(otherBodypart))
                _otherBodyparts.Remove(otherBodypart);
        }
    }
}
