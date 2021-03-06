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
    [Space]
    [Header("Right Arm")]
    public Transform rightShoulder;
    public Transform rightElbow;
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
public class CharacterCustomization : MonoBehaviour
{

    public bool isNPC;
    public bool usePlayerPrefs = false;

    [SerializeField] Animator ragdollAnimator;

    public SO_RagdollPreset preset;
    public Bodyparts baseBody;

    AudioSource _audioPlayer;
    RagdollController _ragdollController;
    CharacterHealthSystem _healthController;
    ShootSystem _shootSystem;
    EquipmentSystem _equipmentController;
    CharacterArms _armController;
    PlayerCameraController _cameraController;
    NPC_Ragdoll _npc;
    CharacterWalljump _walljumpController;

    public Animator Animator => ragdollAnimator;
    public AudioSource SoundSource => _audioPlayer;
    public RagdollController RagdollController  => _ragdollController;
    public CharacterHealthSystem HealthController => _healthController;
    public ShootSystem ShootSystem => _shootSystem; 
    public EquipmentSystem EquipmentController => _equipmentController; 
    public CharacterArms ArmController => _armController;
    public PlayerCameraController CameraController => _cameraController;
    public NPC_Ragdoll NPC => _npc;
    public CharacterWalljump WalljumpController => _walljumpController;

    private void Awake()
    {
        if (usePlayerPrefs)
            preset = PlayerPreferences.instance.suit_list[PlayerPreferences.instance.json_structure.choosen_suit];

        _audioPlayer = GetComponent<AudioSource>();

        #region Establishing parameters for required components
        transform.tag = preset.faction;

        _ragdollController = GetComponent<RagdollController>();
        _healthController = GetComponent<CharacterHealthSystem>();
        _shootSystem = GetComponent<ShootSystem>();
        _equipmentController = GetComponent<EquipmentSystem>();
        _armController = GetComponent<CharacterArms>();
        _cameraController = GetComponent<PlayerCameraController>();
        _npc = GetComponent<NPC_Ragdoll>();
        _walljumpController = GetComponent<CharacterWalljump>();
        #endregion

        if (!_npc) isNPC = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        if(baseBody.body && baseBody.head) Gizmos.DrawLine(baseBody.body.position, baseBody.head.position);

        if(baseBody.leftShoulder) Gizmos.DrawLine(baseBody.body.position, baseBody.leftShoulder.position);
        if(baseBody.leftElbow) Gizmos.DrawLine(baseBody.leftShoulder.position, baseBody.leftElbow.position);

        if(baseBody.rightShoulder) Gizmos.DrawLine(baseBody.body.position, baseBody.rightShoulder.position);
        if(baseBody.rightElbow) Gizmos.DrawLine(baseBody.rightShoulder.position, baseBody.rightElbow.position);

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

        // Get suit visuals component (for applying transforms)
        BattleSuitVisuals suitVisuals = characterSuit.GetComponent<BattleSuitVisuals>();

        // Parent suit to base body
        #region Parenting
        if(suitVisuals.limbs.body) suitVisuals.limbs.body.parent = baseBody.body;
        if(suitVisuals.limbs.head) suitVisuals.limbs.head.parent = baseBody.head;

        if(suitVisuals.limbs.leftShoulder) suitVisuals.limbs.leftShoulder.parent = baseBody.leftShoulder;
        if(suitVisuals.limbs.leftElbow) suitVisuals.limbs.leftElbow.parent = baseBody.leftElbow;
        //suitVisuals.limbs.leftHand.parent = baseBody.leftHand; Hands are not implemented yet.

        if(suitVisuals.limbs.rightShoulder) suitVisuals.limbs.rightShoulder.parent = baseBody.rightShoulder;
        if(suitVisuals.limbs.rightElbow) suitVisuals.limbs.rightElbow.parent = baseBody.rightElbow;
        //suitVisuals.limbs.rightHand.parent = baseBody.rightHand; Hands are not implemented yet.

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
        CharacterPoser char_poser = GetComponent<CharacterPoser>();
        if (char_poser) char_poser.PoseCharacter();
    }

    #region Sounds
    public void PlaySFX(AudioClip sfx)
    {
        _audioPlayer.PlayOneShot(sfx);
    }
    public void PlayEnemyDownSFX()
    {
        PlaySFX(AudioTools.GetRandomClip(preset.enemyDownSFX));
    }
    public void PlayAlertSFX()
    {
        PlaySFX(AudioTools.GetRandomClip(preset.alertSFX));
    }
    public void PlayPainSFX()
    {
        PlaySFX(AudioTools.GetRandomClip(preset.painSFX));
    }
    public void PlayInjurySFX()
    {
        PlaySFX(AudioTools.GetRandomClip(preset.injuryStateSFX));
    }
    public void PlayDeathSFX()
    {
        PlaySFX(preset.forcedDeathSFX);
        PlaySFX(AudioTools.GetRandomClip(preset.deathSFX));
    }
    #endregion
}
