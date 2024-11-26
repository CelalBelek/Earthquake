using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPhysics : MonoBehaviour
{
    [SerializeField] BuildingPhysics[] buildingsPhysics;

    public bool isAnim;
    private Rigidbody rb;
    public float structureResistance = 100f;
    public float earthquakeMagnitude = 5f; // Maksimum deprem şiddeti
    public BuildingMaterial buildingMaterial;

    [SerializeField] MeshRenderer thisMeshrenderer;

    public float blockHeight; // Kat yüksekliği
    float impactThreshold = 15f; // Çarpma şiddeti eşiği
    private bool isSimulating = false; // Simülasyonun çalışıp çalışmadığını kontrol eder
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
        Debug.Log($"Blok tetiklendi! Yükseklik: {earthquakeMagnitude} {name}, Şiddet: {currentMagnitude}");

        for (int i = 0; i < repetitions; i++)
        {
            // Blok yüksekliği ve şiddete bağlı kontrol
            if (currentMagnitude >= structureResistance / blockHeight)
            {
                TriggerFire(); // Elemanları tetikle
            }

            for (int j = 0; j < hitsPerRepetition; j++)
            {
                // Her vuruş sırasında kısa süreli kuvvet uygula
                yield return StartCoroutine(ApplyForceOverTime(hitDuration, currentMagnitude));
            }

            // Turlar arasında bekle
            yield return new WaitForSeconds(delayBetweenReps);

            // Deprem şiddetini artır
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
    }

    void TriggerFire()
    {
        Debug.Log("TriggerFire tetiklendi: " + name);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        // Eğer alt elemanlar varsa, onları tetikle
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
        // Çarpışma şiddetini hesapla
        float impactForce = collision.relativeVelocity.magnitude;

        Debug.Log($"Çarpışma algılandı! Şiddet: {impactForce}, Eşik: {impactThreshold}, Bu: {name}, Nesne: {collision.gameObject.name}");

        // Çarpışma şiddeti eşiği aşarsa TriggerFire'i tetikle
        if (impactForce >= impactThreshold)
        {
            TriggerFire();
        }
    }
}
