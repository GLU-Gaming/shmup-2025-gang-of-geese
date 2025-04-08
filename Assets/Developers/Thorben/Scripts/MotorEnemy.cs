using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotorEnemy : MonoBehaviour
{
    private hpsystem playerhpsystem;
    [Header("Target Settings")]
    public Transform player;
    public bool autoFindPlayer = true;
    public string playerTag = "Player";

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    [Header("Movement Settings")]
    public float forwardSpeed = 1f;
    public float sideSpeed = 2f;
    public float maxLeftDistance = 5f;
    public float maxRightDistance = 5f;
    public float obstacleCheckDistance = 1f;
    public float randomMovementInterval = 3f;
    public float targetingDelay = 8f;
    public LayerMask obstacleLayer;

    [Header("Attack Settings")]
    public float shootInterval = 0.5f; // Faster shooting interval
    public float bulletSpeed = 15f; // Faster bullet speed
    public float bulletLifetime = 5f;

    private bool isActive = false;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isObstacleAhead = false;
    private bool isTargeting = false;
    private float lastRandomMoveTime;
    private List<GameObject> detectedObstacles = new List<GameObject>();

    void Start()
    {
        playerhpsystem = FindFirstObjectByType<hpsystem>();
        startPosition = transform.position;
        lastRandomMoveTime = Time.time;

        // Find player if needed
        if (player == null && autoFindPlayer)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Make sure the Player object has the '" + playerTag + "' tag or assign it directly.");
            }
        }

        // Activate the enemy
        Activate();
    }

    void Update()
    {
        CheckForObstacles();
        if (!isObstacleAhead && detectedObstacles.Count == 0)
        {
            if (Time.time - lastRandomMoveTime > randomMovementInterval)
            {
                StartCoroutine(RandomSideMovement());
            }
            else if (!isTargeting)
            {
                StartCoroutine(TargetPlayer());
            }
        }
    }

    void CheckForObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, obstacleCheckDistance, obstacleLayer);
        isObstacleAhead = hit.collider != null;
        if (isObstacleAhead && !detectedObstacles.Contains(hit.collider.gameObject))
        {
            detectedObstacles.Add(hit.collider.gameObject);
        }
        else if (!isObstacleAhead && detectedObstacles.Count > 0)
        {
            detectedObstacles.Clear();
        }
    }

    IEnumerator RandomSideMovement()
    {
        lastRandomMoveTime = Time.time;
        movingRight = Random.value > 0.5f;
        float moveTime = Random.Range(0.5f, 1.5f);
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            float direction = movingRight ? 1 : -1;
            Vector3 movement = Vector3.right * direction * sideSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;
            float clampedX = Mathf.Clamp(newPosition.x, startPosition.x - maxLeftDistance, startPosition.x + maxRightDistance);
            transform.position = new Vector3(clampedX, newPosition.y, newPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator TargetPlayer()
    {
        isTargeting = true;
        Vector3 directionToPlayer = player.position - transform.position;

        // Only allow targeting if player is to the right (positive x direction)
        if (directionToPlayer.x > 0)
        {
            // Calculate direction while ignoring Z-axis difference
            Vector3 horizontalDirectionToPlayer = new Vector3(directionToPlayer.x, directionToPlayer.y, 0);
            float angle = Mathf.Atan2(horizontalDirectionToPlayer.y, horizontalDirectionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Wait for the specified targeting delay before moving
            yield return new WaitForSeconds(targetingDelay);

            float moveTime = 1f;
            float elapsedTime = 0f;
            while (elapsedTime < moveTime)
            {
                // Only move towards the player if they are still to the right
                if (player.position.x > transform.position.x)
                {
                    // Move only on X-axis, keeping original Y and Z
                    Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, sideSpeed * Time.deltaTime);
                }
                else
                {
                    // Break out of the targeting if player moves left
                    break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        isTargeting = false;
    }

    void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(ShootBullets());
            Debug.Log("Motor enemy activated and shooting faster!");
        }
    }

    IEnumerator ShootBullets()
    {
        while (isActive)
        {
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        // Use bulletSpawnPoint if available, otherwise use enemy position
        Vector3 spawnPosition = bulletSpawnPoint != null ?
            bulletSpawnPoint.position : transform.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // Shoot along the z-axis
            bulletRb.linearVelocity = transform.forward * bulletSpeed;
        }

        // Add a component to handle collision and destruction
        BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
        collisionHandler.playerHpSystem = playerhpsystem;  // Pass the reference

        Destroy(bullet, bulletLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hpsystem playerHpSystem = collision.gameObject.GetComponent<hpsystem>();
            if (playerHpSystem != null)
            {
                playerHpSystem.TakeDamage();
            }
        }
    }
}

public class BulletCollisionHandler : MonoBehaviour
{
    public hpsystem playerHpSystem;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            if (playerHpSystem != null)
            {
                playerHpSystem.TakeDamage();
            }
            else
            {
                Debug.LogError("Player HP System is null!");
            }

            // Destroy the bullet upon hitting the player
            Destroy(gameObject);
        }
        else
        {
            // Destroy bullet on any other collision
            Destroy(gameObject);
        }
    }
}
