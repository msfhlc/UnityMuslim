using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonSimple : MonoBehaviour
{
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float rotationSpeed = 12f;

    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    private CharacterController cc;
    private Animator anim;
    private Vector3 velocity;

    static readonly int HashSpeed = Animator.StringToHash("Speed");     // Blend Tree paramý (0, 0.5, 1)
    static readonly int HashIsJumping = Animator.StringToHash("IsJumping"); // Jump state paramý (bool)

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>(); // SkinnedMesh çocukta ise
    }

    void Update()
    {
        // --- 1) Input (WASD), kameraya göre yön
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 camF = Camera.main.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = Camera.main.transform.right; camR.y = 0; camR.Normalize();
        Vector3 moveDir = camF * input.z + camR * input.x;

        // --- 2) Hýz seçimi (Shift = Run)
        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = running ? runSpeed : walkSpeed;

        // --- 3) Döndürme (hareket varsa)
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // --- 4) Yer çekimi + zýplama
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * -gravity * jumpHeight);
            anim.SetBool(HashIsJumping, true);
        }
        velocity.y += gravity * Time.deltaTime;

        // --- 5) Hareket uygula
        Vector3 horizontal = moveDir * targetSpeed;
        cc.Move((horizontal + velocity) * Time.deltaTime);

        // --- 6) Animator parametreleri
        // Blend Tree eþikleri: 0=Idle, 0.5=Walk, 1=Run
        float speedParam = 0f;
        if (moveDir.sqrMagnitude > 0.01f)
            speedParam = running ? 1f : 0.5f;

        anim.SetFloat(HashSpeed, speedParam);

        // Jump'tan iniþte kapat
        if (anim.GetBool(HashIsJumping) && cc.isGrounded && velocity.y <= 0.01f)
            anim.SetBool(HashIsJumping, false);
    }
}
