using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    private Vector3 offset;

    // YEN�: Kameran�n Rigidbody bile�enini tutaca��m�z referans.
    private Rigidbody rb;

    void Start()
    {
        // YEN�: Ba�lang��ta Rigidbody bile�enini al�yoruz.
        rb = GetComponent<Rigidbody>();

        offset = transform.position - player.position;
    }

    // DE����KL�K: T�m hareket mant���n� LateUpdate'ten FixedUpdate'e ta��d�k.
    // Art�k hem oyuncu hem de kamera ayn� fizik saatinde g�ncelleniyor.
    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;

        // DE����KL�K: transform.position yerine, Rigidbody'nin pozisyonunu yumu�ak�a g�ncelliyoruz.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kinematic bir Rigidbody'yi hareket ettirmenin en do�ru yolu MovePosition'd�r.
        rb.MovePosition(smoothedPosition);
    }
}