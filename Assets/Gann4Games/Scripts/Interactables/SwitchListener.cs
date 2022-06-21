using UnityEngine;

namespace Gann4Games.Thirdym.Interactables
{
    /// <summary>
    /// Listens for a signal on the specified switches.
    /// </summary>
    public abstract class SwitchListener : MonoBehaviour
    {
        [SerializeField] InteractableSwitch[] switches;
        private void OnEnable()
        {
            foreach(InteractableSwitch interactable in switches)
                interactable.Signal += InteractableSwitch_Signal;
        }
        private void OnDisable()
        {
            foreach (InteractableSwitch interactable in switches)
                interactable.Signal -= InteractableSwitch_Signal;
        }
        public abstract void InteractableSwitch_Signal(object sender, System.EventArgs e);
    }
}
