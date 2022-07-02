using UnityEngine;
using Gann4Games.Thirdym.Enums;
using Gann4Games.Thirdym.Core;

namespace Gann4Games.Thirdym.ScriptableObjects
{

    [CreateAssetMenu(fileName = "New Ragdoll Preset", menuName = "Scriptable Objects/Ragdoll Preset")]
    public class SO_RagdollPreset : ScriptableObject
    {
        [Header("HUD related")]
        public Texture2D profile;
        public string character_name;

        [Space]

        [Header("Ingame parameters")]
        public GameObject battleSuit;
        public CharacterSkillHandler.SkillType skill;
        [Header("Health")]
        public float maximumHealth = 1500;
        public float injuryLevel = 100;
        public float regeneration_rate = 500;
        [Header("Damage")]
        [Tooltip("Damage amount caused by character's blades.")]
        public float bladeDamage = 10;

        [Space]

        [Header("Player effects")]
        [Tooltip("Effect displayed when shot.")]
        public GameObject blood_splat;
        [Tooltip("Effect displayed after shot.")]
        public GameObject blood_bleed;
        [Tooltip("Effect displayed on dismembered limbs.")]
        public GameObject blood_squirt;
        public Color bloodColor = Color.red;
        [ColorUsage(true, true)]
        public Color bladesColor = Color.cyan;

        [Space]

        [Header("Tags")]
        public string faction = "Factions/MyFaction";
        public string[] enemyTags;
        public string[] allyTags;

        [Space]

        [Header("Sound Effects (SFX)")]
        [Header("Collision SFX")]
        public AudioClip sfxCollideHard;
        public AudioClip sfxCollideMedium;
        public AudioClip sfxCollideSoft;
        [Header("Pain and death SFX")]
        public AudioClip[] enemyDownSFX;
        public AudioClip[] alertSFX;
        public AudioClip[] painSFX;
        public AudioClip[] injuryStateSFX;
        public AudioClip[] deathSFX;
        public AudioClip forcedDeathSFX;    // Plays at the same time that deathSFX

        [Header("Credits")]
        public string author;

        public GameObject BloodSplatFX() => BloodFX(blood_splat);
        public GameObject BloodBleedFX() => BloodFX(blood_bleed);
        public GameObject BloodSquirtFX() => BloodFX(blood_squirt);
        GameObject BloodFX(GameObject obj)
        {
            GameObject newFX = Instantiate(obj);
            ParticleSystem[] particles = newFX.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particles)
            {

                ParticleSystem.MainModule ps_main = ps.main;
                ps_main.startColor = bloodColor;
            }
            return newFX;
        }
    }
}