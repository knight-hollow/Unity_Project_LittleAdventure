using UnityEngine;

/// <summary>
/// Main player controller that handles movement, rolling, attacking,
/// damage, health, and UI updates.
/// This script integrates input, physics, and animations.
/// </summary>
public class Player : MonoBehaviour
{
    // ==============================
    // --- Player & Component Setup ---
    // ==============================

    public static Player Instance;        // Singleton instance for easy global access
    public Rigidbody _rb;                 // Rigidbody for physics-based movement
    public Animator _anim;                // Animator for controlling character animations

    // ==============================
    // --- Movement Variables ---
    // ==============================

    public float moveSpeed = 5f;          // Base walking/running speed
    public float horizontalInput;         // Input from A/D or Left/Right keys
    public float verticalInput;           // Input from W/S or Up/Down keys
    public Vector3 inputDirection;        // Directional vector based on input
    public bool ismove = false;           // Whether the player is moving or idle

    // ==============================
    // --- Rolling System ---
    // ==============================

    public bool canRoll = true;           // Whether rolling is available
    public bool isRolling = false;        // Whether the player is currently rolling
    public float rollingTimer = 0f;       // Timer for roll duration tracking
    public float rollingtime = 0.4f;      // Roll lasts for 0.4 seconds
    public float rollColdingTimer = 0f;   // Timer for roll cooldown
    public float rollColdingTime = 1f;    // Roll cooldown (1 second before next roll)

    // ==============================
    // --- Health & Damage System ---
    // ==============================

    public bool isDead = false;           // Player death flag
    public bool isHurt = false;           // Player temporary invulnerability flag
    public float dontHurtTimer = 0f;      // Timer for hurt invulnerability duration
    public float dontHurtTime = 1f;       // 1 second of invulnerability after being hurt
    public float hp = 100f;               // Current HP
    public float maxHp = 100f;            // Maximum HP

    // ==============================
    // --- Miscellaneous States ---
    // ==============================

    public bool isGround = true;          // Whether the player is touching the ground
    public float gravity = -4f;           // Custom gravity (if Rigidbody.useGravity is disabled)
    public float attackPower = 20f;       // Attack damage per hit
    public int coin = 200;                // Player's current coin count
    public BoxCollider _AttackTriggerBox; // Collider used as attack hitbox (activated via animation)

    // ==============================
    // --- Unity Lifecycle ---
    // ==============================

    private void Awake()
    {
        // Setup singleton instance and cache components
        Instance = this;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        // Configure Rigidbody for stable control
        _rb.freezeRotation = true;
        _rb.useGravity = false; // Manual gravity handling
    }

    void Start()
    {
        // Locate the DamageCaster collider for melee hit detection
        _AttackTriggerBox = transform.Find("DamageCaster").GetComponent<BoxCollider>();
    }

    void Update()
    {
        // --- Stop updates when dead ---
        if (isDead) return;

        // --- Attack input ---
        if (Input.GetMouseButtonDown(0))
        {
            _anim.SetTrigger("Attack");
        }

        // --- Handle temporary invulnerability after being hurt ---
        if (isHurt)
        {
            dontHurtTimer += Time.deltaTime;
            if (dontHurtTimer >= dontHurtTime)
            {
                isHurt = false;
                dontHurtTimer = 0;
            }
        }

        // --- Read input ---
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // --- Set animation state based on movement ---
        ismove = inputDirection.magnitude != 0;
        _anim.SetBool("ismove", ismove);

        // --- Adjust input direction to match camera or isometric view ---
        inputDirection = Quaternion.Euler(0, -45f, 0) * inputDirection;

        // --- Face the movement direction ---
        if (inputDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(inputDirection);
        }

        // --- Handle rolling input ---
        if (Input.GetKeyDown(KeyCode.Space) && canRoll)
        {
            canRoll = false;
            isRolling = true;
            _anim.SetTrigger("Roll");
        }

        // --- Rolling duration timer ---
        if (isRolling)
        {
            rollingTimer += Time.deltaTime;
            if (rollingTimer >= rollingtime)
            {
                isRolling = false;
                rollingTimer = 0;
            }
        }

        // --- Rolling cooldown timer ---
        if (!canRoll)
        {
            rollColdingTimer += Time.deltaTime;
            if (rollColdingTimer >= rollColdingTime)
            {
                rollColdingTimer = 0;
                canRoll = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // --- Manual gravity simulation ---
        if (!isGround)
        {
            transform.Translate(0, gravity * Time.deltaTime, 0);
        }

        // --- Movement control ---
        if (isRolling)
        {
            // During roll: move forward at high speed, ignoring normal movement input
            _rb.linearVelocity = transform.forward * moveSpeed * 2.5f;
            return;
        }

        // Standard walking movement
        _rb.linearVelocity = inputDirection * moveSpeed;
    }

    // ==============================
    // --- Combat and Health ---
    // ==============================

    /// <summary>
    /// Applies incoming damage, plays hurt animation, and handles death logic.
    /// </summary>
    public void Hurt(float damage)
    {
        if (isHurt) return; // Skip if temporarily invulnerable

        isHurt = true;
        hp = Mathf.Max(0, hp - damage);

        _anim.SetTrigger("Hurt");
        GameManage.Instance.UpdateHealth();

        if (hp <= 0)
        {
            Dead();
        }
    }

    /// <summary>
    /// Handles player death: plays animation, stops movement, and triggers GameOver UI.
    /// </summary>
    public void Dead()
    {
        isDead = true;
        _anim.SetTrigger("Dead");

        _rb.linearVelocity = Vector3.zero; // Stop movement
        GameManage.Instance.GameOver();
    }

    // ==============================
    // --- Ground Detection ---
    // ==============================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("floor"))
            isGround = true;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("floor"))
            isGround = false;
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("floor"))
            isGround = true;
    }

    // ==============================
    // --- Attack Trigger Control ---
    // ==============================

    /// <summary>
    /// Enables the attack hitbox during attack animation.
    /// </summary>
    public void OpenAttackTrigger()
    {
        _AttackTriggerBox.enabled = true;
    }

    /// <summary>
    /// Disables the attack hitbox (called at the end of the attack animation).
    /// </summary>
    public void CloseAttackTrigger()
    {
        _AttackTriggerBox.enabled = false;
    }

    // ==============================
    // --- Coin and Health Management ---
    // ==============================

    /// <summary>
    /// Adds coins to the player's total and updates UI.
    /// </summary>
    public void AddCoin(int c)
    {
        coin += c;
        GameManage.Instance.UpdateCoin();
    }

    /// <summary>
    /// Restores health and updates the health bar.
    /// </summary>
    public void AddHealth(float h)
    {
        hp = Mathf.Min(hp + h, maxHp);
        GameManage.Instance.UpdateHealth();
    }
}
