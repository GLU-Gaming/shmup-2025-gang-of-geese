using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // Scroll speed
    public float backgroundLength = 10f; // The length of the background image along the z-axis.

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;  // Save the starting position of the background.
    }

    void Update()
    {
        // Move the background backward along the z-axis
        transform.Translate(Vector3.back * scrollSpeed * Time.deltaTime);

        // If the background has moved past the point of looping, reset its position
        if (transform.position.z <= startPosition.z - backgroundLength)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, startPosition.z);
        }
    }
}
