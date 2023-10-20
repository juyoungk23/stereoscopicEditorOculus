using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SelectedObjectTracker : MonoBehaviour
{
    public static GameObject selectedObject;  // Static reference, accessible from other scripts
    public TMP_Text displayText;  // Reference to your TextMeshPro text component

    // This function can be called from other scripts to set the selected object
    public void UpdateSelectedObject(GameObject newSelectedObject)
    {
        selectedObject = newSelectedObject;

        if (selectedObject != null)
        {
            displayText.text = selectedObject.name;
            Debug.Log("Selected object: " + selectedObject.name);
        }
        else
        {
            displayText.text = "No Object Selected";
            Debug.Log("No object selected");
        }
    }
}
