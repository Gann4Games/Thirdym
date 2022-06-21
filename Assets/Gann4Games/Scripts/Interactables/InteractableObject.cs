using UnityEngine;

namespace Gann4Games.Thirdym.Interactables
{
    /// <summary>
    /// Base class for objects that interact with the player with the `USE` key.
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        public string UseKey => PlayerInputHandler.InputAsString(PlayerInputHandler.instance.gameplayControls.Player.Use);
        public virtual string Hint => string.Format("Press '{0}' to interact.", UseKey);
        public abstract void Interact(CharacterCustomization character = null);
        public virtual void ShowTooltip() => NotificationHandler.Notify(Hint);

        private void OnTriggerEnter(Collider other)
        {
            CharacterCustomization character = other.GetComponent<CheckGround>().character;
            if(character && character.isPlayer) ShowTooltip();
        }
    }
}
