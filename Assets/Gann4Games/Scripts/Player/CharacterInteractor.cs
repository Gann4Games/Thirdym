using System;
using System.Linq;
using UnityEngine;
using Gann4Games.Thirdym.Interactables;

[RequireComponent(typeof(SphereCollider))]
public class CharacterInteractor : MonoBehaviour
{
    [SerializeField] float overlapRadius = 1;
    SphereCollider _collider => GetComponent<SphereCollider>();
    public RagdollController Ragdoll { get; private set; }

    private void Awake()
    {
        if (TryGetComponent(out RagdollController character)) Ragdoll = character;
    }

    private void OnValidate()
    {
        if (!_collider.isTrigger) _collider.isTrigger = true;
        _collider.radius = overlapRadius;
    }

    private void Update()
    {
        if (Ragdoll.Customizator.isNPC) return;
        if (Ragdoll.InputHandler.use) RaiseInteract();
    }

    InteractableObject ClosestInteractable() => Physics.OverlapSphere(transform.position, overlapRadius)
            .Where(o => o.GetComponent<InteractableObject>())
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault()
            ?.GetComponent<InteractableObject>();

    private void OnTriggerEnter(Collider other) => RaiseTooltip();

    void RaiseInteract() => ClosestInteractable()?.Interact(Ragdoll);

    void RaiseTooltip() => ClosestInteractable()?.ShowTooltip();
}
