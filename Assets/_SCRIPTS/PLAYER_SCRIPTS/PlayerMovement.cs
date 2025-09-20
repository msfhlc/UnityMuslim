using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float speed = 10f;
    public float laneSwitchSpeed = 10f;

    [Header("Jumping")]
    public float jumpForce = 8f;

    [Header("Sliding")]
    public float slideDuration = 1.0f;

    [Header("Lane Settings")]
    public float laneDistance = 2.5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    // --- Private Değişkenler ---
    private int targetLane = 1;
    private bool isGrounded;
    private bool isSliding = false;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private Animator animator;
    private bool isStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = capsuleCollider.height;
        originalColliderCenter = capsuleCollider.center;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isStopped) return;
        CheckIfGrounded();
        HandleInput();
        animator.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        if (isStopped) return;
        Debug.Log("FixedUpdate HAREKET UYGULUYOR! Hız: " + rb.linearVelocity);
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
        float targetXPosition = (targetLane - 1) * laneDistance;
        float xVelocity = (targetXPosition - rb.position.x) * laneSwitchSpeed;

        // DÜZELTME: Her yerde "velocity" kullandığımızdan emin oluyoruz.
        rb.linearVelocity = new Vector3(xVelocity, rb.linearVelocity.y, speed);
    }

    void MoveLane(bool goingRight)
    {
        targetLane += goingRight ? 1 : -1;
        targetLane = Mathf.Clamp(targetLane, 0, 2);
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        // DÜZELTME: Her yerde "velocity" kullandığımızdan emin oluyoruz.
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    void StartSlide()
    {
        isSliding = true;
        animator.SetBool("IsSliding", true);
        capsuleCollider.height = originalColliderHeight / 2f;
        capsuleCollider.center = originalColliderCenter / 2f;
        StartCoroutine(StopSlide());
    }

    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(slideDuration);
        if (isSliding)
        {
            isSliding = false;
            animator.SetBool("IsSliding", false);
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }

    void CheckIfGrounded()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void StopRunning()
    {
        isStopped = true;
        // DÜZELTME: Her yerde "velocity" kullandığımızdan emin oluyoruz.
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("isStopped", true);
    }

    public void ResumeRunning()
    {
        Debug.Log("ResumeRunning ÇAĞRILDI! isStopped false yapılıyor.");
        isStopped = false;
        animator.SetBool("isStopped", false);
        // Bu satır zaten doğruydu, "velocity" kullanıyordu.
        rb.linearVelocity = new Vector3(0, 0, speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            GameManager.instance.GameOver();
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Collectible")
        {
            GameManager.instance.AddScore(10);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "QuizGate")
        {
            StopRunning();
            QuizManager.instance.StartQuiz(other.gameObject);
        }
    }
}