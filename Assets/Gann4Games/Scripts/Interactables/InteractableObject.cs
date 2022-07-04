using UnityEngine;

namespace Gann4Games.Thirdym.Interactables
{
    /// <summary>
    /// Base class for objects that interact with the player with the `USE` key.
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] protected string hintMessage = "Press {0} to interact."; 
        public string UseKey => PlayerInputHandler.InputAsString(PlayerInputHandler.instance.gameplayControls.Player.Use);
        public virtual string Hint => string.Format(hintMessage, UseKey.ToUpper());
        public abstract void Interact(RagdollController ragdoll = null);
        public virtual void ShowTooltip() => NotificationHandler.Notify(Hint);
    }
}
