using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Renderer playerRenderer;
    private Color originalColor;
    private bool isBlinking = false;

    void Start()
    {
        // Initialize player renderer and original color
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    public void TakeDamage()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkRed());
        }
    }

    private IEnumerator BlinkRed()
    {
        isBlinking = true;
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        isBlinking = false;
    }

    public static void DamageAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage();
            }
        }
    }
}


