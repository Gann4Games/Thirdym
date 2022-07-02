#if UNITY_EDITOR
using UnityEngine;

namespace AwSim.Tags
{
    [AddComponentMenu("Tags/Decorative")]
    [DisallowMultipleComponent]
    public class Decorative : MonoBehaviour 
    {
        [SerializeField] private string decorativeName;
        void OnValidate() 
        {
            gameObject.name = $"<--  {decorativeName.ToUpper()}  -->";
        }
    }
}
#endif