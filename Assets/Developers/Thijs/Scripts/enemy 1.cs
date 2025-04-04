using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PoliceSwatCarEnemy : MonoBehaviour
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

    [Header("Activation Settings")]
    [Tooltip("Should the enemy activate automatically on start?")]
    public bool activateOnStart = true;

    [System.Serializable]
    public class PositionConfig
    {
        public enum PositionType { UseTransform, UseVector3 }

        [Tooltip("Whether to use a Transform reference or a direct Vector3 position")]
        public PositionType positionType = PositionType.UseVector3;

        [Tooltip("The Transform to use as position reference (if positionType is UseTransform)")]
        public Transform targetTransform;

        [Tooltip("The direct position to use (if positionType is UseVector3)")]
        public Vector3 position;

        public Vector3 GetPosition(Transform defaultTransform = null)
        {
            if (positionType == PositionType.UseTransform && targetTransform != null)
                return targetTransform.position;
            else if (positionType == PositionType.UseTransform && defaultTransform != null)
                return defaultTransform.position;
            return position;
        }
    }

    [Header("Position Configuration")]
    [Tooltip("The starting position of the enemy")]
    public PositionConfig startPosition = new PositionConfig();

    [Tooltip("The left boundary for left-right movement")]
    public PositionConfig leftBoundary = new PositionConfig();

    [Tooltip("The right boundary for left-right movement")]
    public PositionConfig rightBoundary = new PositionConfig();

    [Tooltip("The position to move to when throwing a frag bomb")]
    public PositionConfig bombPosition = new PositionConfig();

    [Tooltip("The position to move to for zigzag shooting")]
    public PositionConfig zigZagPosition = new PositionConfig();

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
        public PositionConfig targetPosition = new PositionConfig(); // For MoveToPosition
    }

    [Header("Action Sequence")]
    public List<EnemyAction> actionSequence = new List<EnemyAction>();

    private Vector3 cachedStartPosition;
    private Vector3 cachedLeftBoundary;
    private Vector3 cachedRightBoundary;
    private bool movingRight = true;
    private bool isPerformingAction = false;
    private int currentActionIndex = 0;
    private bool isActive = false;

    void Start()
    {
        // Cache positions
        cachedStartPosition = startPosition.GetPosition(transform);

        // Set default left boundary if not configured
        if (leftBoundary.positionType == PositionConfig.PositionType.UseVector3 && leftBoundary.position == Vector3.zero)
        {
            leftBoundary.position = cachedStartPosition - new Vector3(5f, 0, 0);
        }
        cachedLeftBoundary = leftBoundary.GetPosition(transform);

        // Set default right boundary if not configured
        if (rightBoundary.positionType == PositionConfig.PositionType.UseVector3 && rightBoundary.position == Vector3.zero)
        {
            rightBoundary.position = cachedStartPosition + new Vector3(5f, 0, 0);
        }
        cachedRightBoundary = rightBoundary.GetPosition(transform);

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

        // Activate the enemy if set to activate on start
        if (activateOnStart)
        {
            Activate();
        }
    }

    // Call this method to activate the enemy behavior
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            if (actionSequence.Count == 0)
            {
                Debug.LogWarning("No actions in sequence. Configure actions in the inspector.");
                return;
            }
            StopAllCoroutines(); // Stop any running coroutines to avoid conflicts
            StartCoroutine(ExecuteActionSequence());
            Debug.Log("Enemy activated and executing action sequence!");
        }
    }

    public void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();
        Debug.Log("Enemy deactivated!");
    }

    IEnumerator ExecuteActionSequence()
    {
        currentActionIndex = 0;
        Debug.Log("Starting action sequence with " + actionSequence.Count + " actions.");

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
                Debug.Log("Restarting action sequence from beginning.");
            }

            EnemyAction currentAction = actionSequence[currentActionIndex];
            Debug.Log("Executing action: " + currentAction.actionType + " (Duration: " + currentAction.duration + ")");

            isPerformingAction = true;
            yield return StartCoroutine(ExecuteAction(currentAction));
            isPerformingAction = false;

            currentActionIndex++;
            yield return new WaitForSeconds(0.1f); // Small delay before next action
        }
    }

    IEnumerator ExecuteAction(EnemyAction action)
    {
        switch (action.actionType)
        {
            case ActionType.MoveLeftRight:
                Debug.Log("Starting MoveLeftRight action");
                yield return StartCoroutine(MoveLeftRight(action.duration));
                break;
            case ActionType.HomeToPlayer:
                Debug.Log("Starting HomeToPlayer action");
                yield return StartCoroutine(HomeToPlayer(action.duration));
                break;
            case ActionType.ShootBullets:
                Debug.Log("Starting ShootBullets action");
                yield return StartCoroutine(ShootBullets(action.duration));
                break;
            case ActionType.ThrowFragBomb:
                Debug.Log("Starting ThrowFragBomb action");
                Vector3 bombPos = bombPosition.GetPosition(transform);
                yield return StartCoroutine(MoveToPosition(bombPos, 1f)); // Move to bomb position first
                ThrowFragBomb();
                yield return new WaitForSeconds(2f); // Wait for bomb activation
                break;
            case ActionType.ZigZagShooting:
                Debug.Log("Starting ZigZagShooting action");
                yield return StartCoroutine(ZigZagShooting(action.duration));
                break;
            case ActionType.MoveToPosition:
                Vector3 targetPos = action.targetPosition.GetPosition(transform);
                Debug.Log("Starting MoveToPosition action to " + targetPos);
                yield return StartCoroutine(MoveToPosition(targetPos, action.duration));
                break;
        }
        Debug.Log("Finished action: " + action.actionType);
    }

    IEnumerator MoveLeftRight(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration && isActive)
        {
            float direction = movingRight ? 1 : -1;
            Vector3 movement = Vector3.right * direction * sideSpeed * Time.deltaTime;
            transform.position += movement;

            // Reverse direction if out of bounds
            if ((movingRight && transform.position.x > cachedRightBoundary.x) ||
                (!movingRight && transform.position.x < cachedLeftBoundary.x))
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

        while (elapsedTime < duration && isActive)
        {
            if (player == null)
            {
                Debug.LogWarning("Player reference is null! Cannot home to player.");
                yield break; // Ensure player is valid
            }

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.position += directionToPlayer * homingSpeed * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ShootBullets(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration && isActive)
        {
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
            elapsedTime += shootInterval;
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

        if (bulletRb != null && player != null)
        {
            Vector3 direction = (player.position - spawnPosition).normalized;
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
        else if (bulletRb != null)
        {
            // If no player, shoot forward
            bulletRb.linearVelocity = transform.forward * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        Debug.Log("Moving from " + startingPos + " to " + targetPosition + " over " + duration + " seconds");

        while (elapsedTime < duration && isActive)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startingPos, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches the exact position
        if (isActive)
        {
            transform.position = targetPosition;
            Debug.Log("Reached target position: " + targetPosition);
        }
    }

    void ThrowFragBomb()
    {
        if (fragBombPrefab == null)
        {
            Debug.LogWarning("Frag bomb prefab is not assigned!");
            return;
        }

        Vector3 bombPos = transform.position; // Use current position after moving
        GameObject fragBomb = Instantiate(fragBombPrefab, bombPos, Quaternion.identity);

        // Try to find FragBomb component or any component with Explode method
        FragBomb bombScript = fragBomb.GetComponent<FragBomb>();
        if (bombScript != null)
        {
            bombScript.Explode(); // Assuming the FragBomb script handles fragment spawning
        }
        else
        {
            Debug.LogWarning("FragBomb component not found on the instantiated prefab!");
        }
    }

    IEnumerator ZigZagShooting(float duration)
    {
        float elapsedTime = 0f;
        Vector3 zigZagCenter = zigZagPosition.GetPosition(transform);

        // Move to zigzag position first
        Debug.Log("Moving to zigzag center position: " + zigZagCenter);
        yield return StartCoroutine(MoveToPosition(zigZagCenter, 1f));

        while (elapsedTime < duration && isActive)
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
        // Only run these calculations in editor mode to avoid overhead
        Vector3 startPos = Application.isPlaying ? cachedStartPosition : startPosition.GetPosition(transform);
        Vector3 leftPos = Application.isPlaying ? cachedLeftBoundary : leftBoundary.GetPosition(transform);
        Vector3 rightPos = Application.isPlaying ? cachedRightBoundary : rightBoundary.GetPosition(transform);
        Vector3 bombPos = bombPosition.GetPosition(transform);
        Vector3 zigPos = zigZagPosition.GetPosition(transform);

        // Draw start position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos, 0.5f);
        Gizmos.DrawLine(startPos, startPos + Vector3.up * 1.5f);

        // Draw bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawSphere(leftPos, 0.3f);
        Gizmos.DrawSphere(rightPos, 0.3f);

        // Draw bomb position
        if (bombPosition.positionType == PositionConfig.PositionType.UseVector3 ||
            (bombPosition.positionType == PositionConfig.PositionType.UseTransform && bombPosition.targetTransform != null))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bombPos, 0.4f);
            Gizmos.DrawLine(bombPos, bombPos + Vector3.up * 1f);
        }

        // Draw zigzag position
        if (zigZagPosition.positionType == PositionConfig.PositionType.UseVector3 ||
            (zigZagPosition.positionType == PositionConfig.PositionType.UseTransform && zigZagPosition.targetTransform != null))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(zigPos, 0.4f);

            // Draw zigzag pattern
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Vector3 lastPos = zigPos - new Vector3(zigZagAmplitude, 0, 0);
            for (float t = 0; t <= 2 * Mathf.PI; t += 0.1f)
            {
                float xOffset = Mathf.Sin(t) * zigZagAmplitude;
                Vector3 newPos = zigPos + new Vector3(xOffset, 0, 0);
                Gizmos.DrawLine(lastPos, newPos);
                lastPos = newPos;
            }
        }

        // Draw action sequence positions if any
        if (!Application.isPlaying)
        {
            int index = 0;
            foreach (EnemyAction action in actionSequence)
            {
                if (action.actionType == ActionType.MoveToPosition)
                {
                    Vector3 actionPos = action.targetPosition.GetPosition(transform);
                    Gizmos.color = new Color(1f, 0.5f, 0f); // Orange
                    Gizmos.DrawSphere(actionPos, 0.35f);

                    // Draw index number
                    Handles.Label(actionPos + Vector3.up * 0.5f, "Action " + index);
                }
                index++;
            }
        }
    }
}