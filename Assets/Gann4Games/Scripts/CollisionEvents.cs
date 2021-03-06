using System;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
    public event EventHandler<CollisionArgs> OnCollideHard;
    public event EventHandler<CollisionArgs> OnCollideMedium;
    public event EventHandler<CollisionArgs> OnCollideSoft;

    public class CollisionArgs : EventArgs
    {
        public float collisionMagnitude;
    }

    [SerializeField] float collisionSoftMagnitude = 3;
    [SerializeField] float collisionMediumMagnitude = 6;
    [SerializeField] float collisionHardMagnitude = 12;

    private void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;
        if (collisionForce > collisionHardMagnitude)
            OnCollideHard?.Invoke(this, new CollisionArgs { collisionMagnitude = 0 });
        else if (collisionForce > collisionMediumMagnitude)
            OnCollideMedium?.Invoke(this, new CollisionArgs { collisionMagnitude = 0 });
        else if (collisionForce > collisionSoftMagnitude)
            OnCollideSoft?.Invoke(this, new CollisionArgs { collisionMagnitude = 0 });
    }
}
