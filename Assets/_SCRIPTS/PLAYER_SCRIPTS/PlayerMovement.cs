using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float speed = 10f;
    public float laneSwitchSpeed = 10f;

    [Header("Sliding")]
    public float slideDuration = 1.0f; // Bu satırı geri ekliyoruz!

    [Header("Jumping")]
    public float jumpForce = 8f;

    [Header("Lane Settings")]
    public float laneDistance = 2.5f;

    private int targetLane = 1;
    private bool isGrounded = true;
    private bool isSliding = false;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = capsuleCollider.height;
        originalColliderCenter = capsuleCollider.center;

        // DEĞİŞİKLİK: Animator artık alt objede değil, bu objenin kendisinde.
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { MoveLane(false); }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { MoveLane(true); }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding) { Jump(); }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded && !isSliding)
            {
                StartSlide();
            }
        }
    }

    void HandleMovement()
    {
        // DÜZELTME: targetLane değişkenini kullanan bu satırlar, uyarıyı giderecek.
        float targetXPosition = (targetLane - 1) * laneDistance;
        float xVelocity = (targetXPosition - rb.position.x) * laneSwitchSpeed;
        rb.linearVelocity = new Vector3(xVelocity, rb.linearVelocity.y, speed);
    }

    void MoveLane(bool goingRight)
    {
        targetLane += goingRight ? 1 : -1;
        targetLane = Mathf.Clamp(targetLane, 0, 2);
    }

    void Jump()
    {
        isGrounded = false;
        animator.SetTrigger("Jump");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    void StartSlide()
    {
        isSliding = true;
        animator.SetBool("IsSliding", true);
        capsuleCollider.height = originalColliderHeight / 2f;
        capsuleCollider.center = originalColliderCenter / 2f;

        // Coroutine'i tekrar buradan başlatıyoruz.
        StartCoroutine(StopSlide());
    }

    // Bu fonksiyon artık Animasyon Olayı tarafından görülecek!
    IEnumerator StopSlide()
    {
        // slideDuration değişkenimizle animasyonun süresini senkronize ediyoruz.
        yield return new WaitForSeconds(slideDuration);

        // Eğer hala kayıyorsak (başka bir şey bizi durdurmadıysa)
        if (isSliding)
        {
            isSliding = false;
            animator.SetBool("IsSliding", false);
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle") { GameManager.instance.GameOver(); this.enabled = false; }
        if (collision.gameObject.tag == "Ground") { isGrounded = true; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Collectible") { GameManager.instance.AddScore(10); Destroy(other.gameObject); }
    }
}