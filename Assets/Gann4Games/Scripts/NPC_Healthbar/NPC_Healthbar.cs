using UnityEngine;

public class NPC_Healthbar : MonoBehaviour
{
    [SerializeField] private float height = .7f;
    [SerializeField] private GameObject healthbarPrefab;

    private TMPro.TextMeshPro _textField;
    private Animator _animator;
    private CharacterCustomization _character;

    private Vector3 followPosition => transform.position + Vector3.up * height;

    private void Start()
    {
        _character = GetComponent<CharacterCustomization>();
        healthbarPrefab = Instantiate(healthbarPrefab, transform.position + Vector3.up * height, transform.rotation);

        _textField = healthbarPrefab.GetComponentInChildren<TMPro.TextMeshPro>();
        _animator = healthbarPrefab.GetComponent<Animator>();
    }
    private void Update()
    {
        if(_animator) _animator.SetFloat("fill", _character.HealthController.HealthPercentage);
        _textField.text = string.Format("{0}", _character.preset.character_name);
    }
}
