using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Gann4Games;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SmartTurret : ObjectScanner {
    private enum TurretStatus
    {
        Searching,
        Identifying,
        Attacking,
    }
    
    
    [Space]
    [Header("Turret settings")]
    [SerializeField] private TurretStatus status;
    
    [SerializeField] private Transform joint;

    [SerializeField] private Collider character;

    private void Update()
    {
        if (!character) return;
        Quaternion rotation = Quaternion.LookRotation(character.transform.position - transform.position, transform.up);
        joint.rotation = Quaternion.Lerp(joint.rotation, rotation, Time.deltaTime);
    }

    public override void Scan()
    {
        character = GetCollidersInRange()
            .Where(o => o.GetComponent<CharacterCustomization>())
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position))
            .FirstOrDefault();
    }
}
