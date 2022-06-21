using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Gann4Games.Thirdym.Interactables;
using System.Collections;

namespace Gann4Games
{
    public class CharacterInteractor : MonoBehaviour
    {
        [SerializeField] float overlapRadius = 1;

        CharacterCustomization _character;

        private void Awake()
        {
            if (TryGetComponent(out CharacterCustomization character)) _character = character;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 1, 0.25f);
            Gizmos.DrawWireSphere(transform.position, overlapRadius);
        }

        private void Update()
        {
            if (_character.isNPC) return;
            if (_character.InputHandler.use) RaiseInteract();
        }

        void RaiseInteract()
        {
            // Get all colliders that have the component InteractableObject
            Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius).Where(col => col.GetComponent<InteractableObject>()).ToArray();

            // From the colliders, return the InteractableObject components
            InteractableObject[] interactables = colliders.Select(col => col.GetComponent<InteractableObject>()).ToArray();

            // From the InteractableObjects, return the closest one
            InteractableObject closestInteractable = interactables.OrderBy(interactable => Vector3.Distance(transform.position, interactable.transform.position)).FirstOrDefault();

            // Interact with the closest InteractableObject
            closestInteractable?.Interact(_character);
       }
    }
}
