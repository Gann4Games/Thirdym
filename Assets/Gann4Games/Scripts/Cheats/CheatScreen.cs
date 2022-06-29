using UnityEngine;
using UnityEngine.UI;

namespace Gann4Games.Thirdym.Cheats
{
    public class CheatScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("A button that opens the cheat screen. \nThis might be found inside ingame menu's prefab.")]
        [SerializeField] Button btnCheatEnabler;

        private void Awake() => btnCheatEnabler.gameObject.SetActive(true);
        public void CreateObjectOnView(GameObject newObject)
        {
            Instantiate(newObject, PlayerCameraController.Instance.CameraCenterPoint + Vector3.up * 1, Quaternion.identity);
        }
    }
}