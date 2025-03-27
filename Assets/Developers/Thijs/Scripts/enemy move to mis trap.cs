using UnityEngine;

public class enemymovetomistrap : MonoBehaviour
{
    [SerializeField] private float avoidanceSpeed = 3f;
    [SerializeField] private Vector2 avoidanceDirection = Vector2.right;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AvoidTrap()
    {
        rb.linearVelocity = avoidanceDirection.normalized * avoidanceSpeed;
    }

}
