using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform target; // Hedef pozisyonu
    public float speed = 5f; // Hareket hızı
    public float stoppingDistance = 1f; // Hedefe yaklaşma mesafesi
    public float decelerationRate = 1f; // Hız azalması oranı
    public float stopTurnAngle = 10f; // Dururken hafif sağa dönüş açısı
    public Light[] frontLights; // Ön farlar
    public Light[] stopLights; // Stop lambaları
    public Light[] signalLights; // Sinyal lambaları

    private bool isFiring = false;
    private bool isSignal = false;

    private bool isStopping = false; // Araç yavaşlama durumunda mı?
    private float currentSpeed; // Anlık hız

    private void Start()
    {
        BlockPhysicsEvents.OnEarthquake += HandleEarthquake; // Deprem tetiklenince başla
    }

    private void OnDisable()
    {
        BlockPhysicsEvents.OnEarthquake -= HandleEarthquake;
    }

    void Update()
    {
        if (isFiring && !isSignal)
        {
            HandleSignalLights();
        }
        else if (target != null)
        {
            MoveTowardsTarget();
        }
        else if (!isStopping)
        {
            StopCar(false); // Başlangıçta target null ise stop ışıkları yanmaz
        }
    }

    IEnumerator SimulateEarthquakeSequence()
    {
        currentSpeed = speed; // Başlangıç hızı

        yield return new WaitForSeconds(5f);
        isFiring = true;
        StopCar(true);
        target = null;
    }

    private void HandleEarthquake()
    {
        StartCoroutine(SimulateEarthquakeSequence());
    }

    void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stoppingDistance)
        {
            // Hedefe doğru hareket
            transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
            EnableFrontLights(true);
            EnableStopLights(false);
        }
        else
        {
            // Hedefe ulaştı, dur
            StopCar(true);
        }
    }

    void StopCar(bool gradualStop)
    {
        if (target == null) return;
        
        if (gradualStop)
        {
            if (!isStopping)
            {
                StartCoroutine(GradualStop()); // Kademeli durma işlemi
            }
        }
        else
        {
            EnableFrontLights(false);
            EnableStopLights(false); // Stop lambaları yanmaz
        }
    }

    IEnumerator GradualStop()
    {
        isStopping = true;

        while (currentSpeed > 0)
        {
            // Hafif sağa çekme
            transform.Rotate(Vector3.down, stopTurnAngle * Time.deltaTime);

            // Hız azaltma
            currentSpeed -= decelerationRate * Time.deltaTime;

            // Araç hareketi
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

            EnableFrontLights(false);
            EnableStopLights(true);

            yield return null;
        }

        currentSpeed = 0;
        isStopping = false;
    }

    void HandleSignalLights()
    {
        StartCoroutine(BlinkSignalLights());
    }

    IEnumerator BlinkSignalLights()
    {
        isSignal = true;
        while (isFiring)
        {
            foreach (Light light in signalLights)
            {
                light.enabled = !light.enabled;
            }
            yield return new WaitForSeconds(0.5f); // 0.5 saniyede bir yanıp sönme
        }

        foreach (Light light in signalLights)
        {
            light.enabled = false; // Fire durunca sinyaller kapanır
        }
        isSignal = false;
    }

    void EnableFrontLights(bool enable)
    {
        foreach (Light light in frontLights)
        {
            light.enabled = enable;
        }
    }

    void EnableStopLights(bool enable)
    {
        foreach (Light light in stopLights)
        {
            light.enabled = enable;
        }
    }

    public void TriggerFire(bool fire)
    {
        isFiring = fire;
    }
}
