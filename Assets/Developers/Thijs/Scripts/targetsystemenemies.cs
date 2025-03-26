using UnityEngine;
using System.Collections;
public class targetsystemenemies : MonoBehaviour
{

    public Transform player;
    public float moveSpeed = 3f;
    public float targetingDelay = 8f;
    private bool canTarget = true;
    private bool isMoving = false;


    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

   
    }

    void Update()
    {
        // Only check for targeting if allowed
        if (canTarget && !isMoving)
        {
            StartCoroutine(TargetWithDelay());
        }
    }

    IEnumerator TargetWithDelay()
    {
        canTarget = false;

        // Determine if player is to the left or right
        bool playerIsOnLeft = player.position.x < transform.position.x;

        // Flip sprite based on player position
      

        // Wait for targeting delay
        yield return new WaitForSeconds(targetingDelay);

        // Move towards player
        StartCoroutine(MoveTowardsPlayer(playerIsOnLeft));

        // Reset targeting after movement completes
        canTarget = true;
    }

    IEnumerator MoveTowardsPlayer(bool moveLeft)
    {
        isMoving = true;
        float moveDirection = moveLeft ? -1f : 1f;
        float moveDuration = 2f; // Time to move
        float timer = 0f;

        while (timer < moveDuration)
        {
            transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}
