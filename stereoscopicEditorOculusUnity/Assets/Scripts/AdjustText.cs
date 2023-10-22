using UnityEngine;
using TinyGiantStudio.Text;

public class AdjustText : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Slider extrusionSlider; // Fully qualified to avoid ambiguity

    [SerializeField]
    private UnityEngine.UI.Toggle lowercaseToggle;

    [SerializeField]
    private UnityEngine.UI.Toggle capitalizeToggle;

    private GameObject lastSelectedObject;

    void Start()
    {
        if (extrusionSlider != null)
        {
            extrusionSlider.onValueChanged.AddListener(UpdateExtrusion);
        }
        else
        {
            Debug.LogError("ExtrusionSlider is not set. Please assign it in the Inspector.");
        }

        if (lowercaseToggle != null)
        {
            lowercaseToggle.onValueChanged.AddListener(ToggleLowercase);
        }
        else
        {
            Debug.LogError("LowercaseToggle is not set. Please assign it in the Inspector.");
        }

        if (capitalizeToggle != null)
        {
            capitalizeToggle.onValueChanged.AddListener(ToggleCapitalize);
        }
        else
        {
            Debug.LogError("CapitalizeToggle is not set. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        GameObject currentSelectedObject = SelectedObjectTracker.selectedObject;

        if (currentSelectedObject != lastSelectedObject)
        {
            lastSelectedObject = currentSelectedObject;

            if (currentSelectedObject != null)
            {
                Modular3DText textObject = currentSelectedObject.GetComponent<Modular3DText>();
                if (textObject != null)
                {
                    // Update the extrusion slider
                    extrusionSlider.value = textObject.FontSize.z;

                    // Update the toggles based on the properties of Modular3DText
                    lowercaseToggle.isOn = textObject.LowerCase;
                    capitalizeToggle.isOn = textObject.Capitalize;
                }
            }
        }
    }


    private void UpdateExtrusion(float value)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;

        if (selectedObject == null)
        {
            Debug.Log("No object selected. Cannot adjust extrusion.");
            return;
        }

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();

        if (textObject == null)
        {
            Debug.Log("Selected object does not have a Modular3DText component. Cannot adjust extrusion.");
            return;
        }

        Vector3 currentFontSize = textObject.FontSize;
        currentFontSize.z = value;
        textObject.FontSize = currentFontSize;
    }
    public void ToggleLowercase(bool isOn)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;

        if (selectedObject == null)
        {
            Debug.Log("No object selected. Cannot adjust extrusion.");
            return;
        }

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();

        if (textObject == null)
        {
            Debug.Log("Selected object does not have a Modular3DText component. Cannot adjust extrusion.");
            return;
        }

        textObject.LowerCase = !textObject.LowerCase;
    }

    public void ToggleCapitalize(bool isOn)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;

        if (selectedObject == null)
        {
            Debug.Log("No object selected. Cannot adjust extrusion.");
            return;
        }

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();

        if (textObject == null)
        {
            Debug.Log("Selected object does not have a Modular3DText component. Cannot adjust extrusion.");
            return;
        }

        textObject.Capitalize = !textObject.Capitalize;
    }
    private void OnDestroy()
    {
        if (extrusionSlider != null)
        {
            extrusionSlider.onValueChanged.RemoveListener(UpdateExtrusion);
        }

        if (lowercaseToggle != null)
        {
            lowercaseToggle.onValueChanged.RemoveListener(ToggleLowercase);
        }

        if (capitalizeToggle != null)
        {
            capitalizeToggle.onValueChanged.RemoveListener(ToggleCapitalize);
        }
    }
}
