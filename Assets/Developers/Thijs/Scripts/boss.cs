using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float phase2HealthThreshold = 650f;
    [SerializeField] private float phase3HealthThreshold = 300f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveCloserDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float[] phaseCooldowns = { 3f, 2f, 1.5f };
    [SerializeField] private float attackDamage = 20f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject phaseTransitionEffect;
    [SerializeField] private GameObject areaIndicator;
    [SerializeField] private GameObject explosionEffect;

    // Internal state
    private int currentPhase = 1;
    private bool isChangingPhase = false;
    private float attackTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        attackTimer = phaseCooldowns[0];
    }

    void Update()
    {
        if (isChangingPhase)
            return;

        // Check if we need to change phase
        if (currentPhase == 1 && currentHealth <= phase2HealthThreshold)
            StartCoroutine(ChangePhase(2));
        else if (currentPhase == 2 && currentHealth <= phase3HealthThreshold)
            StartCoroutine(ChangePhase(3));

        // Handle attack timing
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = phaseCooldowns[currentPhase - 1];
        }
    }

    private IEnumerator ChangePhase(int newPhase)
    {
        isChangingPhase = true;
        Debug.Log($"Boss transitioning to Phase {newPhase}");

        // Visual effect
        if (phaseTransitionEffect != null)
        {
            GameObject effect = Instantiate(phaseTransitionEffect, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
            Destroy(effect, 3f);
        }

        yield return new WaitForSeconds(1f);

        // Move closer to player
        Vector3 targetPosition = transform.position;
        targetPosition.z -= moveCloserDistance;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Update phase
        currentPhase = newPhase;
        isChangingPhase = false;

        // Alert message
        string message = (newPhase == 2) ?
            "The boss becomes more aggressive!" :
            "The boss enters a desperate frenzy!";
        Debug.Log(message);
    }

    private void PerformAttack()
    {
        int attackType = Random.Range(0, 3);

        switch (currentPhase)
        {
            case 1:
                switch (attackType)
                {
                    case 0: SingleAreaAttack(); break;
                    case 1: TripleAreaAttack(); break;
                    case 2: PatternAreaAttack(); break;
                }
                break;

            case 2:
                switch (attackType)
                {
                    case 0: RapidFireAttack(); break;
                    case 1: CirclePatternAttack(); break;
                    case 2: SweepAttack(); break;
                }
                break;

            case 3:
                switch (attackType)
                {
                    case 0: CrossAttack(); break;
                    case 1: HomingAttack(); break;
                    case 2: GridAttack(); break;
                }
                break;
        }
    }

    #region Phase 1 Attacks
    private void SingleAreaAttack()
    {
        Debug.Log("Boss used Single Area Attack!");
        Vector3 targetPosition = playerTransform.position;
        StartCoroutine(CreateAreaAttack(targetPosition, 5f, Color.red));
    }

    private void TripleAreaAttack()
    {
        Debug.Log("Boss used Triple Area Attack!");
        Vector3 playerPos = playerTransform.position;

        StartCoroutine(CreateAreaAttack(playerPos + new Vector3(5f, 0, 0), 4f, Color.red));
        StartCoroutine(CreateAreaAttack(playerPos + new Vector3(-5f, 0, 0), 4f, Color.red, 0.3f));
        StartCoroutine(CreateAreaAttack(playerPos + new Vector3(0, 0, -5f), 4f, Color.red, 0.6f));
    }

    private void PatternAreaAttack()
    {
        Debug.Log("Boss used Pattern Area Attack!");
        float spacing = 3f;
        Vector3 startPos = playerTransform.position + new Vector3(-spacing, 0, -spacing);

        for (int i = 0; i < 3; i++)
        {
            Vector3 attackPos = startPos + new Vector3(spacing * i, 0, 0);
            StartCoroutine(CreateAreaAttack(attackPos, 3f, Color.red, i * 0.2f));
        }
    }
    #endregion

    #region Phase 2 Attacks
    private void RapidFireAttack()
    {
        Debug.Log("Boss used Rapid Fire Attack!");
        StartCoroutine(RapidFireSequence());
    }

    private IEnumerator RapidFireSequence()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
            Vector3 targetPos = playerTransform.position + offset;

            StartCoroutine(CreateAreaAttack(targetPos, 2.5f, Color.magenta, 0f, 0.8f));
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void CirclePatternAttack()
    {
        Debug.Log("Boss used Circle Pattern Attack!");
        StartCoroutine(CirclePattern());
    }

    private IEnumerator CirclePattern()
    {
        int areaCount = 8;
        float radius = 6f;
        Vector3 centerPos = playerTransform.position;

        for (int i = 0; i < areaCount; i++)
        {
            float angle = i * (360f / areaCount) * Mathf.Deg2Rad;
            float x = centerPos.x + radius * Mathf.Cos(angle);
            float z = centerPos.z + radius * Mathf.Sin(angle);

            StartCoroutine(CreateAreaAttack(new Vector3(x, 0, z), 3f, Color.magenta));
        }

        yield return new WaitForSeconds(1.2f);
        StartCoroutine(CreateAreaAttack(centerPos, 4f, Color.red));
    }

    private void SweepAttack()
    {
        Debug.Log("Boss used Sweep Attack!");
        StartCoroutine(SweepPattern());
    }

    private IEnumerator SweepPattern()
    {
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
        Vector3 perpendicular = Vector3.Cross(playerDirection, Vector3.up).normalized;

        int attackCount = 7;
        float width = 12f;
        float distanceFromBoss = 8f;

        Vector3 startPoint = transform.position + playerDirection * distanceFromBoss - perpendicular * (width / 2);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 attackPos = startPoint + perpendicular * (width * i / (attackCount - 1));
            StartCoroutine(CreateAreaAttack(attackPos, 3f, Color.magenta, i * 0.15f));
        }

        yield return null;
    }
    #endregion

    #region Phase 3 Attacks
    private void CrossAttack()
    {
        Debug.Log("Boss used Cross Attack!");
        StartCoroutine(CrossPattern());
    }

    private IEnumerator CrossPattern()
    {
        Vector3 playerPos = playerTransform.position;

        for (int i = -3; i <= 3; i++)
        {
            if (i == 0) continue;
            StartCoroutine(CreateAreaAttack(playerPos + new Vector3(i * 3f, 0, 0), 3f, Color.yellow, Mathf.Abs(i) * 0.2f));
            StartCoroutine(CreateAreaAttack(playerPos + new Vector3(0, 0, i * 3f), 3f, Color.yellow, Mathf.Abs(i) * 0.2f));
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CreateAreaAttack(playerPos, 5f, Color.red));
    }

    private void HomingAttack()
    {
        Debug.Log("Boss used Homing Attack!");
        StartCoroutine(HomingSequence());
    }

    private IEnumerator HomingSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject indicator = Instantiate(areaIndicator, playerTransform.position, Quaternion.identity);
            indicator.transform.localScale = new Vector3(4f, 1f, 4f);

            Renderer indicatorRenderer = indicator.GetComponent<Renderer>();
            if (indicatorRenderer != null)
                indicatorRenderer.material.color = new Color(1f, 0.5f, 0, 0.5f);

            // Follow player for 1.5 seconds
            float timer = 0;
            while (timer < 1.5f)
            {
                indicator.transform.position = new Vector3(
                    playerTransform.position.x,
                    0.1f,
                    playerTransform.position.z
                );

                timer += Time.deltaTime;
                yield return null;
            }

            // Change color to red when locked
            if (indicatorRenderer != null)
                indicatorRenderer.material.color = new Color(1f, 0, 0, 0.5f);

            Vector3 lockedPosition = indicator.transform.position;
            yield return new WaitForSeconds(0.8f);

            // Explode
            Destroy(indicator);
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, lockedPosition, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            // Check if player is hit
            float blastRadius = 2f;
            if (Vector3.Distance(lockedPosition, playerTransform.position) <= blastRadius)
                Debug.Log("Player hit by homing attack!");

            yield return new WaitForSeconds(0.7f);
        }
    }

    private void GridAttack()
    {
        Debug.Log("Boss used Grid Attack!");
        StartCoroutine(GridPattern());
    }

    private IEnumerator GridPattern()
    {
        int gridSize = 5;
        float spacing = 4f;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 gridCenter = transform.position + direction * 10f;

        float offset = (gridSize - 1) * spacing / 2;
        Vector3 startPos = gridCenter - new Vector3(offset, 0, offset);

        yield return new WaitForSeconds(1.0f);

        for (int layer = 0; layer < (gridSize + 1) / 2; layer++)
        {
            for (int i = layer; i < gridSize - layer; i++)
            {
                for (int j = layer; j < gridSize - layer; j++)
                {
                    if (i == layer || i == gridSize - layer - 1 ||
                        j == layer || j == gridSize - layer - 1)
                    {
                        Vector3 pos = startPos + new Vector3(i * spacing, 0, j * spacing);
                        float delay = layer * 0.3f + Random.Range(0f, 0.2f);
                        StartCoroutine(CreateAreaAttack(pos, 3f, Color.yellow, delay));
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    private IEnumerator CreateAreaAttack(Vector3 position, float size, Color color, float delay = 0f, float indicationDuration = 1.5f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        // Create indicator
        GameObject indicator = Instantiate(areaIndicator, position, Quaternion.identity);
        indicator.transform.localScale = new Vector3(size, 1, size);

        Renderer indicatorRenderer = indicator.GetComponent<Renderer>();
        if (indicatorRenderer != null)
        {
            color.a = 0.5f;
            indicatorRenderer.material.color = color;
        }

        yield return new WaitForSeconds(indicationDuration);

        // Destroy indicator and create explosion
        Destroy(indicator);
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        // Check if player is hit
        float radius = size / 2f;
        if (Vector3.Distance(position, playerTransform.position) <= radius)
            Debug.Log($"Player hit by area attack for {attackDamage} damage!");
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        // Add game-specific boss death behavior here
    }
}