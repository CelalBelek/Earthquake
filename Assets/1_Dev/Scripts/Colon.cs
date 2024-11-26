using UnityEngine;

public class Colon : MonoBehaviour
{
    Rigidbody groundRB;
    Rigidbody colon;
    float maxMass = 500f; // Alt kat kolonlarının maksimum ağırlığı
    float minMass = 50f;  // Üst kat kolonlarının minimum ağırlığı
    float maxBreakForce = 20000f; // Alt kat kolonlarının maksimum kopma kuvveti
    float minBreakForce = 2000f;  // Üst kat kolonlarının minimum kopma kuvveti

    void Start()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        // Mevcut Joint'leri temizle
        ConfigurableJoint[] existingJoints = this.GetComponents<ConfigurableJoint>();
        foreach (var joint in existingJoints)
        {
            // Yeni Joint ekle
            joint.enableCollision = true;

            // Lineer hareket sınırları
            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Locked; // Yatay eksen
            joint.zMotion = ConfigurableJointMotion.Limited;

            // Açısal hareket sınırları
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            // Mesafeye göre ağırlık ve sağlamlık hesapla
            float distanceFromGround = transform.position.y; // Zeminden yükseklik
            float normalizedHeight = Mathf.InverseLerp(0, 10, distanceFromGround); // 0-1 arasında normalize et (10: bina yüksekliği)

            // Ağırlık ayarı
            rb.mass = Mathf.Lerp(maxMass, minMass, normalizedHeight) * 6;

            // Kopma kuvveti ayarı
            float breakForce = Mathf.Lerp(maxBreakForce, minBreakForce, normalizedHeight);
            float breakTorque = Mathf.Lerp(maxBreakForce / 2, minBreakForce / 2, normalizedHeight);

            // Joint limitleri
            JointDrive drive = new JointDrive
            {
                positionSpring = 2000f, // Esneme kuvveti
                positionDamper = 200f,  // Sönümleme
                maximumForce = 10000f    // Maksimum kuvvet
            };
            joint.xDrive = drive;
            joint.zDrive = drive;

            joint.breakForce = breakForce * 6;
            joint.breakTorque = breakTorque * 6;

            joint.hideFlags = HideFlags.None; 

            Debug.Log($"Colon setup: Mass = {rb.mass}, BreakForce = {breakForce}, BreakTorque = {breakTorque}");
        }
    }
}
