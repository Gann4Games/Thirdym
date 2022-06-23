using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gann4Games
{
    /// <summary>
    /// Scans for elements around the scene object (gameObject)
    /// </summary>
    public class ObjectScanner : MonoBehaviour
    {
        [Header("Scan settings")]
        [Tooltip("The maximum distance the object will scan around.")]
        [SerializeField] private float scanRange = 10;
        [Tooltip("The time it takes to scan for objects in the scene repeatedly.")]
        [SerializeField] private float scanCooldownTime = 1;

        [Header("Gizmos")]
        [SerializeField] private Color areaColor = new Color(1, 1, 1, 0.1f);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = areaColor;
            Gizmos.DrawSphere(transform.position, scanRange);
        }

        private void Start() => InvokeRepeating(nameof(Scan), 0, scanCooldownTime);

        public virtual void Scan() { }

        public Collider[] GetCollidersInRange() => Physics.OverlapSphere(transform.position, scanRange);
    }
}
