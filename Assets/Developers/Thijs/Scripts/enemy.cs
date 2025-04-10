using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    public float shootInterval = 2f;
    public float forwardSpeed = 1f;
    public float sideSpeed = 2f;
    public float maxLeftDistance = 5f;
    public float maxRightDistance = 5f;
    public float obstacleCheckDistance = 1f;
    public float randomMovementInterval = 3f;
    public float targetingDelay = 8f;
    public LayerMask obstacleLayer;
    public Transform player;

    // Reference to the HP system
    private hpsystem playerHpSystem;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isObstacleAhead = false;
    private bool isTargeting = false;
    private float lastRandomMoveTime;
    private List<GameObject> detectedObstacles = new List<GameObject>();

    void Start()
    {
        startPosition = transform.position;
        lastRandomMoveTime = Time.time;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Find the HP system component
        playerHpSystem = Object.FindFirstObjectByType<hpsystem>();
        if (playerHpSystem == null)
        {
            Debug.LogError("No HP system found in the scene!");
        }
        if (bulletSpawnPoint != null)
        {
            StartCoroutine(ShootBullet());
        }

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

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

    IEnumerator ShootBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            // Add a component to handle collision and destruction
            BulletCollisionHandler1 collisionHandler = bullet.AddComponent<BulletCollisionHandler1>();
            collisionHandler.playerHpSystemInstance = playerHpSystem;  // Pass the reference
            Destroy(bullet, 5f);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * obstacleCheckDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(startPosition.x - maxLeftDistance, transform.position.y, transform.position.z),
                        new Vector3(startPosition.x + maxRightDistance, transform.position.y, transform.position.z));
    }

    private void OnCollisionEnter(Collision collision)
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

public class BulletCollisionHandler1 : MonoBehaviour
{
    public hpsystem playerHpSystemInstance;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            if (playerHpSystemInstance != null)
            {
                playerHpSystemInstance.TakeDamage();
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


