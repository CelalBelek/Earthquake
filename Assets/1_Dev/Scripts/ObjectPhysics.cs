using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float structureResistance = 100f;
    public float earthquakeMagnitude = 5f;
    public BuildingMaterial buildingMaterial;

    public MeshRenderer thisObj;
    public GameObject[] thisChildObj;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = rb.mass * rb.velocity.magnitude;

            if (impactForce > 0.0f)
            {
                // Destroy(collision.gameObject); // Alttaki objeyi kır
                Debug.Log($"{collision.gameObject.name} has been destroyed by impact.");
                thisObj.enabled = false;

                foreach (var item in thisChildObj)
                {
                    item.SetActive(true);
                }
            }
    }
}
