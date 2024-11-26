using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPhysics : MonoBehaviour
{
    [SerializeField] BuildingPhysics[] buildingsPhysics;

    public bool isAnim;
    private Rigidbody rb;
    public float structureResistance = 100f;
    public float earthquakeMagnitude = 5f; 
    public BuildingMaterial buildingMaterial;

    [SerializeField] MeshRenderer thisMeshrenderer;

    public float blockHeight; 
    float impactThreshold = 15f; 
    private bool isSimulating = false; 
    private bool isJoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lambda yerine bağımsız metot
        BlockPhysicsEvents.OnEarthquake += HandleEarthquake;
        BlockPhysicsEvents.OnPower += EarthquakeMagnitude;

        if (!GetComponent<Joint>())
        {
            impactThreshold = 8;
        }
    }

    private void OnDisable()
    {
        BlockPhysicsEvents.OnEarthquake -= HandleEarthquake;
        BlockPhysicsEvents.OnPower -= EarthquakeMagnitude;
    }

    private void OnDestroy()
    {
        BlockPhysicsEvents.OnEarthquake -= HandleEarthquake;
        BlockPhysicsEvents.OnPower -= EarthquakeMagnitude;
    }

    private void HandleEarthquake()
    {
        StartCoroutine(SimulateEarthquakeSequence(10, 0.9f, 4, 0.3f));
    }

    private void OnJointBreak(float breakForce)
    {
        if (breakForce == 0) return;
        Debug.Log("Joint kopması algılandı! Kopma kuvveti: " + breakForce + "  -  " + name);

        // Kopma gerçekleşirse alt elemanları tetikle
        isJoint = true;
        TriggerFire();
    }

    IEnumerator SimulateEarthquakeSequence(int repetitions, float delayBetweenReps, int hitsPerRepetition, float hitDuration)
    {
        if (!isAnim || isSimulating) yield break;

        isSimulating = true;

        yield return new WaitForSeconds(5f);

        float currentMagnitude = earthquakeMagnitude * 0.5f; // Deprem şiddetinin başlangıcı (yarısı)
        float magnitudeStep = (earthquakeMagnitude - currentMagnitude) / repetitions; // Her turda artış miktarı

        //10 = 10000
        Debug.Log($"Blok tetiklendi! : {earthquakeMagnitude} {name}, Şiddet: {currentMagnitude}");

        for (int i = 0; i < repetitions; i++)
        {
            // Blok yüksekliği ve şiddete bağlı kontrol
            if (currentMagnitude >= structureResistance / blockHeight)
            {
                TriggerFire(); // Elemanları tetikle
            }

            for (int j = 0; j < hitsPerRepetition; j++)
            {
                yield return StartCoroutine(ApplyForceOverTime(hitDuration, currentMagnitude));
            }

            yield return new WaitForSeconds(delayBetweenReps);

            currentMagnitude = Mathf.Min(currentMagnitude + magnitudeStep, earthquakeMagnitude); // Maksimum şiddeti aşmamalı
        }

        isSimulating = false;
    }

    IEnumerator ApplyForceOverTime(float duration, float currentMagnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Vector3 earthquakeForce = Random.insideUnitSphere * currentMagnitude;
            rb.AddForce(earthquakeForce, ForceMode.Force);
            rb.AddTorque(earthquakeForce, ForceMode.Force);

            elapsedTime += Time.deltaTime;
            yield return null; // Bir frame bekle
        }
    }

    void EarthquakeMagnitude(float value)
    {
        earthquakeMagnitude = value;
        earthquakeMagnitude *= 1.25f;
    }

    void TriggerFire()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        foreach (BuildingPhysics child in buildingsPhysics)
        {
            int rndm = Random.Range(0, 10);
            if (rndm <= 4)
            if (child != null)
            {
                child.BuildingDeform();
                child.BuildingCollapse();
            }

            if (isJoint)
                child.BuildingDeform();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= impactThreshold)
        {
            TriggerFire();
        }
    }
}
