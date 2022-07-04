using UnityEngine;
using Gann4Games.Thirdym.Utility;
using Gann4Games.Thirdym.ScriptableObjects;
using Gann4Games.Thirdym.NPC;

[System.Serializable]
public class Bodyparts
{
    [Header("Body & head")]
    public Transform body;
    public Transform head;
    [Space]
    [Header("Left Arm")]
    public Transform leftShoulder;
    public Transform leftElbow;
    public Transform leftHand;
    [Space]
    [Header("Right Arm")]
    public Transform rightShoulder;
    public Transform rightElbow;
    public Transform rightHand;
    [Space]
    [Header("Left Leg")]
    public Transform leftLeg;
    public Transform leftKnee;
    public Transform leftFoot;
    [Space]
    [Header("Right Leg")]
    public Transform rightLeg;
    public Transform rightKnee;
    public Transform rightFoot;
}

/// <summary>
/// 
/// </summary>
public class CharacterCustomization : MonoBehaviour
{
    public bool isNPC => GetComponent<NpcRagdollController>();
    public bool isPlayer => !isNPC;
    public bool usePlayerPrefs = false;

    public SO_RagdollPreset preset;
    public Bodyparts baseBody;

    public RagdollController Ragdoll { get; private set; }

    private void Awake() 
    {
        Ragdoll = GetComponent<RagdollController>();
    }

    private void OnEnable() => Ragdoll.OnReady += Initialize;
    private void OnDisable() => Ragdoll.OnReady -= Initialize;

    private void Initialize(object sender, System.EventArgs args)
    {
        transform.tag = preset.faction;
        if (usePlayerPrefs)
            preset = PlayerPreferences.instance.suit_list[PlayerPreferences.instance.json_structure.choosen_suit];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        if(baseBody.body && baseBody.head) Gizmos.DrawLine(baseBody.body.position, baseBody.head.position);

        if(baseBody.leftShoulder) Gizmos.DrawLine(baseBody.body.position, baseBody.leftShoulder.position);
        if(baseBody.leftElbow) Gizmos.DrawLine(baseBody.leftShoulder.position, baseBody.leftElbow.position);
        if (baseBody.leftHand) Gizmos.DrawLine(baseBody.leftElbow.position, baseBody.leftHand.position);

        if(baseBody.rightShoulder) Gizmos.DrawLine(baseBody.body.position, baseBody.rightShoulder.position);
        if(baseBody.rightElbow) Gizmos.DrawLine(baseBody.rightShoulder.position, baseBody.rightElbow.position);
        if (baseBody.rightHand) Gizmos.DrawLine(baseBody.rightElbow.position, baseBody.rightHand.position);

        if(baseBody.leftLeg) Gizmos.DrawLine(baseBody.body.position, baseBody.leftLeg.position);
        if(baseBody.leftKnee) Gizmos.DrawLine(baseBody.leftLeg.position, baseBody.leftKnee.position);
        if(baseBody.leftFoot) Gizmos.DrawLine(baseBody.leftKnee.position, baseBody.leftFoot.position);

        if(baseBody.rightLeg) Gizmos.DrawLine(baseBody.body.position, baseBody.rightLeg.position);
        if(baseBody.rightKnee) Gizmos.DrawLine(baseBody.rightLeg.position, baseBody.rightKnee.position);
        if (baseBody.rightFoot) Debug.DrawLine(baseBody.rightKnee.position, baseBody.rightFoot.position);
    }
    private void Start()
    {

        // Spawn suit
        GameObject characterSuit = Instantiate(preset.battleSuit, baseBody.body.position, baseBody.body.rotation);
        //characterSuit.transform.localScale = transform.parent.localScale;

        // Get suit visuals component (for applying transforms)
        BattleSuitVisuals suitVisuals = characterSuit.GetComponent<BattleSuitVisuals>();

        // Parent suit to base body
        #region Parenting
        if(suitVisuals.limbs.body) suitVisuals.limbs.body.parent = baseBody.body;
        if(suitVisuals.limbs.head) suitVisuals.limbs.head.parent = baseBody.head;

        if(suitVisuals.limbs.leftShoulder) suitVisuals.limbs.leftShoulder.parent = baseBody.leftShoulder;
        if(suitVisuals.limbs.leftElbow) suitVisuals.limbs.leftElbow.parent = baseBody.leftElbow;
        if(suitVisuals.limbs.leftHand) suitVisuals.limbs.leftHand.parent = baseBody.leftHand;

        if(suitVisuals.limbs.rightShoulder) suitVisuals.limbs.rightShoulder.parent = baseBody.rightShoulder;
        if(suitVisuals.limbs.rightElbow) suitVisuals.limbs.rightElbow.parent = baseBody.rightElbow;
        if(suitVisuals.limbs.rightHand) suitVisuals.limbs.rightHand.parent = baseBody.rightHand;

        if(suitVisuals.limbs.leftLeg) suitVisuals.limbs.leftLeg.parent = baseBody.leftLeg;
        if(suitVisuals.limbs.leftKnee) suitVisuals.limbs.leftKnee.parent = baseBody.leftKnee;
        if(suitVisuals.limbs.leftFoot) suitVisuals.limbs.leftFoot.parent = baseBody.leftFoot;

        if(suitVisuals.limbs.rightLeg) suitVisuals.limbs.rightLeg.parent = baseBody.rightLeg;
        if(suitVisuals.limbs.rightKnee) suitVisuals.limbs.rightKnee.parent = baseBody.rightKnee;
        if(suitVisuals.limbs.rightFoot) suitVisuals.limbs.rightFoot.parent = baseBody.rightFoot;

        // Remove unnecesary or unassigned objects
        Destroy(suitVisuals.gameObject, 1);
        #endregion

        // Pose character for mainmenu
        CharacterPoser charPoser = GetComponent<CharacterPoser>();
        if (charPoser) charPoser.PoseCharacter();
    }

    public void SetAnimationOverride(AnimatorOverrideController animatorOverride)
    {
        if (animatorOverride == null) return;
        Ragdoll.Animator.runtimeAnimatorController = animatorOverride;
    }
}
