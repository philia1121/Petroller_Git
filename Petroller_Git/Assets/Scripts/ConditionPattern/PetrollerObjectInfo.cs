using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PetrollerObjectInfo : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public Vector3 AngularVelocity { get; private set; }
    public float Speed => Velocity.magnitude;
    public float AngularSpeed => AngularVelocity.magnitude;
    ControlMap controlMap;

    public Vector2 joystickRead;
    public bool verticalPress, horizontalPress;
    public enum MovementState
    {
        Idle,
        Moving
    }
    public MovementState CurrentMovementState { get; private set; } = MovementState.Idle;

    void Awake()
    {
        controlMap = new ControlMap();   
    }
    void OnEnable()
    {
        controlMap.Petroller.Enable();

        controlMap.Petroller.Pull.started += GetAction_Pull;
        controlMap.Petroller.Pull.performed += GetAction_Pull;
        controlMap.Petroller.Pull.canceled += GetAction_Pull;
        controlMap.Petroller.VerticalPress.started += GetAction_VerticalPress;
        controlMap.Petroller.VerticalPress.canceled += GetAction_VerticalPress;
        controlMap.Petroller.HorizontalPress.started += GetAction_HorizontalPress;
        controlMap.Petroller.HorizontalPress.canceled += GetAction_HorizontalPress;
        controlMap.Petroller.Pos.performed += GetPos;
        controlMap.Petroller.Rot.performed += GetRot;
    }
    void OnDisable()
    {
        controlMap.Petroller.Pull.started -= GetAction_Pull;
        controlMap.Petroller.Pull.performed -= GetAction_Pull;
        controlMap.Petroller.Pull.canceled -= GetAction_Pull;
        controlMap.Petroller.VerticalPress.started -= GetAction_VerticalPress;
        controlMap.Petroller.VerticalPress.canceled -= GetAction_VerticalPress;
        controlMap.Petroller.HorizontalPress.started -= GetAction_HorizontalPress;
        controlMap.Petroller.HorizontalPress.canceled -= GetAction_HorizontalPress;
        controlMap.Petroller.Pos.performed -= GetPos;
        controlMap.Petroller.Rot.performed -= GetRot;

        controlMap.Petroller.Disable();
    }

    void GetAction_Pull(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        joystickRead = value;
    }
    void GetAction_VerticalPress(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        bool pressed = value > 0.5f ? true : false;
        verticalPress = pressed;
    }
    void GetAction_HorizontalPress(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        bool pressed = value > 0.5f ? true : false;
        horizontalPress = pressed;
    }
    void GetPos(InputAction.CallbackContext ctx)
    {
        Vector3 value = ctx.ReadValue<Vector3>();
        transform.position = value;
    }
    void GetRot(InputAction.CallbackContext ctx)
    {
        Quaternion value = ctx.ReadValue<Quaternion>();
        transform.rotation = value;
    }
}
