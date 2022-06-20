using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Events;
using Gann4Games.Thirdym.Core;

[RequireComponent(typeof(CollisionEvents))]
[RequireComponent(typeof(AudioSource))]
public class LootCrate : BreakableObject {
    [System.Serializable]
    public class ObjectToSpawn
    {
        public GameObject item;
        public int count = 1;
    }

    [Header("Object spawning")]
    [SerializeField] List<ObjectToSpawn> lootContent;

    Rigidbody[] _rigidbodies;

    public override void Initialize()
    {
        base.Initialize();
        ApplyCrateMass();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        SpawnLoot();
    }

    void SpawnLoot()
    {
        for (int i = 0; i < lootContent.Count; i++)
        {
            if (lootContent[i].item == null)
            {
                Debug.LogWarning($"Gann! Crate '{name}' has empty fields, you should check that.");
                continue;
            }

            for (int amount = 0; amount < lootContent[i].count; amount++)
                Instantiate(lootContent[i].item, transform.position, transform.rotation);
        }
    }

    void ApplyCrateMass()
    {
        for (int index = 0; index < lootContent.Count; index++)
        {
            for (int count = 0; count < lootContent[index].count; count++)
            {
                _rigidbodies = lootContent[index].item.GetComponentsInChildren<Rigidbody>();
                for (int rbodies = 0; rbodies < _rigidbodies.Length; rbodies++)
                    GetComponent<Rigidbody>().mass += _rigidbodies[rbodies].mass;
            }
        }
    }
}
