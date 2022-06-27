using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Localization; // a very bad code smell...

public class CheckGround : MonoBehaviour {

    public bool IsGrounded => _grounded;
    public bool IsSwimming => _swimming;
    public CharacterCustomization Character { get; private set; }
    EquipmentSystem _equipmentController;

    bool _grounded;
    bool _swimming;

    private void Start()
    {
        Character = transform.parent.GetComponentInChildren<CharacterCustomization>();
        _equipmentController = Character.EquipmentController;
    }
    private void Update()
    {
        if (Character.HealthController.IsDead)
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
