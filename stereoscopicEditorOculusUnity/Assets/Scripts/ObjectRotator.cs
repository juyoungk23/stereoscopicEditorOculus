using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using BNG;

public class ObjectRotator : MonoBehaviour
{
    [Header("Input")]
    public bool AllowInput = true;
    public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.RightThumbStickAxis };
    public InputActionReference RotateAction;

    [Header("Smooth / Snap Turning")]
    public RotationMechanic RotationType = RotationMechanic.Snap;

    [Header("Snap Turn Settings")]
    public float SnapRotationAmount = 45f;
    public float SnapInputAmount = 0.75f;

    [Header("Smooth Turn Settings")]
    public float SmoothTurnSpeed = 40f;
    public float SmoothTurnMinInput = 0.1f;

    [Header("Dolly Settings")]
    public float DollySpeed = 1f; // Speed at which the object will dolly

    private float rotationAmount = 0;
    private float xAxis;
    private float yAxis; // Variable for the y-axis input for dollying
    private float previousXInput;

    public UIPointer uiPointer;  // Reference to the UIPointer script

    private PointerEvents pointerEvents;
    private GameObject currentTargetObject = null;

    void Start()
    {
        pointerEvents = GetComponent<PointerEvents>();
        if (pointerEvents != null)
        {
            pointerEvents.OnPointerDownEvent.AddListener((eventData) => { ManipulateObject(gameObject, true); });
            pointerEvents.OnPointerUpEvent.AddListener((eventData) => { ManipulateObject(null, false); });
        }
        else
        {
            Debug.LogWarning("No PointerEvents script found on this GameObject.");
        }
    }

    public void ManipulateObject(GameObject targetObject, bool isPointerDown)
    {
        currentTargetObject = isPointerDown ? targetObject : null;
    }

    void Update()
    {
        if (!AllowInput || currentTargetObject == null)
        {
            return;
        }

        // Read the value from the RotateAction input for both rotation and dollying
        Vector2 inputVector = RotateAction.action.ReadValue<Vector2>();
        xAxis = inputVector.x;
        yAxis = inputVector.y;

        if (uiPointer.data != null)
        {
            GameObject hitObject = uiPointer.data.pointerCurrentRaycast.gameObject;

            if (hitObject == currentTargetObject)
            {
                if (RotationType == RotationMechanic.Snap)
                {
                    DoSnapRotation(xAxis);
                }
                else if (RotationType == RotationMechanic.Smooth)
                {
                    DoSmoothRotation(xAxis);
                }

                // Apply dolly movement based on y-axis input
                DoDolly(yAxis);
            }
        }

        previousXInput = xAxis;
    }

    public virtual void DoSnapRotation(float xInput)
    {
        rotationAmount = 0;

        if (xInput >= SnapInputAmount && previousXInput < SnapInputAmount)
        {
            rotationAmount += SnapRotationAmount;
        }
        else if (xInput <= -SnapInputAmount && previousXInput > -SnapInputAmount)
        {
            rotationAmount -= SnapRotationAmount;
        }

        if (Math.Abs(rotationAmount) > 0)
        {
            currentTargetObject.transform.Rotate(0, rotationAmount, 0, Space.World);
        }
    }

    public virtual void DoSmoothRotation(float xInput)
    {
        rotationAmount = xInput * SmoothTurnSpeed * Time.deltaTime;
        currentTargetObject.transform.Rotate(0, rotationAmount, 0, Space.World);
    }

    // Method to handle dolly movement
    public void DoDolly(float yInput)
    {
        if (Mathf.Abs(yInput) > SmoothTurnMinInput)
        {
            currentTargetObject.transform.Translate(0, 0, yInput * DollySpeed * Time.deltaTime, Space.Self);
        }
    }
}
