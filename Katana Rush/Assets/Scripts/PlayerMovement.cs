using UnityEngine;
using static SuperCharacterController;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim;

    private bool isGrounded;

    private float h;
    private float v;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        bool isMoving = (h != 0 || v != 0);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        anim.SetBool("isRun", isMoving);
        anim.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            anim.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;

        Vector3 moveVelocity = moveDir * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    public void FootR()
    {
        
    }

    public void FootL()
    {
        
    }
}
