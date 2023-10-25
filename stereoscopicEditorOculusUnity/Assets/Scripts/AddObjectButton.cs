using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AddObjectButton : MonoBehaviour
{
    public SceneLoader sceneLoader;  // Reference to your SceneLoader script
    public Button addCubeButton;  // Reference to your UI Button for adding a cube
    public Button addSphereButton;  // Reference to your UI Button for adding a sphere
    public Button addAppleButton;  // Reference to your UI Button for adding a gear
    public Button addTextButton;
    public TMP_InputField inputField;
    // Object Data for Cube
    private ObjectData cubeData = new ObjectData
    {
        id = "newCube",
        assetBundleName = "cube",
        objectType = "cube",
        position = new Vector3Data { x = 0, y = 1, z = 0 },
        rotation = new Vector3Data { x = 0, y = 0, z = 0 },
        scale = new Vector3Data { x = 1, y = 1, z = 1 }
    };

    // Object Data for Sphere
    private ObjectData sphereData = new ObjectData
    {
        id = "newSphere",
        assetBundleName = "sphere",
        objectType = "sphere",
        position = new Vector3Data { x = 2, y = 1, z = 0 },
        rotation = new Vector3Data { x = 0, y = 0, z = 0 },
        scale = new Vector3Data { x = 1, y = 1, z = 1 }
    };

    // Object Data for Gear
    private ObjectData appleData = new ObjectData
    {
        id = "newapple",
        assetBundleName = "apple",
        objectType = "apple",
        position = new Vector3Data { x = 4, y = 1, z = 0 },
        rotation = new Vector3Data { x = 0, y = 0, z = 0 },
        scale = new Vector3Data { x = 1, y = 1, z = 1 }
    };

    private ObjectData textData = new ObjectData
    {
        id = "newText",
        assetBundleName = "text",
        objectType = "Text",
        position = new Vector3Data { x = 3, y = 1, z = 0 },
        rotation = new Vector3Data { x = 0, y = 0, z = 0 },
        scale = new Vector3Data { x = 1, y = 1, z = 1 }
    };


    void Start()
    {
        // Attach the OnClick behavior for each button
        addCubeButton.onClick.AddListener(OnAddCubeButtonClick);
        addSphereButton.onClick.AddListener(OnAddSphereButtonClick);
        addAppleButton.onClick.AddListener(OnAddAppleButtonClick);
        addTextButton.onClick.AddListener(OnAddTextButtonClick);
    }

    public void OnAddCubeButtonClick()
    {
        Debug.Log("OnAddCubeButtonClick called");
        string assetBundleUrl = SceneLoader.assetBundleBasePath + cubeData.assetBundleName;
        StartCoroutine(sceneLoader.DownloadAndInstantiateObject(cubeData, assetBundleUrl));
    }

    public void OnAddSphereButtonClick()
    {
        Debug.Log("OnAddSphereButtonClick called");
        string assetBundleUrl = SceneLoader.assetBundleBasePath + sphereData.assetBundleName;
        StartCoroutine(sceneLoader.DownloadAndInstantiateObject(sphereData, assetBundleUrl));
    }

    public void OnAddAppleButtonClick()
    {
        Debug.Log("OnAddAppleButtonClick called");
        string assetBundleUrl = SceneLoader.assetBundleBasePath + appleData.assetBundleName;
        StartCoroutine(sceneLoader.DownloadAndInstantiateObject(appleData, assetBundleUrl));
    }

    public void OnAddTextButtonClick()
    {
        string newTextContent = inputField.text;  // Get text from input field

        // if (string.IsNullOrEmpty(newTextContent))
        // {
        //     Debug.LogWarning("Input field is empty. Not adding text object.");
        //     return;
        // }

        if (string.IsNullOrEmpty(newTextContent))
        {
            newTextContent = "Hello World!";
        }

        Debug.Log("OnAddTextButtonClick called");
        string assetBundleUrl = SceneLoader.assetBundleBasePath + textData.assetBundleName;
        StartCoroutine(sceneLoader.DownloadAndInstantiateObject(textData, assetBundleUrl, newTextContent));  // Pass the new text content
    }
}
