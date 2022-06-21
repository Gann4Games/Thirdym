using UnityEngine;

namespace Gann4Games.Thirdym.Utility
{
    public abstract class PhysicsTools
    {
        public static JointSpring SetHingeJointSpring(JointSpring joint, float value)
        {
            JointSpring newSpring = joint;
            newSpring.spring = value;
            return newSpring;
        }
    }
}
