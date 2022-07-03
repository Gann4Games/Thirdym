using UnityEngine;

public class NPC_Healthbar : MonoBehaviour
{
    public float height = .7f;
    public GameObject healthbarPrefab;
    public TMPro.TextMeshPro textField;

    private Animator _anim;
    private CharacterCustomization _character;

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * height);
    }
    private void Start()
    {
        _character = GetComponent<CharacterCustomization>();
        healthbarPrefab = Instantiate(healthbarPrefab, transform.position + Vector3.up*height, transform.rotation);

        textField = healthbarPrefab.GetComponentInChildren<TMPro.TextMeshPro>();

        _anim = healthbarPrefab.GetComponent<Animator>();
        healthbarPrefab.transform.parent = transform;
    }
    private void Update()
    {
        if(_anim) _anim.SetFloat("fill", _character.HealthController.HealthPercentage);
        textField.text = string.Format("{0}", _character.preset.character_name);
    }
}
