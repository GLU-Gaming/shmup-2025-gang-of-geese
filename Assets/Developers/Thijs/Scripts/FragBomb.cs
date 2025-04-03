using UnityEngine;
using System.Collections;

public class FragBomb : MonoBehaviour
{
    public GameObject fragmentPrefab; // Prefab for the fragments
    public int fragmentCount = 12; // Number of fragments to spawn
    public float explosionRadius = 5f; // Radius of the explosion
    public float explosionForce = 10f; // Force applied to fragments
    public float fuseTime = 2f; // Time before the bomb explodes

   public void Start()
    {
        // Start the fuse countdown
        StartCoroutine(ExplodeAfterDelay(fuseTime));
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    public void Explode()
    {
        // Spawn fragments in a circular pattern in the XZ plane
        for (int i = 0; i < fragmentCount; i++)
        {
            float angle = i * (360f / fragmentCount);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            SpawnFragment(direction);
        }

        // Destroy the bomb object after exploding
        Destroy(gameObject);
    }

  public  void SpawnFragment(Vector3 direction)
    {
        GameObject fragment = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
        FragmentMovement fragmentMovement = fragment.AddComponent<FragmentMovement>();
        fragmentMovement.direction = direction;
        fragmentMovement.speed = explosionForce;

        // Destroy the fragment after a certain time to avoid clutter
        Destroy(fragment, 5f);
    }
}
