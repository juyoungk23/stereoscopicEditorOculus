using UnityEngine;
using TinyGiantStudio.Text;

public class AdjustText : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Slider extrusionSlider;

    [SerializeField]
    private UnityEngine.UI.Toggle lowercaseToggle;

    [SerializeField]
    private UnityEngine.UI.Toggle capitalizeToggle;

    private GameObject lastSelectedObject;

    void Start()
    {
        extrusionSlider.onValueChanged.AddListener(UpdateExtrusion);
        lowercaseToggle.onValueChanged.AddListener(ToggleLowercase);
        capitalizeToggle.onValueChanged.AddListener(ToggleCapitalize);
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
                    // Update extrusion slider
                    extrusionSlider.value = textObject.FontSize.z;

                    // Update toggles
                    lowercaseToggle.isOn = textObject.LowerCase;
                    capitalizeToggle.isOn = textObject.Capitalize;
                }
            }
        }
    }

    private void UpdateExtrusion(float value)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;
        if (selectedObject == null) return;

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();
        if (textObject == null) return;

        Vector3 currentFontSize = textObject.FontSize;
        currentFontSize.z = value;
        textObject.FontSize = currentFontSize;
    }

    public void ToggleLowercase(bool isOn)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;
        if (selectedObject == null) return;

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();
        if (textObject == null) return;

        textObject.LowerCase = isOn;  // Set based on toggle status
    }

    public void ToggleCapitalize(bool isOn)
    {
        GameObject selectedObject = SelectedObjectTracker.selectedObject;
        if (selectedObject == null) return;

        Modular3DText textObject = selectedObject.GetComponent<Modular3DText>();
        if (textObject == null) return;

        textObject.Capitalize = isOn;  // Set based on toggle status
    }

    private void OnDestroy()
    {
        extrusionSlider.onValueChanged.RemoveListener(UpdateExtrusion);
        lowercaseToggle.onValueChanged.RemoveListener(ToggleLowercase);
        capitalizeToggle.onValueChanged.RemoveListener(ToggleCapitalize);
    }
}
