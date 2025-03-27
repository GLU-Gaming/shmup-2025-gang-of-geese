using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.right * -bulletSpeed;
        }
        Destroy(gameObject, bulletLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ScoreSystem.instance.AddScore(10);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
