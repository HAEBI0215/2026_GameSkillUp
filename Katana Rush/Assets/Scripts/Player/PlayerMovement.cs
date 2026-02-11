using UnityEngine;
using static SuperCharacterController;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    public AudioClip jumpClip;

    public Transform cam;

    [Range(0f, 1f)]
    public float footstepVolume = 0.8f;
    [Range(0f, 1f)]
    public float jumpVolume = 1f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

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
    private Vector3 moveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
        AudioClip clip = GetComponent<AudioClip>();
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        bool isMoving = (h != 0 || v != 0);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        anim.SetBool("isRun", isMoving);
        anim.SetBool("isGrounded", isGrounded);

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDir = (camForward * v + camRight * h).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            anim.SetTrigger("Jump");
            PlayJumpSound();
        }

        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = moveDir * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    public void FootR()
    {
        PlayFootstep();
    }
    public void FootL()
    {
        PlayFootstep();
    }
    void PlayFootstep()
    {
        if (audioSource == null) return;
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip, footstepVolume);
    }
    void PlayJumpSound()
    {
        if (audioSource == null) return;
        if (jumpClip == null) return;

        audioSource.PlayOneShot(jumpClip, jumpVolume);
    }
}
