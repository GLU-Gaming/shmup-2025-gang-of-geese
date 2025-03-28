using UnityEngine;
using System.Collections;

public class ramenemy : MonoBehaviour
{
    public float ramSpeed = 10f;
    public float ramDistance = 5f;
    public float ramCooldown = 5f;

    private Vector3 originalPosition;
    private bool isRamming = false;
    private float ramTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        ramTimer += Time.deltaTime;

        if (!isRamming && ramTimer >= ramCooldown)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                StartCoroutine(RamPlayer(player));
                ramTimer = 0f;
            }
        }
    }

    private IEnumerator RamPlayer(GameObject player)
    {
        isRamming = true;

        Vector3 ramTarget = player.transform.position;
        while (Vector3.Distance(transform.position, ramTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ramTarget, ramSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Pause before returning

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, ramSpeed * Time.deltaTime);
            yield return null;
        }

        isRamming = false;
    }
}

