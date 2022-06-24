using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Utility;

public class LiquidObject : MonoBehaviour
{
    [SerializeField] GameObject splashEffect;
    [SerializeField] float damageAmount = 0;
    [SerializeField] float damageDelay = 0.5f;
    public float liquidDrag = 2;
    public bool blocksSound = true;
    public Vector3 buoyancyDirection = Vector3.up*4.5f;
    [SerializeField] AudioClip sfxDamage;

    List<CharacterCustomization> _ragdollsInside = new List<CharacterCustomization>();
    TimerTool _timer;
    private void Start()
    {
        _timer = new TimerTool(damageDelay);
    }
    private void Update()
    {
        if (damageAmount == 0 || _ragdollsInside.Count == 0) return;
        _timer.Count();
        if (_timer.HasFinished())
        {
            SendDamage();
            _timer.Reset();
        }
    }
    void SendDamage()
    {
        foreach(CharacterCustomization rag in _ragdollsInside)
        {
            if (!rag.HealthController.IsDead)
            {
                rag.HealthController.DealDamage(damageAmount, Vector3.zero);
                rag.PlaySFX(sfxDamage);
            }
        }
    }
    void CreateSplash(Transform where)
    {
        Vector3 _creationPosition = new Vector3(where.position.x, transform.position.y, where.position.z);

        if (splashEffect)
        {
            GameObject _splash = Instantiate(splashEffect, _creationPosition, Quaternion.identity, where);

            LiquidSplash _splashComponent = _splash.GetComponent<LiquidSplash>();
            _splashComponent.waterLevel = transform.position.y;
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        Rigidbody rb = collision.GetComponent<Rigidbody>();
        CharacterCustomization rag = collision.GetComponent<CharacterBodypart>().character;
        if (rag && !_ragdollsInside.Contains(rag))
        {
            _ragdollsInside.Add(rag);
        }

        if(rb) CreateSplash(collision.transform);
    }
    void OnTriggerExit(Collider collision)
    {
        CharacterCustomization rag = collision.GetComponent<CharacterBodypart>().character;
        if (rag && _ragdollsInside.Contains(rag))
        {
            _ragdollsInside.Remove(rag);
        }
    }
}
