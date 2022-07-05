using Gann4Games.Thirdym.NPC;
using UnityEngine;

public class NPC_Healthbar : MonoBehaviour
{
    [SerializeField] private float height = .7f;
    [SerializeField] private GameObject healthbarPrefab;

    private TMPro.TextMeshPro _textField;
    private Animator _animator;
    private NpcRagdollController _npc;

    private Vector3 followPosition => transform.position + Vector3.up * height;

    private void Awake()
    {
        _npc = GetComponent<NpcRagdollController>();
        healthbarPrefab = Instantiate(healthbarPrefab, followPosition, transform.rotation);

        _textField = healthbarPrefab.GetComponentInChildren<TMPro.TextMeshPro>();
        _animator = healthbarPrefab.GetComponent<Animator>();
    }
    private void Update()
    {
        if(_animator) _animator.SetFloat("fill", _npc.Ragdoll.HealthController.HealthPercentage);
        _textField.text = string.Format("[{1}] {0}", _npc.Ragdoll.Customizator.preset.character_name, _npc.CurrentStateName);
        healthbarPrefab.transform.position = followPosition;
    }
}
