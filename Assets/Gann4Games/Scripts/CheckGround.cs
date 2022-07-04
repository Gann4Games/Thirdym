using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Localization; // a very bad code smell...

public class CheckGround : MonoBehaviour {

    public bool IsGrounded => _grounded;
    public bool IsSwimming => _swimming;
    public RagdollController Ragdoll { get; private set; }

    bool _grounded;
    bool _swimming;

    private void Start()
    {
        Ragdoll = transform.parent.GetComponentInChildren<RagdollController>();
    }
    private void Update()
    {
        if (Ragdoll.HealthController.IsDead)
            _grounded = false;
    }
    
    void OnTriggerStay(Collider collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Map":
                _grounded = true;
                break;
            case "Water":
                _swimming = true;
                break;
        }
    }
    void OnTriggerExit(Collider collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Map":
                _grounded = false;
                break;
            case "Water":
                _swimming = false;
                break;
        }
    }
}
