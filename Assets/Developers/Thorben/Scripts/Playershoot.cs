using UnityEngine;

public class Playershoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;

    // Spread shot variables
    [Header("Spread Shot Settings")]
    public float spreadAngleDegrees = 30f; // Single declaration of spreadAngleDegrees
    public int spreadBulletCount = 5; // Number of bullets in the spread

    // Charge shot variables
    private float chargeStartTime;
    private bool isCharging = false;
    private const float MAX_CHARGE_TIME = 1f; // Maximum charge time in seconds
    private const float MAX_SCALE_MULTIPLIER = 3f;

    // Weapon modes
    private enum WeaponMode
    {
        Normal,
        SpreadShot,
        ChargeShot
    }
    private WeaponMode currentMode = WeaponMode.Normal;

    void Update()
    {
        // Switch weapon modes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = WeaponMode.Normal;
            Debug.Log("Switched to Normal Shot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = WeaponMode.SpreadShot;
            Debug.Log("Switched to Spread Shot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMode = WeaponMode.ChargeShot;
            Debug.Log("Switched to Charge Shot");
        }

        // Shooting mechanics
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (currentMode)
            {
                case WeaponMode.Normal:
                    Shoot();
                    break;
                case WeaponMode.SpreadShot:
                    ShootSpread();
                    break;
                case WeaponMode.ChargeShot:
                    StartChargeShot();
                    break;
            }
        }

        // Charge shot release
        if (currentMode == WeaponMode.ChargeShot)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
            {
                ReleaseChargeShot();
            }
        }
    }

    void Shoot()
    {
        CreateBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, 1f);
    }

    void ShootSpread()
    {
        // Calculate the angle between each bullet in the spread
        float angleStep = spreadAngleDegrees * 2 / (spreadBulletCount - 1);

        for (int i = 0; i < spreadBulletCount; i++)
        {
            // Calculate the rotation for this bullet
            float currentAngle = -spreadAngleDegrees + (angleStep * i);

            Quaternion bulletRotation = Quaternion.Euler(
                bulletSpawnPoint.rotation.eulerAngles + new Vector3(0, currentAngle, 0)
            );

            // Create the bullet with the calculated rotation
            CreateBullet(bulletSpawnPoint.position, bulletRotation, 1f);
        }
    }

    void StartChargeShot()
    {
        chargeStartTime = Time.time;
        isCharging = true;
    }

    void ReleaseChargeShot()
    {
        if (!isCharging) return;

        // Calculate charge duration
        float chargeDuration = Mathf.Clamp(Time.time - chargeStartTime, 0f, MAX_CHARGE_TIME);

        // Calculate scale multiplier based on charge time
        float scaleMultiplier = 1f + (chargeDuration / MAX_CHARGE_TIME) * (MAX_SCALE_MULTIPLIER - 1f);

        CreateBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, scaleMultiplier);

        isCharging = false;
    }

    GameObject CreateBullet(Vector3 position, Quaternion rotation, float scaleMultiplier)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);

        // Add Bullet component
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.bulletSpeed = bulletSpeed;
        bulletScript.bulletLifetime = bulletLifetime;

        // Scale the bullet based on charge time
        bullet.transform.localScale *= scaleMultiplier;

        // Adjust text rotation
        TextMesh textMesh = bullet.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.transform.localRotation = Quaternion.Euler(90, 0, 0);
            // Optionally, you could scale the text size here if desired
            textMesh.characterSize *= scaleMultiplier;
        }

        bullet.transform.Rotate(0, 270, 0);

        return bullet;
    }
}