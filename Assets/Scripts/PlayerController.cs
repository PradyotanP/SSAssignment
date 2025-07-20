using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float strafeSpeed = 5f;
    public float acceleration = 10f;

    [Header("Jump")]
    public float jumpForce = 20f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.6f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool jumpRequested;
    private float lastJumpTime;
    private float jumpCooldown = 0.1f;

    [Header("Joystick Input")]
    public FloatingJoystick joystick;
    public float deadZone = 0.1f;

    [Header("Sound")]
    public AudioClip jumpClip;
    public AudioClip hitClip;
    public AudioClip bgmClip;
    private AudioSource sfxSource;
    private AudioSource bgmSource;

    [Header("VFX")]
    public ParticleSystem movementVFX;
    public GameObject deathVFX;

    [Header("Control Flags")]
    public bool isControllable = true;
    public bool isDead;

    private Rigidbody rb;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Audio setup
        sfxSource = gameObject.AddComponent<AudioSource>();
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.volume = 0.4f;
        bgmSource.Play();
    }

    void Update()
    {
        if (!isControllable) return;

        CheckGround();

//#if UNITY_EDITOR
//        HandleEditorInput();
//        if (Keyboard.current.spaceKey.wasPressedThisFrame)
//        {
//            Jump();
//        }
//#else
//    HandleMobileInput();
//#endif

        HandleMobileInput();

        // Handle jump
        if (jumpRequested && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            sfxSource.PlayOneShot(jumpClip);
            jumpRequested = false;
            lastJumpTime = Time.time;
        }

        // Movement VFX
        if (movementVFX != null)
        {
            if (inputDirection.magnitude > 0.1f && isGrounded)
            {
                if (!movementVFX.isPlaying)
                    movementVFX.Play();
            }
            else
            {
                if (movementVFX.isPlaying)
                    movementVFX.Stop();
            }
        }

        // Fall off
        if (transform.position.y < -5f)
        {
            FindObjectOfType<GameManager>().EndRun();
        }
    }


    void FixedUpdate()
    {
        if (!isControllable) return;

        Vector3 targetVelocity = inputDirection * maxSpeed;
        Vector3 velocityChange = targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(new Vector3(velocityChange.x, 0, velocityChange.z) * acceleration, ForceMode.Acceleration);
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckRadius + 0.1f, groundLayer);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
        }
    }

    public void SetControllable(bool state) => isControllable = state;

    private void HandleMobileInput()
    {
        if (joystick != null)
        {
            Vector3 input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
            inputDirection = input.magnitude < deadZone ? Vector3.zero : input.normalized;
        }
        else
        {
            inputDirection = Vector3.zero;
        }
    }


    private void HandleEditorInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputDirection = new Vector3(h, 0, v).normalized;
    }

    public void FreezePlayer(bool freeze)
    {
        rb.isKinematic = freeze;
        isControllable = !freeze;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isDead)
        {
            sfxSource.PlayOneShot(hitClip);
            isDead = true;

            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }

            FindObjectOfType<GameManager>().EndRun();
        }
    }
}
