using UnityEngine;
using DG.Tweening;
using TMPro; // DOTween için gerekli kütüphane

public class EarthquakeEffect : MonoBehaviour
{
    public static EarthquakeEffect Instance;
    public TextMeshProUGUI amplitudetext;
    public Rigidbody GetRigidbody;
    public float amplitude = 0.5f; // Deprem gücü
    public float duration = 1.0f; // Deprem süresi
    public float interval = 0.5f; // Her bir hareket süresi
    private Vector3 initialPosition; // Başlangıç pozisyonu
    public bool isShaking = false; // Deprem durumu
    Rigidbody rb;

    private void Awake() {
        Instance = this;
    }

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        AmplitudeCount(0.1f);
    }

    public void TriggerEarthquake()
    {
        if (isShaking) return; // Zaten deprem etkisi aktifse tekrar tetikleme
        isShaking = true;

        // DOTween ile hareket döngüsü
        Sequence earthquakeSequence = DOTween.Sequence();

        earthquakeSequence.Append(rb.DOMove(initialPosition + new Vector3(amplitude, 0, 0), interval).SetEase(Ease.InOutSine)) // Sağa hareket
                          .Append(rb.DOMove(initialPosition - new Vector3(amplitude, 0, 0), interval).SetEase(Ease.InOutSine)) // Sola hareket
                          .Append(rb.DOMove(initialPosition + new Vector3(0, 0, amplitude), interval).SetEase(Ease.InOutSine)) // İleri hareket
                          .Append(rb.DOMove(initialPosition - new Vector3(0, 0, amplitude), interval).SetEase(Ease.InOutSine)) // Geri hareket
                          .SetLoops(Mathf.FloorToInt(duration / (interval * 4))) // Döngü sayısı
                          .OnComplete(() =>
                          {
                              // Deprem bittiğinde pozisyonu sıfırla
                              rb.DOMove(initialPosition, 0.5f).SetEase(Ease.OutQuad);
                              isShaking = false;
                          });

                        // Vector3 randomDirection = new Vector3(Random.Range(-amplitude, amplitude), 0, Random.Range(-amplitude, amplitude));
                        // earthquakeSequence.Append(rb.DOMove(initialPosition + randomDirection, interval).SetEase(Ease.InOutSine));
    }

    public void AmplitudeCount(float value){
        amplitude += value;
        amplitudetext.text = (amplitude * 10f).ToString("F1");
    }
}
