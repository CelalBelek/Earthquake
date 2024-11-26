using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Floor : MonoBehaviour
{
    [SerializeField] Transform[] colons;
    void Start()
    {
        foreach(Transform colon in colons){
            SpringJoint joint = this.gameObject.GetComponent<SpringJoint>();
                joint.enableCollision = true;
                joint.connectedBody = colon.GetComponent<Rigidbody>();
                joint.spring = EarthquakeEffect.Instance.amplitude * 100f;
                joint.damper = 10f;
        }
    }
}
