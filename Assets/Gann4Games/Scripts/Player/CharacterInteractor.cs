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

    InteractableObject ClosestInteractable() => Physics.OverlapSphere(transform.position, overlapRadius)
            .Where(o => o.GetComponent<InteractableObject>())
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault()
            ?.GetComponent<InteractableObject>();

    private void OnTriggerEnter(Collider other) => RaiseTooltip();

    void RaiseInteract() => ClosestInteractable()?.Interact(Character);

    void RaiseTooltip() => ClosestInteractable()?.ShowTooltip();
}
