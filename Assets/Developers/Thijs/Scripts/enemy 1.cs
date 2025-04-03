using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceCarEnemy : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public bool autoFindPlayer = true;
    public string playerTag = "Player";

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject fragBombPrefab;
    public Transform bulletSpawnPoint;

    [Header("Movement Settings")]
    public float sideSpeed = 2f;
    public float homingSpeed = 3f;
    public float zigZagAmplitude = 2f;
    public float zigZagFrequency = 2f;

    [Header("Attack Settings")]
    public float shootInterval = 1f;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 5f;

    [Header("Position Transforms")]
    public Transform startPositionTransform;
    public Transform leftBoundTransform;
    public Transform rightBoundTransform;
    public Transform bombPositionTransform;
    public Transform zigZagPositionTransform;

    [System.Serializable]
    public enum ActionType
    {
        MoveLeftRight,
        HomeToPlayer,
        ShootBullets,
        ThrowFragBomb,
        ZigZagShooting,
        MoveToPosition
    }

    [System.Serializable]
    public class EnemyAction
    {
        public ActionType actionType;
        public float duration = 3f;
        public Transform targetPosition; // For MoveToPosition
    }

    [Header("Action Sequence")]
    public List<EnemyAction> actionSequence = new List<EnemyAction>();

    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isPerformingAction = false;
    private int currentActionIndex = 0;
    private bool isActive = false;

    void Start()
    {
        // If no start position is assigned, use current position
        if (startPositionTransform == null)
        {
            startPositionTransform = transform;
        }
        startPosition = startPositionTransform.position;

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

        // Set default boundaries if not specified
        if (leftBoundTransform == null)
        {
            GameObject leftBound = new GameObject("LeftBound");
            leftBound.transform.position = startPosition - new Vector3(5f, 0, 0);
            leftBound.transform.parent = transform.parent;
            leftBoundTransform = leftBound.transform;
        }

        if (rightBoundTransform == null)
        {
            GameObject rightBound = new GameObject("RightBound");
            rightBound.transform.position = startPosition + new Vector3(5f, 0, 0);
            rightBound.transform.parent = transform.parent;
            rightBoundTransform = rightBound.transform;
        }
    }

    // Call this method to activate the enemy behavior
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(ExecuteActionSequence());
        }
    }

    public void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();
    }

    IEnumerator ExecuteActionSequence()
    {
        currentActionIndex = 0;

        while (isActive)
        {
            if (actionSequence.Count == 0)
            {
                Debug.LogWarning("No actions in sequence. Configure actions in the inspector.");
                yield break;
            }

            if (currentActionIndex >= actionSequence.Count)
            {
                currentActionIndex = 0; // Loop back to start
            }

            EnemyAction currentAction = actionSequence[currentActionIndex];

            isPerformingAction = true;
            yield return StartCoroutine(ExecuteAction(currentAction));
            isPerformingAction = false;

            currentActionIndex++;
            yield return null; // Small delay before next action
        }
    }

    IEnumerator ExecuteAction(EnemyAction action)
    {
        switch (action.actionType)
        {
            case ActionType.MoveLeftRight:
                yield return StartCoroutine(MoveLeftRight(action.duration));
                break;
            case ActionType.HomeToPlayer:
                yield return StartCoroutine(HomeToPlayer(action.duration));
                break;
            case ActionType.ShootBullets:
                yield return StartCoroutine(ShootBullets(action.duration));
                break;
            case ActionType.ThrowFragBomb:
                ThrowFragBomb();
                yield return new WaitForSeconds(2f); // Wait for bomb activation
                break;
            case ActionType.ZigZagShooting:
                yield return StartCoroutine(ZigZagShooting(action.duration));
                break;
            case ActionType.MoveToPosition:
                if (action.targetPosition != null)
                {
                    yield return StartCoroutine(MoveToPosition(action.targetPosition.position, action.duration));
                }
                else
                {
                    Debug.LogWarning("No target position assigned for MoveToPosition action!");
                    yield return null;
                }
                break;
        }
    }

    IEnumerator MoveLeftRight(float duration)
    {
        float elapsedTime = 0f;
        Vector3 leftBound = leftBoundTransform.position;
        Vector3 rightBound = rightBoundTransform.position;

        while (elapsedTime < duration)
        {
            float direction = movingRight ? 1 : -1;
            Vector3 movement = Vector3.right * direction * sideSpeed * Time.deltaTime;
            transform.position += movement;

            // Reverse direction if out of bounds
            if ((movingRight && transform.position.x > rightBound.x) ||
                (!movingRight && transform.position.x < leftBound.x))
            {
                movingRight = !movingRight;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HomeToPlayer(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (player == null) yield break; // Ensure player is valid

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.position += directionToPlayer * homingSpeed * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ShootBullets(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
            elapsedTime += shootInterval;
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null && player != null)
        {
            bulletRb.linearVelocity = (player.position - bulletSpawnPoint.position).normalized * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure it reaches the exact position
    }

    void ThrowFragBomb()
    {
        if (fragBombPrefab == null) return;

        Vector3 bombPosition = bombPositionTransform != null ?
            bombPositionTransform.position : transform.position;

        GameObject fragBomb = Instantiate(fragBombPrefab, bombPosition, Quaternion.identity);
        FragBomb bombScript = fragBomb.GetComponent<FragBomb>();

        if (bombScript != null)
        {
            bombScript.Explode(); // Assuming the FragBomb script handles fragment spawning
        }
    }

    IEnumerator ZigZagShooting(float duration)
    {
        float elapsedTime = 0f;
        Vector3 zigZagCenter = zigZagPositionTransform != null ?
            zigZagPositionTransform.position : transform.position;

        // Move to zigzag position first if specified
        if (zigZagPositionTransform != null)
        {
            yield return StartCoroutine(MoveToPosition(zigZagCenter, 1f));
        }

        while (elapsedTime < duration)
        {
            float zigZagOffset = Mathf.Sin(Time.time * zigZagFrequency) * zigZagAmplitude;
            Vector3 newPosition = zigZagCenter + new Vector3(zigZagOffset, 0, 0);
            transform.position = newPosition;

            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
            elapsedTime += shootInterval;
        }
    }

    // For visual debugging in the editor
    void OnDrawGizmos()
    {
        // Draw start position
        if (startPositionTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPositionTransform.position, 0.5f);
        }

        // Draw bounds
        if (leftBoundTransform != null && rightBoundTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftBoundTransform.position, rightBoundTransform.position);
            Gizmos.DrawSphere(leftBoundTransform.position, 0.3f);
            Gizmos.DrawSphere(rightBoundTransform.position, 0.3f);
        }

        // Draw bomb position
        if (bombPositionTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bombPositionTransform.position, 0.4f);
        }

        // Draw zigzag position
        if (zigZagPositionTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(zigZagPositionTransform.position, 0.4f);

            // Draw zigzag pattern
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Vector3 lastPos = zigZagPositionTransform.position - new Vector3(zigZagAmplitude, 0, 0);
            for (float t = 0; t <= 2 * Mathf.PI; t += 0.1f)
            {
                float xOffset = Mathf.Sin(t) * zigZagAmplitude;
                Vector3 newPos = zigZagPositionTransform.position + new Vector3(xOffset, 0, 0);
                Gizmos.DrawLine(lastPos, newPos);
                lastPos = newPos;
            }
        }
    }
}