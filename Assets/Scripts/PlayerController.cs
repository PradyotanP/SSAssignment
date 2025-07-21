using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement Settings
    [Header("Movement")]
    public float maxSpeed = 8f;           // Maximum movement speed
    public float strafeSpeed = 5f;        // (Unused in current script)
    public float acceleration = 10f;      // Acceleration rate

    // Jump Settings
    [Header("Jump")]
    public float jumpForce = 20f;         // Force applied when jumping
    public Transform groundCheck;         // Ground check reference point (not used here directly)
    public float groundCheckRadius = 0.6f;// Distance to check for the ground
    public LayerMask groundLayer;         // Layer considered as ground
    private bool isGrounded;              // Whether the player is grounded
    private bool jumpRequested;           // Flag to handle jump input
    private float lastJumpTime;           // Time of the last jump
    private float jumpCooldown = 0.1f;    // Cooldown between jumps

    // Joystick Input
    [Header("Joystick Input")]
    public FloatingJoystick joystick;     // Reference to the floating joystick
    public float deadZone = 0.1f;         // Threshold to ignore small inputs

    // Audio Settings
    [Header("Sound")]
    public AudioClip jumpClip;            // Jump sound effect
    public AudioClip hitClip;             // Hit/death sound effect
    public AudioClip bgmClip;             // Background music clip
    private AudioSource sfxSource;        // Source for playing sound effects
    private AudioSource bgmSource;        // Source for playing background music

    // Visual Effects
    [Header("VFX")]
    public ParticleSystem movementVFX;    // Dust or movement effect
    public GameObject deathVFX;           // Visual effect on death

    // Player State Flags
    [Header("Control Flags")]
    public bool isControllable = true;    // Whether the player can move
    public bool isDead;                   // Whether the player is dead

    private Rigidbody rb;                 // Rigidbody component
    private Vector3 inputDirection;       // Input direction for movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set up audio sources for SFX and BGM
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

        CheckGround();  // Update grounded status

        HandleMobileInput(); // Use joystick input on mobile

        // Handle jump if requested
        if (jumpRequested && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            sfxSource.PlayOneShot(jumpClip); // Play jump sound
            jumpRequested = false;
            lastJumpTime = Time.time;
        }

        // Control movement particle effect
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

        // Trigger game over if player falls below a threshold
        if (transform.position.y < -5f)
        {
            FindObjectOfType<GameManager>().EndRun();
        }
    }

    void FixedUpdate()
    {
        if (!isControllable) return;

        // Smooth movement physics using force
        Vector3 targetVelocity = inputDirection * maxSpeed;
        Vector3 velocityChange = targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(new Vector3(velocityChange.x, 0, velocityChange.z) * acceleration, ForceMode.Acceleration);
    }

    // Check if player is on the ground
    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckRadius + 0.1f, groundLayer);
    }

    // Public method to trigger a jump
    public void Jump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
        }
    }

    // Control whether the player can move
    public void SetControllable(bool state) => isControllable = state;

    // Handle input from mobile joystick
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

    // Optional: Handle keyboard input for editor testing
    private void HandleEditorInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputDirection = new Vector3(h, 0, v).normalized;
    }

    // Freeze/unfreeze the player movement (for replay)
    public void FreezePlayer(bool freeze)
    {
        rb.isKinematic = freeze;
        isControllable = !freeze;
    }

    // Handle collision with obstacles
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isDead)
        {
            sfxSource.PlayOneShot(hitClip); // Play death sound
            isDead = true;

            // Play death VFX if assigned
            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }

            // End the run
            FindObjectOfType<GameManager>().EndRun();
        }
    }
}
