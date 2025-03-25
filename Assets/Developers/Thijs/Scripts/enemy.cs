
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class enemy : MonoBehaviour
{



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
    }

    void Update()
    {
        CheckForObstacles();

        if (!isObstacleAhead && detectedObstacles.Count == 0)
        {
            MoveForward();
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

    void MoveForward()
    {
        transform.Translate(Vector3.up * forwardSpeed * Time.deltaTime);
    }

    IEnumerator RandomSideMovement()
    {
        lastRandomMoveTime = Time.time;

        // Decide direction
        movingRight = Random.value > 0.5f;

        float moveTime = Random.Range(0.5f, 1.5f);
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            float direction = movingRight ? 1 : -1;
            Vector3 movement = Vector3.right * direction * sideSpeed * Time.deltaTime;

            // Clamp the movement within boundaries
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

        // Face the player
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        yield return new WaitForSeconds(targetingDelay);

        // Move towards the player
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

    void OnDrawGizmos()
    {
        // Visualize the obstacle check ray
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * obstacleCheckDistance);

        // Visualize the movement boundaries
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(startPosition.x - maxLeftDistance, transform.position.y, transform.position.z),
                        new Vector3(startPosition.x + maxRightDistance, transform.position.y, transform.position.z));
    }
}


    

