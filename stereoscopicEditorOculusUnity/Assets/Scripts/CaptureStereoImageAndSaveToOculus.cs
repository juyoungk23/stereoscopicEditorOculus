using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CaptureStereoImageAndSaveToOculus : MonoBehaviour
{
    public Camera virtualCamera; // Drag your main camera here in inspector
    public Button captureButton; // Drag your UI Button here in inspector
    private Camera leftCamera;
    private string folderPath;

    void Start()
    {
        // For Oculus Quest, /sdcard/Pictures/ is generally accessible
        folderPath = "/sdcard/Pictures/";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Clone the main camera GameObject to create a temporary camera for the left eye view
        GameObject leftCameraGameObject = Instantiate(virtualCamera.gameObject, virtualCamera.transform.parent);
        leftCameraGameObject.name = "LeftCamera";
        leftCamera = leftCameraGameObject.GetComponent<Camera>();
        leftCameraGameObject.transform.localPosition += new Vector3(-0.032f, 0, 0); // Move it to the left of the main camera

        // Register the Capture function to the button's onClick event
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(Capture);
        }
    }

    void Update()
    {
        // This part can be kept for keyboard support or removed
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            Capture();
        }
    }

    void Capture()
    {
        // Set a custom high resolution
        int width = 7680; // Example for 8K width
        int height = 4320; // Example for 8K height

        // Create render textures with HDR support
        RenderTexture rtRight = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf);
        RenderTexture rtLeft = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf);

        rtRight.antiAliasing = 1; // Disabling MSAA to allow TAA to work
        rtLeft.antiAliasing = 1;  // Disabling MSAA to allow TAA to work
        rtRight.useMipMap = true;
        rtLeft.useMipMap = true;

        virtualCamera.targetTexture = rtRight;
        leftCamera.targetTexture = rtLeft;
        virtualCamera.Render();
        leftCamera.Render();

        // Create a new texture to store the final stereo image
        Texture2D stereoTexture = new Texture2D(width * 2, height, TextureFormat.RGB24, false);

        // Read the pixel values and store them in the texture
        RenderTexture.active = rtRight;
        stereoTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = rtLeft;
        stereoTexture.ReadPixels(new Rect(0, 0, width, height), width, 0);
        stereoTexture.Apply();

        // Save the captured image
        byte[] bytes = stereoTexture.EncodeToPNG();
        string filename = folderPath + "StereoCapture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        File.WriteAllBytes(filename, bytes);
        Debug.Log($"Saved to {filename}");

        // Cleanup
        RenderTexture.active = null;
        virtualCamera.targetTexture = null;
        leftCamera.targetTexture = null;
        Destroy(rtRight);
        Destroy(rtLeft);
        Destroy(stereoTexture);
    }
}
