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
        StartCoroutine(ShootBullet());
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
        }
        else if (!isTargeting)
        {
            StartCoroutine(TargetPlayer());
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
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        yield return new WaitForSeconds(targetingDelay);
        float moveTime = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, sideSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
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
}
