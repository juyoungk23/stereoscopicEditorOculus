using UnityEngine;
using UnityEngine.UI;

public class CubeInstantiator : MonoBehaviour
{
    public GameObject cubePrefab; // Assign a cube prefab in the Unity inspector

    private void Start()
    {
        // Get the Button component and add the listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(InstantiateCube);
        }
        else
        {
            Debug.LogWarning("Button component not found on this GameObject.");
        }
    }

    public void InstantiateCube()
    {
        // Instantiate the cube at a random position within 5 units from the origin
        Vector3 position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        Instantiate(cubePrefab, position, Quaternion.identity);
    }
}
