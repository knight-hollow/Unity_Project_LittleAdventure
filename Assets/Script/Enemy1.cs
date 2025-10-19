using UnityEngine;

public class Enemy1 : EnemyBase
{
    // Reference to the player's transform (target to chase and attack)
    public Transform targetPlayer;

    // NavMeshAgent used for navigation and pathfinding
    public UnityEngine.AI.NavMeshAgent _nav;

    // Collider used as a hitbox during attacks
    public BoxCollider _AttackTriggerBox;

    private void Awake()
    {
        // Cache components
        _nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _anim = GetComponent<Animator>();

        // Find and store a reference to the attack hitbox (child object named "DamageCaster")
        _AttackTriggerBox = transform.Find("DamageCaster").GetComponent<BoxCollider>();
    }

    void Start()
    {
        // Set the target to the player (singleton instance)
        targetPlayer = Player.Instance.transform;

        // Initialize the enemy's attack damage value
        Damage = 30f;
    }

    void Update()
    {
        // Stop all logic if the enemy is dead
        if (isDead)
        {
            return;
        }

        // Handle ongoing attack states
        if (isAttacking)
        {
            // Countdown the attack timer
            attackTimer -= Time.deltaTime;

            // Once a certain portion of the attack time has passed, stop attacking
            if (attackTimer <= (attackInterval - attackConsumerTime))
            {
                isAttacking = false;
            }
        }

        // Handle attack cooldown (enemy cannot attack again until the timer reaches 0)
        if (!canAttack && !isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
                canAttack = true;
            }
        }

        // Main movement and decision-making logic
        CalculateEnemyMovement();

        // Update animation movement state based on NavMesh speed
        if (_nav.speed > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    /// <summary>
    /// Handles navigation, chasing, and attack range decisions.
    /// </summary>
    private void CalculateEnemyMovement()
    {
        // Skip movement logic during attack animation
        if (isAttacking)
        {
            return;
        }

        // Compute distance to player
        float distance = Vector3.Distance(targetPlayer.position, transform.position);

        // Compute facing direction and target rotation toward the player
        Vector3 direction = targetPlayer.position - transform.position;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);

        // --- Behavior Logic ---

        // 1️⃣ If player is too far (> 6 units) — enemy stays idle
        if (distance > 6)
        {
            _nav.speed = 0f;
            _anim.SetFloat("speed", _nav.speed);
            return;
        }

        // 2️⃣ If player is at medium range (2–6 units) — chase the player
        if (distance >= 2)
        {
            _nav.speed = 3.5f; // Move toward player
            _nav.SetDestination(targetPlayer.position);
            _anim.SetFloat("speed", _nav.speed);
        }
        // 3️⃣ If player is within attack range (< 2 units) — stop and attack
        else
        {
            _nav.SetDestination(targetPlayer.position);
            _nav.speed = 0f;
            _anim.SetFloat("speed", _nav.speed);

            // If attack cooldown is ready, perform attack
            if (canAttack)
            {
                Attack();
            }

            // Smoothly rotate enemy to face the player
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Initiates the attack sequence by triggering animation and timers.
    /// </summary>
    private void Attack()
    {
        // Play attack animation
        _anim.SetTrigger("Attack");

        // Disable further attacks until cooldown resets
        canAttack = false;

        // Mark enemy as currently attacking
        isAttacking = true;

        // Reset the attack timer for cooldown tracking
        attackTimer = attackInterval;
    }

    /// <summary>
    /// Called by animation events to enable the attack hitbox.
    /// </summary>
    public void OpenAttackTrigger()
    {
        _AttackTriggerBox.enabled = true;
    }

    /// <summary>
    /// Called by animation events to disable the attack hitbox.
    /// </summary>
    public void CloseAttackTrigger()
    {
        _AttackTriggerBox.enabled = false;
    }
}
