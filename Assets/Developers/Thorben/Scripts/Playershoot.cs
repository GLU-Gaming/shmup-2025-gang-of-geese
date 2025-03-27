using UnityEngine;

public class Playershoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;

    void Start()
    {
    }

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
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.bulletSpeed = bulletSpeed;
        bulletScript.bulletLifetime = bulletLifetime;

        TextMesh textMesh = bullet.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        bullet.transform.Rotate(0, 270, 0);
    }
}
