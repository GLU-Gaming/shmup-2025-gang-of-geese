using UnityEngine;

public class Screenshot_Simple : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(Random.Range(0,1000000) + "_screenshot.png", 1);
        }
    }
}
