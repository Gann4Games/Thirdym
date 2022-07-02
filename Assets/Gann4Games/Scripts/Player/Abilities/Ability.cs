using UnityEngine;

namespace Gann4Games.Thirdym.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [Header("Usage parameters")]
        public float energyConsumptionPerSecond = 10;
        [Tooltip("How much energy per second will regenerate")]
        public float energyRegenerationPerSecond = 5;
        [Tooltip("The amount of seconds to wait before using it again.")]
        public float cooldown = 3;

        public virtual void OnStartAbility(CharacterSkillHandler context) {}
        public virtual void OnUpdateAbility(CharacterSkillHandler context)
        {
            if(context.IsAbilityRunning) context.Consume(energyConsumptionPerSecond);
            else context.Regenerate(energyRegenerationPerSecond);
        }

        public virtual void OnEndAbility(CharacterSkillHandler context) {}
    }
}
