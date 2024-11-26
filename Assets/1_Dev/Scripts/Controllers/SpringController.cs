using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    void Start()
    {
        SpringJoint[] existingJoints = this.GetComponents<SpringJoint>();
        foreach (var joint in existingJoints)
        {
            joint.spring = 3000;
            joint.damper = 10;
            joint.minDistance = 0;
            joint.maxDistance = 1;
            joint.breakForce = 300000;
            joint.breakTorque = 200000;
        }
    }
}
