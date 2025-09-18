using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float spinSpeed = 100f;

    void Update()
    {
        // Bu objeyi Y ekseninde (kendi etraf�nda) s�rekli d�nd�r.
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}