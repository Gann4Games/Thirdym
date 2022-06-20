using UnityEngine;

namespace Gann4Games.Thirdym.Interactables
{
    /// <summary>
    /// Base class for objects that interact with the player with the `USE` key.
    /// </summary>
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        public virtual void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}
