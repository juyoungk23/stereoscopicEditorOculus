using UnityEngine;

public class SphereInstantiator : MonoBehaviour
{
    public GameObject spherePrefab; // Drag your Sphere Prefab here in the inspector

    // This function will instantiate a sphere at position (0, 0, 0)
    public void InstantiateSphere()
    {
        // Instantiate the sphere at (0, 0, 0) with no rotation
        Instantiate(spherePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
