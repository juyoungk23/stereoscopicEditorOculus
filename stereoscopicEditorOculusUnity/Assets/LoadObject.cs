using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.Networking;
using TinyGiantStudio.Text;
using VRUIP;


[System.Serializable]
public class SceneData2
{
    public string sceneName;
    public List<ObjectData2> objects;
}

[System.Serializable]
public class ObjectData2
{
    public string id;
    public string assetBundleName;
    public string objectType;  // "Cube", "Sphere", "Gear", "Text", etc.
    public Vector3Data2 position;
    public Vector3Data2 rotation;
    public Vector3Data2 scale;
}


[System.Serializable]
public class Vector3Data2
{
    public float x;
    public float y;
    public float z;
}

public class LoadObject : MonoBehaviour
{
    private FirebaseStorage storage;
    private const string sceneJsonPath = "gs://stereoscopiceditor.appspot.com/YourScene.json";
    public const string assetBundleBasePath = "gs://stereoscopiceditor.appspot.com/";

    private Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();  // Add this line

    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        // LoadSceneData2();
    }

    private void LoadSceneData2()
    {
        // Get the storage reference for scene JSON
        StorageReference sceneJsonRef = storage.GetReferenceFromUrl(sceneJsonPath);

        // Fetch scene JSON
        sceneJsonRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get download URL: " + task.Exception);
                return;
            }

            string downloadUrl = task.Result.ToString();
            StartCoroutine(DownloadAndProcessScene(downloadUrl));
        });
    }

    private IEnumerator DownloadAndProcessScene(string downloadUrl)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(downloadUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download scene data: " + www.error);
                yield break;
            }

            // Deserialize JSON to SceneData2 object
            string jsonContent = www.downloadHandler.text;
            SceneData2 sceneData2 = JsonUtility.FromJson<SceneData2>(jsonContent);


            // Process the scene data
            ProcessSceneData2(sceneData2);
        }
    }

    private void ProcessSceneData2(SceneData2 sceneData2)
    {
        foreach (ObjectData2 objectData2 in sceneData2.objects)
        {
            string assetBundleUrl = assetBundleBasePath + objectData2.assetBundleName; // Assuming URL based on assetBundleName
            StartCoroutine(DownloadAndInstantiateObject(objectData2, assetBundleUrl));
        }
    }

    public IEnumerator DownloadAndInstantiateObject(ObjectData2 objectData2, string assetBundleUrl, string newTextContent = null)
    {
        // Get the storage reference for AssetBundle
        StorageReference assetBundleRef = storage.GetReferenceFromUrl(assetBundleUrl);

        // Fetch AssetBundle
        assetBundleRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get download URL for AssetBundle: " + task.Exception);
                return;
            }

            string downloadUrl = task.Result.ToString();

            // Download AssetBundle and instantiate
            StartCoroutine(DownloadAssetBundleAndInstantiate(downloadUrl, objectData2, newTextContent));
        });

        yield return null;
    }
    private IEnumerator DownloadAssetBundleAndInstantiate(string assetBundleUrl, ObjectData2 objectData2, string newTextContent = null)
    {
        AssetBundle bundle;

        if (loadedAssetBundles.TryGetValue(assetBundleUrl, out bundle))
        {
            Debug.Log("AssetBundle already loaded, reusing.");
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUrl))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to download AssetBundle: " + www.error);
                    yield break;
                }

                bundle = DownloadHandlerAssetBundle.GetContent(www);
                loadedAssetBundles.Add(assetBundleUrl, bundle);
            }
        }

        if (bundle == null)
        {
            Debug.LogError("AssetBundle is null");
            yield break;
        }

        GameObject prefab = bundle.LoadAsset<GameObject>(objectData2.assetBundleName);
        Vector3 position = new Vector3(objectData2.position.x, objectData2.position.y, objectData2.position.z);
        Quaternion rotation = Quaternion.Euler(objectData2.rotation.x, objectData2.rotation.y, objectData2.rotation.z);
        Vector3 scale = new Vector3(objectData2.scale.x, objectData2.scale.y, objectData2.scale.z);

        GameObject go = Instantiate(prefab, position, rotation);
        go.transform.localScale = scale;

        if (objectData2.objectType == "Text")
        {
            Modular3DText textComponent = go.GetComponent<Modular3DText>();
            if (textComponent != null)
            {
                // Add a BoxCollider here
                go.AddComponent<BoxCollider>();

                Material originalMaterial = textComponent.Material;
                originalMaterial.shader = Shader.Find(originalMaterial.shader.name);

                Debug.Log("Original Material for Text: " + originalMaterial.name);
                textComponent.Material = originalMaterial;
                Debug.Log("Material reapplied to Text: " + textComponent.Material.name);

                // Get existing MeshRenderer and ColorChanger components
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                VRUIP.ColorChanger colorChanger = go.GetComponent<VRUIP.ColorChanger>();

                if (meshRenderer != null && colorChanger != null)
                {
                    // Link the existing MeshRenderer to the ColorChanger
                    Debug.Log("Mesh Renderer and Color Changer Linked!");
                    colorChanger.SetMeshRenderer(meshRenderer);
                }
                else
                {
                    Debug.LogWarning("MeshRenderer or ColorChanger component not found on the GameObject.");
                }

                if (!string.IsNullOrEmpty(newTextContent))
                {
                    textComponent.Text = newTextContent;
                    textComponent.UpdateText();
                }
                yield break;
            }
            else
            {
                Debug.LogWarning("Modular3DText component not found on the GameObject.");
            }
        }

        yield return null;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            Debug.Log("Number of materials in Renderer: " + materials.Length);
            for (int i = 0; i < materials.Length; i++)
            {
                Material originalMaterial = materials[i];
                Shader originalShader = originalMaterial.shader;
                Debug.Log("Original Material: " + originalMaterial.name);
                Debug.Log("Original Shader: " + originalShader.name);
                originalMaterial.shader = Shader.Find(originalShader.name);
                Debug.Log("Shader reapplied: " + originalMaterial.shader.name);
            }
            renderer.materials = materials;
            Debug.Log("Materials reapplied to Renderer.");
        }
        else
        {
            Debug.LogWarning("Renderer component not found on the GameObject.");
        }
    }

}
