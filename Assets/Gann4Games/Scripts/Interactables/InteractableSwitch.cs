using System;
using UnityEngine;

namespace Gann4Games.Thirdym.Interactables
{
    public class InteractableSwitch : InteractableObject
    {
        public event EventHandler Signal;
        public override void Interact(CharacterCustomization character = null)
        {
            Signal?.Invoke(this, EventArgs.Empty);
        }
    }
}