using System;
using System.Linq;
using UnityEngine;
using Gann4Games.Thirdym.Interactables;

[RequireComponent(typeof(SphereCollider))]
public class CharacterInteractor : MonoBehaviour
{
    [SerializeField] float overlapRadius = 1;
    SphereCollider _collider => GetComponent<SphereCollider>();
    public CharacterCustomization Character { get; private set; }

    private void Awake()
    {
        if (TryGetComponent(out CharacterCustomization character)) Character = character;
    }

    private void OnValidate()
    {
        if (!_collider.isTrigger) _collider.isTrigger = true;
        _collider.radius = overlapRadius;
    }

    private void Update()
    {
        if (Character.isNPC) return;
        if (Character.InputHandler.use) RaiseInteract();
    }

    InteractableObject ClosestInteractable()
    {
        // Get all colliders that have the component InteractableObject
        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius).Where(col => col.GetComponent<InteractableObject>()).ToArray();

        // From the colliders, return the InteractableObject components
        InteractableObject[] interactables = colliders.Select(col => col.GetComponent<InteractableObject>()).ToArray();

        // From the InteractableObjects, return the closest one
        return interactables.OrderBy(interactable => Vector3.Distance(transform.position, interactable.transform.position)).FirstOrDefault();
    }

    private void OnTriggerEnter(Collider other) => RaiseTooltip();

    void RaiseInteract() => ClosestInteractable()?.Interact(Character);

    void RaiseTooltip() => ClosestInteractable()?.ShowTooltip();
}
