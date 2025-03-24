using UnityEngine;

public class Playershoot : MonoBehaviour
{
    public GameObject bulletPrefab;  // The bullet prefab to instantiate
    public Transform bulletSpawnPoint;  // The point from where the bullet will be spawned
    public float bulletSpeed = 20f;  // The speed of the bullet
    public float bulletLifetime = 5f;  // The lifetime of the bullet in seconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the spawn point
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        // Add velocity to the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        }
        // Destroy the bullet after a certain amount of seconds
        Destroy(bullet, bulletLifetime);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Add score when hitting an enemy
            ScoreSystem.instance.AddScore(10);
            // Destroy the enemy
            Destroy(collision.gameObject);
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
