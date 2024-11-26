using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircularRotation : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform rotatorImage; // Daire şeklindeki image (Rotator)
    [SerializeField] private RectTransform targetImage;  // Döndürmek istediğiniz image (TargetImage)

    private Vector2 centerPoint; // Dairenin merkez noktası
    private float currentAngle;  // Şu anki açı
    private float startAngle;    // Sürükleme başladığında başlangıç açısı

    private void Start()
    {
        // Rotator'un merkez noktasını hesapla
        centerPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, rotatorImage.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Sürükleme başladığında başlangıç açısını hesapla
        Vector2 pointerPosition = eventData.position;
        startAngle = GetAngle(pointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sürükleme sırasında açıyı hesapla
        Vector2 pointerPosition = eventData.position;
        float newAngle = GetAngle(pointerPosition);

        // Rotator'un toplam dönüş açısını güncelle
        float deltaAngle = Mathf.DeltaAngle(startAngle, newAngle);
        currentAngle += deltaAngle;
        startAngle = newAngle;

        // Hedef image'i döndür
        if (targetImage != null)
        {
            targetImage.rotation = Quaternion.Euler(0, 0, -currentAngle); // UI'de saat yönünün tersine döner
        }

        Debug.Log($"Current Angle: {currentAngle}");
    }

    private float GetAngle(Vector2 pointerPosition)
    {
        // Pointer'ın merkez noktasına göre açısını hesapla
        Vector2 direction = pointerPosition - centerPoint;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
