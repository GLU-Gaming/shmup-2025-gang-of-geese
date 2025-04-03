using UnityEngine;

public class FragmentMovement : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}

