using System.Linq;
using Gann4Games;
using UnityEngine;

public class SmartTurret : MonoBehaviour {

    [Space]
    [Header("Turret settings")]
    [SerializeField] private Transform joint;
    [SerializeField] private Collider nearestCharacter;
    [SerializeField] private ObjectScanner.ScannerData scannerData;
    private ObjectScanner scanner { get => new ObjectScanner(transform.position, scannerData); }

    private void Awake()
    {
        InvokeRepeating(nameof(Scan), 0, scannerData.scanCooldown);
    }

    private void Update()
    {
        if (!nearestCharacter) return;
        Quaternion rotation = Quaternion.LookRotation(nearestCharacter.transform.position - transform.position, transform.up);
        joint.rotation = Quaternion.Lerp(joint.rotation, rotation, Time.deltaTime);
    }

    public void Scan()
    {
        nearestCharacter = scanner.GetCollidersInRange()
            .Where(o => o.GetComponent<CharacterCustomization>())
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position))
            .FirstOrDefault();
    }
}
