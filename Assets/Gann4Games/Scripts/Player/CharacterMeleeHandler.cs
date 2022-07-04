using UnityEngine;
using Gann4Games.Thirdym.Events;

public class CharacterMeleeHandler : MonoBehaviour
{
    RagdollController _ragdoll;
        
    public AnimationEventsReader animationEvents;

    private void Start()
    {
        _ragdoll = GetComponent<RagdollController>();
    }

    private void OnEnable()
    {
        animationEvents.OnLeftMeleeAttack += AnimationEvents_OnLeftMeleeAttack;
        animationEvents.OnRightMeleeAttack += AnimationEvents_OnRightMeleeAttack;
    }
    private void OnDisable()
    {
        animationEvents.OnLeftMeleeAttack -= AnimationEvents_OnLeftMeleeAttack;
        animationEvents.OnRightMeleeAttack -= AnimationEvents_OnRightMeleeAttack;
    }

    private void AnimationEvents_OnLeftMeleeAttack(object sender, System.EventArgs args)
    {
        CharacterMeleeObject meleeObject = _ragdoll.Customizator.baseBody.leftHand.GetComponentInChildren<CharacterMeleeObject>();
        if (!meleeObject) return;

        meleeObject.EnableCollider(true);
        _ragdoll.PlayFireSFX();
    }

    private void AnimationEvents_OnRightMeleeAttack(object sender, System.EventArgs args)
    {
        CharacterMeleeObject meleeObject = _ragdoll.Customizator.baseBody.rightHand.GetComponentInChildren<CharacterMeleeObject>();
        if (!meleeObject) return;
        
        meleeObject.EnableCollider(true);
        _ragdoll.PlayFireSFX();
    }
}
