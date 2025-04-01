using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ramenemy : MonoBehaviour
{
    public float ramSpeed = 10f;
    public float ramCooldown = 5f;

    private Vector3 originalPosition;
    private bool isRamming = false;
    private float ramTimer = 0f;
    hpsystem playerHpSystem;

    void Start()
    {
        originalPosition = transform.position;
        playerHpSystem = Object.FindFirstObjectByType<hpsystem>();
        if (playerHpSystem == null)
        {
            Debug.LogError("No HP system found in the scene!");
        }
       
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
        ramTarget.z -= 1.5f;
        while (Vector3.Distance(transform.position, ramTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ramTarget, ramSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, ramSpeed * Time.deltaTime);
            yield return null;
        }

        isRamming = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //hpsystem playerHpSystem = collision.gameObject.GetComponent<hpsystem>();
            if (playerHpSystem != null)
            {
                playerHpSystem.TakeDamage();
            }
        }
    }
}
