using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tree : MonoBehaviour
{
    [SerializeField] private GameObject fireLittle;
    [SerializeField] private GameObject fireBig;
    [SerializeField] private List<Tree> otherTrees;
    [SerializeField] private Vector3 direction; // Yön açısı (örneğin, 90 derece)
    [SerializeField] MeshRenderer[] meshRender;

    float earthquakeMagnitude = 5f;
    bool isSimulating;
    bool isBurn = false;

    private void Awake()
    {
        fireLittle.SetActive(false);
        fireBig.SetActive(false);

        fireLittle.transform.localScale = Vector3.zero;

        FireEvents.OnFire += Fire;
        FireEvents.OnWindDirection += WindDirection;
        BlockPhysicsEvents.OnEarthquake += HandleEarthquake;
        BlockPhysicsEvents.OnPower += EarthquakeMagnitude;
    }

    private void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 8f);

        foreach (Collider col in colliders)
        {
            Tree tree = col.GetComponent<Tree>();
            if (tree != null && tree.transform != transform)
            {
                otherTrees.Add(tree);
            }
        }
    }

    private void OnDestroy()
    {
        FireEvents.OnFire -= Fire;
        FireEvents.OnWindDirection -= WindDirection;
        BlockPhysicsEvents.OnEarthquake -= HandleEarthquake;
        BlockPhysicsEvents.OnPower -= EarthquakeMagnitude;
    }

    // Depremi tetikleyen bağımsız fonksiyon
    private void HandleEarthquake()
    {
         StartCoroutine(SimulateEarthquakeSequence(10, 0.9f, 4, 0.3f));
    }

    private void OnDisable()
    {
        FireEvents.OnFire -= Fire;
        FireEvents.OnWindDirection -= WindDirection;

        BlockPhysicsEvents.OnEarthquake -= () => StartCoroutine(SimulateEarthquakeSequence(10, 0.9f, 4, 0.3f)); 
        BlockPhysicsEvents.OnPower -= EarthquakeMagnitude; 
    }

    void WindDirection(Vector3 _direction)
    {
        direction = _direction.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        fireBig.transform.DORotate(targetRotation.eulerAngles, 0.5f);
    }

    public void Fire()
    {
        if (isBurn) return;

        fireLittle.SetActive(true);

        fireLittle.transform.DOScale(Vector3.one, 3f).OnComplete(() =>
        {
            fireBig.SetActive(true);
            isBurn = true;
            DOVirtual.DelayedCall(1f, () =>
            {
                fireLittle.SetActive(false);

                // 5 saniye sonra diğer ağaçlara ateşi yönlendir
                DOVirtual.DelayedCall(5f, () =>
                {
                    if (otherTrees != null)
                    {
                        foreach (Tree item in otherTrees)
                        {
                            if (IsInDirection(item.transform))
                            {
                                item.Fire();
                            }
                        }
                    }

                    DOVirtual.DelayedCall(5f, () =>
                    {
                        // Ateşi küçült ve devre dışı bırak
                        fireBig.transform.DOScale(Vector3.zero, 3f).OnComplete(() =>
                        {
                            fireBig.SetActive(false);
                        });

                        // MeshRenderer'ları kontrol et
                        if (meshRender.Length > 0)
                        {
                            foreach (MeshRenderer item in meshRender)
                            {
                                // Her bir malzeme üzerinde işlem yap
                                Material[] materials = item.materials;
                                for (int i = 0; i < materials.Length; i++)
                                {
                                    materials[i].SetColor("_Color", Color.black);
                                }
                            }
                        }
                    });
                });
            });
        });
    }

    private bool IsInDirection(Transform otherTree)
    {
        Vector3 toOtherTree = (otherTree.position - transform.position).normalized;
        Vector3 normalizedDirection = direction.normalized;
        float dotProduct = Vector3.Dot(normalizedDirection, toOtherTree);
        float tolerance = 0.5f;
        return dotProduct >= tolerance;
    }

    // Depremi başlat
    IEnumerator SimulateEarthquakeSequence(int repetitions, float delayBetweenReps, int hitsPerRepetition, float hitDuration)
    {
        if (isSimulating) yield break;

        isSimulating = true;

        yield return new WaitForSeconds(5f);

        float currentMagnitude = earthquakeMagnitude * 0.5f; // Deprem şiddetinin başlangıcı (yarısı)
        float magnitudeStep = (earthquakeMagnitude - currentMagnitude) / repetitions; // Her turda artış miktarı

        Vector3 originalRotation = transform.eulerAngles; // Nesnenin başlangıç rotasyonu
        float shakeDuration = 0.4f; // Sallanma süresi
        float shakeAngle = 3f; // Sallanma açısı

        Debug.Log($"Deprem simülasyonu başladı! Şiddet: {currentMagnitude}");

        for (int i = 0; i < repetitions; i++)
        {
            Vector3 randomShake = new Vector3(
                Random.Range(-shakeAngle, shakeAngle),
                0,
                Random.Range(-shakeAngle, shakeAngle)
            );

            transform.DORotate(originalRotation + randomShake / 10, shakeDuration / 2).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.DORotate(originalRotation, shakeDuration / 2).SetEase(Ease.InOutBounce);
            }).SetLoops(hitsPerRepetition * 2);

            yield return new WaitForSeconds(delayBetweenReps);

            currentMagnitude = Mathf.Min(currentMagnitude + magnitudeStep, earthquakeMagnitude); 
        }

        isSimulating = false;
        Debug.Log("Deprem simülasyonu tamamlandı.");
    }

    // IEnumerator SimulateEarthquakeSequence()
    // {
    //     if (isSimulating) yield break;

    //     isSimulating = true;

    //     float shakeDuration = 0.4f; 
    //     int shakeCount = 10; 
    //     Vector3 originalRotation = transform.eulerAngles; 

    //     for (int i = 0; i < shakeCount; i++)
    //     {
    //         float shakeAngle = 3f;
    //         Vector3 randomShake = new Vector3(
    //             Random.Range(-shakeAngle, shakeAngle),
    //             0,
    //             Random.Range(-shakeAngle, shakeAngle)
    //         );

    //         transform.DORotate(originalRotation + randomShake / 9, shakeDuration / 2).SetEase(Ease.InOutSine).OnComplete(() =>
    //         {
    //             transform.DORotate(originalRotation, shakeDuration / 2).SetEase(Ease.InOutSine);
    //         });

    //         yield return new WaitForSeconds(shakeDuration);
    //     }

    //     isSimulating = false;
    // }

    void EarthquakeMagnitude(float value)
    {
        earthquakeMagnitude = value;
    }
}
