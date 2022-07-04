using Gann4Games.Thirdym.Utility;
using UnityEngine;
public class HingeJointTarget : MonoBehaviour
{
    public HingeJoint hj;
    public Transform target;
    [Tooltip("Only use one of these values at a time. Toggle invert if the rotation is backwards.")]
    public bool x, y, z, invert;

    private float _startJointSpring;
    private float _springMultiplier;

    public void SetJointWeight(float value)
    {
        _springMultiplier = value;
        hj.spring = PhysicsTools.SetHingeJointSpring(hj.spring, _startJointSpring * _springMultiplier);
    }

    private void Awake()
    {
        hj = GetComponent<HingeJoint>();
        _startJointSpring = hj.spring.spring;
    }

    private void Update()
    {
        if (hj != null)
        {
            JointSpring jointSpring = hj.spring;
            if (x)
            {

                jointSpring.targetPosition = target.transform.localEulerAngles.x;
                if (jointSpring.targetPosition > 180)
                    jointSpring.targetPosition -= 360;
                if (invert)
                    jointSpring.targetPosition *= -1;

                jointSpring.targetPosition = Mathf.Clamp(jointSpring.targetPosition, hj.limits.min + 5, hj.limits.max - 5);
            }
            else if (y)
            {
                jointSpring.targetPosition = target.transform.localEulerAngles.y;
                if (jointSpring.targetPosition > 180)
                    jointSpring.targetPosition -= 360;
                if (invert)
                    jointSpring.targetPosition *= -1;

                jointSpring.targetPosition = Mathf.Clamp(jointSpring.targetPosition, hj.limits.min + 5, hj.limits.max - 5);
            }
            else if (z)
            {
                jointSpring.targetPosition = target.transform.localEulerAngles.z;
                if (jointSpring.targetPosition > 180)
                    jointSpring.targetPosition -= 360;
                if (invert)
                    jointSpring.targetPosition *= -1;

                jointSpring.targetPosition = Mathf.Clamp(jointSpring.targetPosition, hj.limits.min + 5, hj.limits.max - 5);

            }

            jointSpring.spring = _startJointSpring * _springMultiplier;
            hj.spring = jointSpring;
        }
    }
}