using UnityEngine;

public class Enemy2 : EnemyBase
{
    // Reference to the player's transform (target to chase or shoot)
    public Transform targetPlayer;

    // NavMeshAgent handles pathfinding and movement
    public UnityEngine.AI.NavMeshAgent _nav;

    // Bullet prefab to be instantiated when the enemy shoots
    public GameObject bullet;

    // The position (Transform) where bullets are spawned (usually a child object named "ShootPos")
    public Transform _shootPos;

    private void Awake()
    {
        // Cache components
        _nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _anim = GetComponent<Animator>();

        // Load bullet prefab from Resources folder
        bullet = Resources.Load<GameObject>("Prefab/Bullet");

        // Find the shoot point (a child transform)
        _shootPos = transform.Find("ShootPos");
    }

    void Start()
    {
        // Set player target (using singleton instance)
        targetPlayer = Player.Instance.transform;

        // Define this enemy's base damage per shot
        Damage = 20f;
    }

    void Update()
    {
        // Stop all logic if the enemy is dead
        if (isDead)
        {
            return;
        }

        // Manage attack state timing
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            // After the active attack phase ends, mark as not attacking
            if (attackTimer <= (attackInterval - attackConsumerTime))
            {
                isAttacking = false;
            }
        }

        // Manage attack cooldown timing (when the enemy can attack again)
        if (!canAttack && !isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
                canAttack = true;
            }
        }

        // Handle navigation and attack decision-making
        CalculateEnemyMovement();

        // Update animation movement state based on current speed
        isMoving = _nav.speed > 0;
    }

    /// <summary>
    /// Handles AI movement logic: chasing, stopping, and attacking based on distance.
    /// </summary>
    private void CalculateEnemyMovement()
    {
        // Skip movement logic during an attack animation
        if (isAttacking)
        {
            return;
        }

        // Calculate distance between enemy and player
        float distance = Vector3.Distance(targetPlayer.position, transform.position);

        // Direction vector toward player
        Vector3 direction = targetPlayer.position - transform.position;

        // Rotation needed to face the player
        Quaternion TargetRotation = Quaternion.LookRotation(direction);

        // --- Behavior Logic ---
        // 1️⃣ Player too far (> 12 units): idle (do nothing)
        if (distance > 12)
        {
            _nav.speed = 0f;
            _anim.SetFloat("speed", _nav.speed);
            return;
        }

        // 2️⃣ Medium range (6–12 units): chase the player
        if (distance >= 6)
        {
            _nav.speed = 3.5f;
            _nav.SetDestination(targetPlayer.position);
            _anim.SetFloat("speed", _nav.speed);
        }
        // 3️⃣ Close range (< 6 units): stop and attack
        else
        {
            _nav.SetDestination(targetPlayer.position);
            _nav.speed = 0f;
            _anim.SetFloat("speed", _nav.speed);

            // If attack cooldown is ready, initiate attack
            if (canAttack)
            {
                Attack();
            }

            // Smoothly rotate to face the player
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Starts the attack process (animation trigger and timers).
    /// </summary>
    private void Attack()
    {
        // Trigger attack animation (which may call Shoot() via animation event)
        _anim.SetTrigger("Attack");

        // Set attack state and reset timers
        canAttack = false;
        isAttacking = true;
        attackTimer = attackInterval;
    }

    /// <summary>
    /// Fires a bullet toward the player.
    /// Usually called during the attack animation via an Animation Event.
    /// </summary>
    public void Shoot()
    {
        // You could use direct forward direction or calculate dynamic direction to the player
        // Vector3 direction = (targetPlayer.position - transform.position).normalized;
        Vector3 direction = transform.forward;

        // Instantiate a new bullet at the shoot position
        GameObject go = Instantiate(bullet, _shootPos.position, Quaternion.identity);

        // Initialize the bullet with direction and damage value
        go.GetComponent<Bullet>().Init(direction, Damage);
    }
}
