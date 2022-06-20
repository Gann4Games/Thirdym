using UnityEngine;
using Gann4Games.Thirdym.Core;
using Gann4Games.Thirdym.Events;

[RequireComponent(typeof(ExplosionHandler))]
[RequireComponent(typeof(CollisionEvents))]
public class ExplosiveObject : BreakableObject
{
    ExplosionHandler _explosionHandler;
    CollisionEvents _collisionEvents;

    private void Awake() => Initialize();

    public override void Initialize()
    {
        base.Initialize();
        if (TryGetComponent(out ExplosionHandler explosion)) _explosionHandler = explosion;
        if (TryGetComponent(out CollisionEvents collision)) _collisionEvents = collision;
        SetHealth(_explosionHandler.explosiveData.health);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        _explosionHandler.Explode();
    }

    private void OnEnable()
    {
        _collisionEvents.OnCollideHard += CollideHard;
        _collisionEvents.OnCollideMedium += CollideMedium;
        _collisionEvents.OnCollideSoft += CollideSoft;
    }
    private void OnDisable()
    {
        _collisionEvents.OnCollideHard -= CollideHard;
        _collisionEvents.OnCollideMedium -= CollideMedium;
        _collisionEvents.OnCollideSoft -= CollideSoft;
    }
    void CollideHard(object sender, CollisionEvents.CollisionArgs args) => DealDamage(25, DamageType.Collision, Vector3.zero);
    void CollideMedium(object sender, CollisionEvents.CollisionArgs args) => DealDamage(10, DamageType.Collision, Vector3.zero);
    void CollideSoft(object sender, CollisionEvents.CollisionArgs args) => DealDamage(1, DamageType.Collision, Vector3.zero);
}
