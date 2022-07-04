using Gann4Games.Thirdym.NPC;

namespace Gann4Games.Thirdym.StateMachines 
{
    public class NpcInjuryState : IState
    {

        private NpcRagdollController _context;

        public void OnEnterState(StateMachine context)
        {
            _context = context as NpcRagdollController;
            _context.Ragdoll.SetLimbsWeight(0, 5);
            _context.Ragdoll.SetCrouchAnimationState(true);
            _context.Ragdoll.SetVerticalAnimationValue(1);
            _context.Ragdoll.SetRootJointSpring(0);
            _context.Ragdoll.EquipmentController.DropEquippedWeapon();
        }

        public void OnUpdateState(StateMachine context)
        {
            if(_context.Ragdoll.HealthController.IsAlive) _context.SetState(_context.IdleState);
            if(_context.Ragdoll.HealthController.IsDead) _context.SetState(_context.DeadState);
        }

        public void OnExitState(StateMachine context)
        {
            _context.Ragdoll.SetCrouchAnimationState(false);
        }

    }
}