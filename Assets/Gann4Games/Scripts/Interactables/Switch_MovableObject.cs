using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Gann4Games.Thirdym.Enums;
using Gann4Games.Thirdym.Localization;
using System.Text.RegularExpressions;

namespace Gann4Games.Thirdym.Interactables
{
    [System.Serializable]
    public class SpawnParameters
    {
        public GameObject prefab;
        public GameObject spawnParticle;
        public Vector3 position, direction = new Vector3(0, 0, 1);
    }
    [RequireComponent(typeof(AudioSource))]
    public class Switch_MovableObject : MonoBehaviour
    {
        public enum Mode
        {
            Switch_Set,
            Switch_Toggle,
            Trigger_Enter,
            Trigger_Exit,
            Trigger_EnterExit,
            Trigger_Enter_Toggle,
            Trigger_Exit_Toggle,
            Spawn_Prefab,
        }

        public Mode mode;
        bool enter = false;
        bool exit = false;

        public SpawnParameters SpawnSettings;

        public string UnlockedText = "Unlocked";
        public string LockedText = "Locked, press F";
        public string InvalidText = "Password denied";
        public string ValidText = "Password accepted";

        public MovableObject[] platforms;
        [Header("Switch options")]
        PlayerCameraController cam;
        static public bool EnteringPassword;
        public bool RequirePassword;
        public string PasswordRequired;
        public PasswordEditor passwordEditor;
        [Tooltip("Main value")]
        public int[] positionToSet1;
        [Tooltip("Secondary value")]
        public int[] positionToSet2;
        bool positionSet;
        [Header("Trigger options")]
        [Tooltip("Delay in seconds")]
        public float activationTime;
        [Tooltip("Trigger enter action")]
        public int triggerPosition1;
        [Tooltip("Trigger exit action")]
        public int triggerPosition2;
        public string enableByTag;
        [Header("Audio")]
        public AudioMixerGroup output;
        [Range(0, 1)] public float spatialBlend = 1;
        public float maxDistance = 100;
        [Space]
        public AudioClip useSFX;
        public AudioClip blockedSFX;
        AudioSource sourceAudio;
        private void Start()
        {
            sourceAudio = GetComponent<AudioSource>();
            sourceAudio.outputAudioMixerGroup = output;
            sourceAudio.spatialBlend = spatialBlend;
            sourceAudio.maxDistance = maxDistance;

            passwordEditor = GetComponentInChildren<PasswordEditor>();
            try { passwordEditor.currentSwitch = this; } catch (System.NullReferenceException) { }
            passwordEditor.gameObject.GetComponent<Canvas>().worldCamera = null;

            if (RequirePassword) passwordEditor.UIText.text = LockedText;
            else passwordEditor.UIText.text = UnlockedText;
        }
        private void Update()
        {
            if (PlayerInputHandler.instance.pause && cam != null)
            {
                //cam.GetComponent<Camera>().depth = -10;
                //cam.SetActive(false);.
                cam.camMode = CameraMode.Player;
                passwordEditor.gameObject.GetComponent<Canvas>().worldCamera = null;
                EnteringPassword = false;
            }
        }
        public void CheckPassword(string password)
        {
            if (password == PasswordRequired)
            {
                RequirePassword = true;
                EnteringPassword = false;
                passwordEditor.UIText.text = ValidText;
                passwordEditor.Code = "";
                if (mode == Mode.Switch_Set || mode == Mode.Switch_Toggle)
                {
                    positionSet = !positionSet;
                    for (int i = 0; i < platforms.Length; i++)
                    {
                        if (positionSet)
                            SetPosition(positionToSet1[i]);
                        else
                            SetPosition(positionToSet2[i]);
                    }
                }
            }
            else
            {
                RequirePassword = true;
                EnteringPassword = false;
                passwordEditor.UIText.text = InvalidText;
                passwordEditor.Code = "";
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            if (platforms != null)
            {
                for (int i = 0; i < platforms.Length; i++)
                    Gizmos.DrawLine(transform.position, platforms[i].transform.position);
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (mode == Mode.Spawn_Prefab)
            {
                Gizmos.color = Color.red;
                if (SpawnSettings.prefab != null)
                {
                    Gizmos.DrawLine(transform.position, transform.position + SpawnSettings.position);
                    Gizmos.DrawSphere(transform.position + SpawnSettings.position, 0.25f);
                    Gizmos.DrawLine(transform.position + SpawnSettings.position, transform.position + SpawnSettings.position + transform.TransformDirection(SpawnSettings.direction));
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.HasTag("Player") && other.gameObject.layer == LayerMask.NameToLayer("ControllerCollider"))
            {
                if (PlayerInputHandler.instance.use)
                {
                    if (!EnteringPassword && RequirePassword)
                    {
                        EnteringPassword = true;
                        passwordEditor.gameObject.GetComponent<Canvas>().worldCamera = cam.activeCamera;
                        passwordEditor.UIText.text = "Insert password";
                        //cam.SetActive(true);
                        cam.camMode = CameraMode.ButtonSwitch;
                        cam.buttonConfig.target = transform;
                        //cam.GetComponent<Camera>().depth = 10;
                    }
                    else if (!RequirePassword)
                    {
                        if (mode == Mode.Switch_Set || mode == Mode.Switch_Toggle)
                        {
                            positionSet = !positionSet;
                            for (int i = 0; i < platforms.Length; i++)
                            {
                                if (positionSet)
                                    SetPosition(positionToSet1[i]);
                                else
                                    SetPosition(positionToSet2[i]);
                            }
                        }
                        else if (mode == Mode.Spawn_Prefab)
                        {
                            GameObject newParticle = Instantiate(SpawnSettings.spawnParticle);
                            newParticle.transform.position = SpawnSettings.position;

                            Destroy(newParticle, 5);

                            GameObject newPrefab = Instantiate(SpawnSettings.prefab);
                            newPrefab.transform.position = transform.position + SpawnSettings.position;
                            newPrefab.transform.LookAt(transform.position + SpawnSettings.position + transform.TransformDirection(SpawnSettings.direction));
                        }
                    }
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerCameraController>())
                cam = other.gameObject.GetComponent<PlayerCameraController>();

            if (other.gameObject.HasTag(enableByTag) || other.CompareTag(enableByTag))
            {
                switch (mode)
                {
                    case Mode.Trigger_Enter:
                        StartCoroutine(TempTrigger(triggerPosition1));
                        break;
                    case Mode.Trigger_EnterExit:
                        StartCoroutine(TempTrigger(triggerPosition2));
                        break;
                    case Mode.Trigger_Enter_Toggle:
                        enter = !enter;
                        if (enter) StartCoroutine(TempTrigger(triggerPosition1));
                        else StartCoroutine(TempTrigger(triggerPosition2));
                        break;
                }
            }
            if (other.gameObject.HasTag("Player") && other.gameObject.layer == LayerMask.NameToLayer("ControllerCollider") && (mode == Mode.Switch_Set || mode == Mode.Switch_Toggle))
            {
                // Unit Testign would come handy here, dumbass.
                // Refactor required.
                string useKey = PlayerInputHandler.instance.gameplayControls.Player.Use.ToString();
                Match match = Regex.Match(useKey, @"\w.$");
                useKey = match.Value.TrimEnd(']').ToUpper();

                switch (LanguagePrefs.Language)
                {
                    case AvailableLanguages.English:
                        NotificationHandler.Notify($"[{UnlockedText}] Press {useKey} to use.", 3, 1, false);
                        break;
                    case AvailableLanguages.Español:
                        NotificationHandler.Notify($"[{UnlockedText}] Presiona {useKey} para usar.", 3, 1, false);
                        break;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerCameraController>() && !EnteringPassword)
                cam = null;
            if (other.gameObject.HasTag(enableByTag) || other.CompareTag(enableByTag))
            {
                if (mode == Mode.Trigger_Exit)
                    StartCoroutine(TempTrigger(triggerPosition2));
                else if (mode == Mode.Trigger_EnterExit)
                    StartCoroutine(TempTrigger(triggerPosition1));
                else if (mode == Mode.Trigger_Exit_Toggle)
                {
                    exit = !exit;
                    if (exit)
                        StartCoroutine(TempTrigger(triggerPosition1));
                    else
                        StartCoroutine(TempTrigger(triggerPosition2));
                }
            }
        }
        public IEnumerator TempTrigger(int positionToSet)
        {
            yield return new WaitForSeconds(activationTime);
            for (int i = 0; i < platforms.Length; i++)
                platforms[i].positionToFollow = positionToSet;

        }
        public void SetPosition(int positionToSet)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (positionToSet < platforms[i].positions.Length)
                {
                    if (mode == Mode.Switch_Set)
                    {
                        platforms[i].positionToFollow = positionToSet1[i];
                        sourceAudio.Stop();
                        if (platforms[i].objectToMove.position == platforms[i].positions[positionToSet1[i]])
                            sourceAudio.PlayOneShot(blockedSFX);
                        else
                            sourceAudio.PlayOneShot(useSFX);
                    }
                    else if (mode == Mode.Switch_Toggle && platforms[i].transform.position == platforms[i].positions[platforms[i].positionToFollow])
                    {
                        platforms[i].positionToFollow = positionToSet;
                        sourceAudio.Stop();
                        sourceAudio.PlayOneShot(useSFX);
                    }
                }
            }
        }
    }
}