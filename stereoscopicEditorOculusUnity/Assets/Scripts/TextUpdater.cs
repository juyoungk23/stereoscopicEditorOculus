using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TinyGiantStudio.Text;

public class TextUpdater : MonoBehaviour
{
    public TMP_InputField inputField; // Drag your Input Field here in the Inspector

    // Assuming selectedObject is a global static variable that keeps track of the selected object
    public static GameObject selectedObject;

    void Start()
    {
        // If you want to auto-update the input field when the object is selected, you could do it here
        // or wherever you update the selectedObject.
        if (selectedObject != null)
        {
            Modular3DText textComponent = selectedObject.GetComponent<Modular3DText>();
            if (textComponent != null)
            {
                inputField.text = textComponent.Text;
            }
        }

        // Add a listener to detect when the TMP Input Field changes
        inputField.onValueChanged.AddListener(UpdateTextObject);
    }

    // This function is called whenever the TMP Input Field is updated
    void UpdateTextObject(string newText)
    {
        if (selectedObject == null) return;

        // We update the 3D text content here
        Modular3DText textComponent = selectedObject.GetComponent<Modular3DText>();
        if (textComponent != null)
        {
            textComponent.Text = newText;
            textComponent.UpdateText(); // Assuming that UpdateText() applies the changes
        }
    }

    void Update()
    {
        // If you want to update the input field based on the selected object dynamically
        if (selectedObject != null)
        {
            Modular3DText textComponent = selectedObject.GetComponent<Modular3DText>();
            if (textComponent != null)
            {
                if (!inputField.isFocused) // Only update the input field when it's not being edited
                {
                    inputField.text = textComponent.Text;
                }
            }
        }
    }
}
