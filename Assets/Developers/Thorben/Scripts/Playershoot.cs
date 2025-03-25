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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the spawn point
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        // Add the Bullet script to the instantiated bullet
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.bulletSpeed = bulletSpeed;  // Set the bullet speed
        bulletScript.bulletLifetime = bulletLifetime;  // Set the bullet lifetime
    }
}
