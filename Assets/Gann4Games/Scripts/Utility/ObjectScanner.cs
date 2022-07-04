using System.Collections.Generic;
using UnityEngine;

namespace Gann4Games
{
    /// <summary>
    /// Scans for elements around the scene object (gameObject)
    /// </summary>
    public class ObjectScanner
    {
        [System.Serializable]
        public class ScannerData 
        {
            [Header("Scan settings")]
            [Tooltip("The maximum distance the object will scan around. (In units)")]
            public float scanRange = 10;
            
            [Tooltip("The time it takes to scan for objects in the scene repeatedly. (In seconds)")]
            public float scanCooldown = 10;
        }

        public ScannerData data;
        private Vector3 _origin;

        public ObjectScanner(Vector3 origin, ScannerData data)
        {
            Debug.Log("Scanning...");

            this.data = data;
            _origin = origin;
        }

        public Collider[] GetCollidersInRange() => Physics.OverlapSphere(_origin, data.scanRange);
    }
}