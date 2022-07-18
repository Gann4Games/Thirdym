using System.Collections;
using UnityEngine;
using Gann4Games.Thirdym.ScriptableObjects;
using System;
using Gann4Games.Thirdym.Utility;

namespace Gann4Games.Thirdym.ShootSystem
{
    public class ShootBehaviourLoader : MonoBehaviour
    {
        public event EventHandler OnFire;
        public SO_WeaponPreset weaponData;
        public ShootBehaviour behaviour;

        public RagdollController Ragdoll { get; private set; }
        public bool IsFiring { get; private set; }

        private Coroutine _coroutine;
        private TimerTool _timer = new TimerTool(1);


        public void SetFireStatus(bool value) => IsFiring = value;

        private void Awake()
        {
            Ragdoll = GetComponentInParent<RagdollController>();
        }

        private void Update()
        {
            if(weaponData) _timer.SetMaxTime(weaponData.bulletFireTime);
            
            _timer.Count();
            if(_timer.HasFinished() && IsFiring)
            {
                _timer.Reset();
                Fire();
            }

            // UI
            if(Ragdoll.Customizator.isPlayer) MainHUDHandler.instance.crosshairImage.fillAmount = _timer.CurrentTime / weaponData.bulletFireTime;
        }

        // private IEnumerator FireCoroutine()
        // {
        //     while(true)
        //     {
        //         if(behaviour && IsFiring) Fire();
        //         yield return new WaitForSeconds(behaviour ? weaponData.bulletFireTime : 1);
        //     }
        // }

        private void Fire()
        {
            OnFire?.Invoke(this, EventArgs.Empty);
            behaviour.Fire(this);
        }
    }
}
