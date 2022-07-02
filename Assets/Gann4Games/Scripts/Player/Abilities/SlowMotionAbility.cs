using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

namespace Gann4Games.Thirdym.Abilities
{
    [CreateAssetMenu(menuName ="Scriptable Objects/Abilities/SlowMotion", fileName ="Slow Motion Ability")]
    public class SlowMotionAbility : Ability
    {
        [Header("Ability parameters")]
        [SerializeField] private float slowmotionTimeScale = 0.3f;
        [SerializeField] private float transitionTime = 1;
        [SerializeField] private AudioMixer audioMixer;

        public override void OnStartAbility(CharacterSkillHandler context) => SetWorldSpeed(slowmotionTimeScale);

        public override void OnEndAbility(CharacterSkillHandler context) => SetWorldSpeed(1);

        private void SetWorldSpeed(float speed)
        {
            audioMixer.DOSetFloat("PitchMaster", speed, transitionTime);
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, speed, transitionTime);
        }
    }
}