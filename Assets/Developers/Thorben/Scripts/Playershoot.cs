using UnityEngine;
using UnityEngine.UI; // Added for UI elements

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
    private const float MAX_VOLUME_MULTIPLIER = 1.0f; // Maximum volume for fully charged shots

    // Weapon modes
    private enum WeaponMode
    {
        Normal,
        SpreadShot,
        ChargeShot
    }
    private WeaponMode currentMode = WeaponMode.Normal;

    // Audio variables
    [Header("Audio Settings")]
    public AudioClip honk; // Assign your MP3 file here
    private AudioSource audioSource;
    public float baseVolume = 0.3f; // Base volume for normal shots
    public float maxVolume = 1.0f; // Maximum volume for fully charged shots
    public float basePitch = 1.0f; // Normal pitch
    public float maxPitch = 0.6f; // Lower pitch for charged shots (bass boost effect)
    // Sound distortion settings for charged shots
    public bool applyDistortion = true; // Toggle for distortion effect
    private AudioSource distortionSource; // Second audio source for layered sound

    // Shooting delay variables
    [Header("Shooting Delay Settings")]
    public float normalShotDelay = 0.5f; // Delay for Normal mode
    public float spreadShotDelay = 1f; // Delay for SpreadShot mode
    public float chargeShotDelay = 1.5f; // Delay for ChargeShot mode
    private float lastShotTime = 0f;

    // UI elements
    [Header("UI Elements")]
    public GameObject normalShotUI;
    public GameObject spreadShotUI;
    public GameObject chargeShotUI;

    void Start()
    {
        // Initialize the main AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = baseVolume;

        // Initialize the second AudioSource for layered sound effect
        distortionSource = gameObject.AddComponent<AudioSource>();
        distortionSource.playOnAwake = false;
        distortionSource.volume = 0;

        // Set initial UI state
        UpdateUIElements();
    }

    void Update()
    {
        // Switch weapon modes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = WeaponMode.Normal;
            Debug.Log("Switched to Normal Shot");
            UpdateUIElements();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = WeaponMode.SpreadShot;
            Debug.Log("Switched to Spread Shot");
            UpdateUIElements();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMode = WeaponMode.ChargeShot;
            Debug.Log("Switched to Charge Shot");
            UpdateUIElements();
        }

        // Shooting mechanics
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && CanShoot())
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

    // Update UI elements based on current weapon mode
    void UpdateUIElements()
    {
        // Make sure UI elements exist before trying to use them
        if (normalShotUI != null && spreadShotUI != null && chargeShotUI != null)
        {
            // Set all to inactive first
            normalShotUI.SetActive(false);
            spreadShotUI.SetActive(false);
            chargeShotUI.SetActive(false);

            // Then activate only the one corresponding to current mode
            switch (currentMode)
            {
                case WeaponMode.Normal:
                    normalShotUI.SetActive(true);
                    break;
                case WeaponMode.SpreadShot:
                    spreadShotUI.SetActive(true);
                    break;
                case WeaponMode.ChargeShot:
                    chargeShotUI.SetActive(true);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("One or more UI elements are not assigned in the inspector!");
        }
    }

    bool CanShoot()
    {
        float currentDelay = 0f;

        // Determine the delay based on the current weapon mode
        switch (currentMode)
        {
            case WeaponMode.Normal:
                currentDelay = normalShotDelay;
                break;
            case WeaponMode.SpreadShot:
                currentDelay = spreadShotDelay;
                break;
            case WeaponMode.ChargeShot:
                currentDelay = chargeShotDelay;
                break;
        }

        // Check if enough time has passed since the last shot
        if (Time.time - lastShotTime >= currentDelay)
        {
            lastShotTime = Time.time; // Update the last shot time
            return true;
        }
        return false;
    }

    void Shoot()
    {
        PlayAmplifiedHonk(0); // Play at base level (no amplification)
        CreateBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, 1f);
    }

    void ShootSpread()
    {
        PlayAmplifiedHonk(0); // Play at base level (no amplification)

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

        // Calculate charge percentage (0 to 1)
        float chargePercentage = chargeDuration / MAX_CHARGE_TIME;

        // Calculate scale multiplier based on charge time
        float scaleMultiplier = 1f + chargePercentage * (MAX_SCALE_MULTIPLIER - 1f);

        // Play the honk with amplification based on charge percentage
        PlayAmplifiedHonk(chargePercentage);

        // Debug information about the charge
        Debug.Log("Charge Duration: " + chargeDuration + "s, Charge %: " + chargePercentage);
        Debug.Log("Scale Multiplier: " + scaleMultiplier);

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

    // Play the honk with amplification based on charge percentage
    void PlayAmplifiedHonk(float chargePercentage)
    {
        if (honk == null || audioSource == null) return;

        // Calculate final volume (lerp from base to max)
        float finalVolume = Mathf.Lerp(baseVolume, maxVolume, chargePercentage);

        // Calculate pitch (lower pitch = more bass)
        float finalPitch = Mathf.Lerp(basePitch, maxPitch, chargePercentage);

        // Apply volume and pitch to main audio source
        audioSource.pitch = finalPitch;
        audioSource.volume = finalVolume;
        audioSource.clip = honk;
        audioSource.Play();

        // Apply distortion effect for charged shots by playing a layered sound
        if (applyDistortion && chargePercentage > 0.5f)
        {
            float distortionIntensity = (chargePercentage - 0.5f) * 2; // Scale from 0-1 based on charge over 50%

            // Setup the distortion source
            distortionSource.clip = honk;
            // Use an even lower pitch for the distortion layer
            distortionSource.pitch = finalPitch * 0.8f;
            // Volume increases with charge
            distortionSource.volume = distortionIntensity * 0.7f;
            // Small delay for echo effect
            distortionSource.PlayDelayed(0.02f);

            Debug.Log("Applied distortion effect with intensity: " + distortionIntensity);
        }

        Debug.Log("Playing amplified honk - Volume: " + finalVolume + ", Pitch: " + finalPitch);
    }

    // Legacy methods kept for backward compatibility
    void PlaySpecificHonk(AudioClip clip, float volumeLevel)
    {
        audioSource.clip = clip;
        audioSource.volume = Mathf.Clamp(volumeLevel, 0f, 1f);
        audioSource.Play();
    }

    void PlayHonk(float volumeLevel)
    {
        PlaySpecificHonk(honk, volumeLevel);
    }
}