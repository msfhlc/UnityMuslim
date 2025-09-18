using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    private Vector3 offset;

    // YENÝ: Kameranýn Rigidbody bileþenini tutacaðýmýz referans.
    private Rigidbody rb;

    void Start()
    {
        // YENÝ: Baþlangýçta Rigidbody bileþenini alýyoruz.
        rb = GetComponent<Rigidbody>();

        offset = transform.position - player.position;
    }

    // DEÐÝÞÝKLÝK: Tüm hareket mantýðýný LateUpdate'ten FixedUpdate'e taþýdýk.
    // Artýk hem oyuncu hem de kamera ayný fizik saatinde güncelleniyor.
    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;

        // DEÐÝÞÝKLÝK: transform.position yerine, Rigidbody'nin pozisyonunu yumuþakça güncelliyoruz.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kinematic bir Rigidbody'yi hareket ettirmenin en doðru yolu MovePosition'dýr.
        rb.MovePosition(smoothedPosition);
    }
}