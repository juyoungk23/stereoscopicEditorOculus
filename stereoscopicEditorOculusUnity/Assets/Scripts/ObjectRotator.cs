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
    public InputActionReference DollyAction; // New Input Action for dollying the object

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
    private float yAxis; // New variable for the y-axis input
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

        xAxis = GetAxisInput();
        yAxis = GetDollyInput(); // Get the y-axis input for dollying

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

    // Method to get the y-axis value for dollying from the DollyAction
    public float GetDollyInput()
    {
        if (DollyAction != null)
        {
            return DollyAction.action.ReadValue<Vector2>().y;
        }

        return 0f;
    }
    public virtual float GetAxisInput()
    {
        float lastVal = 0;

        if (inputAxis != null)
        {
            for (int i = 0; i < inputAxis.Count; i++)
            {
                float axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]).x;
                if (lastVal == 0)
                {
                    lastVal = axisVal;
                }
                else if (axisVal != 0 && axisVal > lastVal)
                {
                    lastVal = axisVal;
                }
            }
        }

        if (RotateAction != null)
        {
            float axisVal = RotateAction.action.ReadValue<Vector2>().x;
            if (lastVal == 0)
            {
                lastVal = axisVal;
            }
            else if (axisVal != 0 && axisVal > lastVal)
            {
                lastVal = axisVal;
            }
        }

        return lastVal;
    }

    public virtual void DoSnapRotation(float xInput)
    {
        rotationAmount = 0;

        if (xInput >= 0.1f && previousXInput < 0.1f)
        {
            rotationAmount += SnapRotationAmount;
        }
        else if (xInput <= -0.1f && previousXInput > -0.1f)
        {
            rotationAmount -= SnapRotationAmount;
        }

        if (Math.Abs(rotationAmount) > 0)
        {
            currentTargetObject.transform.rotation = Quaternion.Euler(
                new Vector3(
                    currentTargetObject.transform.eulerAngles.x,
                    currentTargetObject.transform.eulerAngles.y + rotationAmount,
                    currentTargetObject.transform.eulerAngles.z));
        }
    }

    public virtual void DoSmoothRotation(float xInput)
    {
        rotationAmount = 0;

        if (xInput >= SmoothTurnMinInput)
        {
            rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
        }
        else if (xInput <= -SmoothTurnMinInput)
        {
            rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
        }

        currentTargetObject.transform.rotation = Quaternion.Euler(
            new Vector3(
                currentTargetObject.transform.eulerAngles.x,
                currentTargetObject.transform.eulerAngles.y + rotationAmount,
                currentTargetObject.transform.eulerAngles.z));
    }

    // New method to handle dolly movement
    public void DoDolly(float yInput)
    {
        if (Math.Abs(yInput) > SmoothTurnMinInput)
        {
            Vector3 dollyDirection = currentTargetObject.transform.forward * yInput * DollySpeed * Time.deltaTime;
            currentTargetObject.transform.position += dollyDirection;
        }
    }
}
