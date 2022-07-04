using UnityEngine;

namespace Gann4Games.Thirdym.Abilities 
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Sprinting", fileName = "Sprinting Ability")]
    public class SprintingAbility : Ability
    {
        [Header("Ability parameters")]
        [Tooltip("This value will multiply the animation speed.")]
        [SerializeField] private float speedMultiplier = 1.5f;
        public override void OnStartAbility(CharacterSkillHandler context)
        {
            context.Ragdoll.Animator.speed = speedMultiplier;
        }

        public override void OnEndAbility(CharacterSkillHandler context)
        {
            context.Ragdoll.Animator.speed = 1;
        }
    }
}