
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform targetTransform;
    private float moveSpeed = 3f;
    private bool hasTarget = false;
    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (hasTarget && targetTransform != null)
        {
            MoveTowardsTarget();
        }
    }

    public void SetTarget(Transform target, float speed)
    {
        targetTransform = target;
        moveSpeed = speed;
        hasTarget = true;
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = targetTransform.position - transform.position;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            // Remove or comment out the following lines to prevent rotation
            //if (direction != Vector3.zero)
            //{
            //    Quaternion targetRotation = Quaternion.LookRotation(direction);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            //}

            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveSpeed", moveSpeed);
            }
        }
        else
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
            }

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
}
