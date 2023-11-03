using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BNG;  // Make sure to include this namespace to access PointerEvents

public class PointerDownLogger : MonoBehaviour
{
    private PointerEvents pointerEvents;

    void Start()
    {
        // Find the PointerEvents script attached to this GameObject
        pointerEvents = GetComponent<PointerEvents>();

        // If PointerEvents is found, subscribe to the OnPointerDownEvent
        if (pointerEvents != null)
        {
            pointerEvents.OnPointerDownEvent.AddListener(LogPointerDown);
        }
        else
        {
            Debug.LogWarning("No PointerEvents script found on this GameObject.");
        }
    }

    // Function that will be called when OnPointerDownEvent is fired
    public void LogPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer down");
    }

    // Unsubscribe from the OnPointerDownEvent when the GameObject is destroyed
    void OnDestroy()
    {
        if (pointerEvents != null)
        {
            pointerEvents.OnPointerDownEvent.RemoveListener(LogPointerDown);
        }
    }
}
