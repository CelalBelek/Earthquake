using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween kütüphanesini dahil et

public enum BuildingMaterial
{
    Wood,
    Steel,
    Concrete
}

public class BuildingPhysics : MonoBehaviour
{
    public float structureResistance = 100f;
    public float earthquakeMagnitude = 5f;
    public BuildingMaterial buildingMaterial;

    [SerializeField] MeshRenderer thisMeshrenderer;
    [SerializeField] GameObject[] thisChilds;
    [SerializeField] GameObject[] thisChildsDrop;

    bool isCollapse;
    bool isDeform;
    bool isDrop;

    void Start()
    {
       
    }

    public void BuildingDeform()
    {
        if (!thisMeshrenderer) return;
        if (thisChilds.Length <= 0) return;
        if (thisChildsDrop.Length <= 0) return;
        if (isDeform) return;
        if (isDrop) return;

        thisMeshrenderer.enabled = false;

        foreach (GameObject item in thisChilds)
        {
            item.gameObject.SetActive(true);

            Rigidbody currentRb = item.GetComponent<Rigidbody>();

            if (currentRb)
            {
                currentRb.isKinematic = true;
                currentRb.useGravity = false;
            }

            item.GetComponent<BoxCollider>().enabled = false;
        }

        foreach (GameObject item in thisChildsDrop)
        {
            item.transform.parent = null;
            item.gameObject.SetActive(true);

            Rigidbody currentRb = item.GetComponent<Rigidbody>();

            if (currentRb)
            {
                currentRb.isKinematic = false;
                currentRb.useGravity = true;
            }

            item.GetComponent<BoxCollider>().enabled = true;
        }

        isDeform = true;
        isCollapse = true;
    }

    public void BuildingCollapse()
    {
        if (thisChilds.Length <= 0) return;
        if (!isCollapse) return;
        if (isDrop) return;
        isDrop = true;

        this.GetComponent<BoxCollider>().enabled = false;

        foreach (GameObject item in thisChilds)
        {
            item.transform.parent = null;
            item.gameObject.SetActive(true);

            Rigidbody currentRb = item.GetComponent<Rigidbody>();

            if (currentRb)
            {
                currentRb.isKinematic = false;
                currentRb.useGravity = true;
            }

            item.GetComponent<BoxCollider>().enabled = true;
        }

    }



  
}
