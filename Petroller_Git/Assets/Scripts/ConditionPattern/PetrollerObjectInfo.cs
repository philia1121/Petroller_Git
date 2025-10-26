using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PetrollerObjectInfo : MonoBehaviour
{
    ControlMap controlMap;

    // For Debug
    public bool showDebug;
    public Material my_mat;
    public GameObject[] dirArrows;
    public GameObject[] rotateAxis;
    enum CoordsType { World, Player, Petroller }
    [SerializeField] private CoordsType logCoords = CoordsType.World;

    // for world and local coords convertion
    public Transform PlayerTransform;
    public Transform PetrollerTransform;

    // Joystick and Button
    public Vector2 JoystickRead { get; private set; }
    public enum JoystickDir
    {
        Origin,
        Ear,
        Tail,
        LeftHand,
        RightHand
    }
    public JoystickDir CurrentJoystickDir { get; private set; } = JoystickDir.Origin;
    public bool VerticalPress { get; private set; }
    public bool HorizontalPress { get; private set; }

    // Controller Motion
    // Velocity
    public Vector3 Velocity { get; private set; }
    public Vector3 PlayerRelativeVelocity { get; private set; }
    public Vector3 PetrollerRelativeVelocity { get; private set; }
    [SerializeField] private float speedThreshold = 0.75f; //0.75f
    [SerializeField] private float moveDirThreshold = 0.3f; //0.3f
    public float Speed { get; private set; }
    public bool IsMoving { get; private set; } = false;
    public Vector3 WorldMoveDirection { get; private set; } = Vector3.zero;
    public Vector3 PlayerRelativeMoveDirection { get; private set; } = Vector3.zero;
    public Vector3 PetrollerRelativeMoveDirection { get; private set; } = Vector3.zero;
    // AngularVelocity
    public Vector3 AngularVelocity { get; private set; }
    public Vector3 PlayerRelativeAngularVelocity { get; private set; } = Vector3.zero;
    public Vector3 PetrollerRelativeAngularVelocity { get; private set; } = Vector3.zero;
    [SerializeField] private float rotateSpeedThreshold = 1f; //1f
    [SerializeField] private float rotateDirThreshold = 0.5f; //0.5f
    public float AngularSpeed { get; private set; }
    public bool IsRotating { get; private set; }
    public Vector3 WorldRotateDirection { get; private set; } = Vector3.zero;
    public Vector3 PlayerRelativeRotateDirection { get; private set; } = Vector3.zero;
    public Vector3 PetrollerRelativeRotateDirection { get; private set; } = Vector3.zero;
    public Vector3 Acceleration { get; private set; }
    public Vector3 AngularAcceleration { get; private set; }

    // Tracking State
    public enum TrackingStatus
    {
        Tracked,
        LostTracked
    }
    public TrackingStatus CurrentTrackingState { get; private set; } = TrackingStatus.Tracked;

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
        controlMap.Petroller.DeviceVelocity.performed += GetDevice_Velocity;
        controlMap.Petroller.DeviceAngularVelocity.performed += GetDevice_AngularVelocity;
        controlMap.Petroller.DeviceAcceleration.performed += GetDevice_Acceleration;
        controlMap.Petroller.DeviceAugularAcceleration.performed += GetDevice_AngularAcceleration;
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
        controlMap.Petroller.DeviceVelocity.performed -= GetDevice_Velocity;
        controlMap.Petroller.DeviceAngularVelocity.performed -= GetDevice_AngularVelocity;
        controlMap.Petroller.DeviceAcceleration.performed -= GetDevice_Acceleration;
        controlMap.Petroller.DeviceAugularAcceleration.performed -= GetDevice_AngularAcceleration;

        controlMap.Petroller.Disable();
    }
    void Update()
    {
        bool isTracked = (OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) & OVRInput.GetControllerOrientationTracked(OVRInput.Controller.RTouch)) ? true : false;
        CurrentTrackingState = isTracked ? TrackingStatus.Tracked : TrackingStatus.LostTracked;
    }
    void GetAction_Pull(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        JoystickRead = value;
        if (value == Vector2.zero)
        {
            CurrentJoystickDir = JoystickDir.Origin;
        }
        else
        {
            float x = Mathf.Abs(value.x);
            float y = Mathf.Abs(value.y);

            if ((x > y) & value.x > 0)
            {
                CurrentJoystickDir = JoystickDir.RightHand;
            }
            else if ((x > y) & value.x < 0)
            {
                CurrentJoystickDir = JoystickDir.LeftHand;
            }
            else if ((x < y) & value.y > 0)
            {
                CurrentJoystickDir = JoystickDir.Ear;
            }
            else if ((x < y) & value.y < 0)
            {
                CurrentJoystickDir = JoystickDir.Tail;
            }
        }
    }
    void GetAction_VerticalPress(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        VerticalPress = value > 0.5f ? true : false;
    }
    void GetAction_HorizontalPress(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        HorizontalPress = value > 0.5f ? true : false;
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
    void GetDevice_Velocity(InputAction.CallbackContext ctx)
    {
        Vector3 value = ctx.ReadValue<Vector3>();
        Velocity = value;
        PlayerRelativeVelocity = PlayerTransform.InverseTransformDirection(Velocity);
        PetrollerRelativeVelocity = PetrollerTransform.InverseTransformDirection(Velocity);

        Speed = Velocity.magnitude;

        if (Speed > speedThreshold)
        {
            IsMoving = true;
            WorldMoveDirection = ConvertRelativeDirection(Velocity, moveDirThreshold);
            PlayerRelativeMoveDirection = ConvertRelativeDirection(PlayerRelativeVelocity, moveDirThreshold);
            PetrollerRelativeMoveDirection = ConvertRelativeDirection(PetrollerRelativeVelocity, moveDirThreshold);

            if (showDebug) my_mat.color = Color.red;
        }
        else
        {
            IsMoving = false;
            WorldMoveDirection = Vector3.zero;
            PlayerRelativeMoveDirection = Vector3.zero;
            PetrollerRelativeMoveDirection = Vector3.zero;

            if (showDebug) my_mat.color = Color.white;
        }

        if (showDebug)
        {
            foreach (var arrow in dirArrows)
            {
                arrow.SetActive(false);
            }

            Vector3 moveDir = Vector3.zero;
            if (logCoords == CoordsType.World) moveDir = WorldMoveDirection;
            if (logCoords == CoordsType.Player) moveDir = PlayerRelativeMoveDirection;
            if (logCoords == CoordsType.Petroller) moveDir = PetrollerRelativeMoveDirection;

            if (moveDir.x > 0) dirArrows[0].SetActive(true);
            if (moveDir.x < 0) dirArrows[1].SetActive(true);
            if (moveDir.y > 0) dirArrows[2].SetActive(true);
            if (moveDir.y < 0) dirArrows[3].SetActive(true);
            if (moveDir.z > 0) dirArrows[4].SetActive(true);
            if (moveDir.z < 0) dirArrows[5].SetActive(true);
        }
    }
    void GetDevice_AngularVelocity(InputAction.CallbackContext ctx)
    {
        Vector3 value = ctx.ReadValue<Vector3>();
        AngularVelocity = value;
        PlayerRelativeAngularVelocity = PlayerTransform.InverseTransformDirection(AngularVelocity);
        PetrollerRelativeAngularVelocity = PetrollerTransform.InverseTransformDirection(AngularVelocity);

        AngularSpeed = AngularVelocity.magnitude;

        if (AngularSpeed > rotateSpeedThreshold)
        {
            IsRotating = true;
            WorldRotateDirection = ConvertRelativeDirection(AngularVelocity, rotateDirThreshold);
            PlayerRelativeRotateDirection = ConvertRelativeDirection(PlayerRelativeAngularVelocity, rotateDirThreshold);
            PetrollerRelativeRotateDirection = ConvertRelativeDirection(PetrollerRelativeAngularVelocity, rotateDirThreshold);
            if (showDebug) my_mat.color = Color.red;
        }
        else
        {
            IsRotating = false;
            WorldRotateDirection = Vector3.zero;
            PlayerRelativeRotateDirection = Vector3.zero;
            PetrollerRelativeRotateDirection = Vector3.zero;

            if (showDebug) my_mat.color = Color.white;
        }

        if (showDebug)
        {
            foreach (var axis in rotateAxis)
            {
                axis.SetActive(false);
            }

            Vector3 rotateDir = Vector3.zero;
            if (logCoords == CoordsType.World) rotateDir = WorldRotateDirection;
            if (logCoords == CoordsType.Player) rotateDir = PlayerRelativeRotateDirection;
            if (logCoords == CoordsType.Petroller) rotateDir = PetrollerRelativeRotateDirection;

            if (Mathf.Abs(rotateDir.x) > 0) rotateAxis[0].SetActive(true);
            if (Mathf.Abs(rotateDir.y) > 0) rotateAxis[1].SetActive(true);
            if (Mathf.Abs(rotateDir.z) > 0) rotateAxis[2].SetActive(true);
        }
    }
    Vector3 ConvertRelativeDirection(Vector3 direction, float threshold)
    {
        Vector3 nm = direction.normalized;

        float dirX = Mathf.Abs(nm.x) > threshold ? (nm.x > 0 ? 1 : -1) : 0;
        float dirY = Mathf.Abs(nm.y) > threshold ? (nm.y > 0 ? 1 : -1) : 0;
        float dirZ = Mathf.Abs(nm.z) > threshold ? (nm.z > 0 ? 1 : -1) : 0;

        return new Vector3(dirX, dirY, dirZ);
    }
    void GetDevice_Acceleration(InputAction.CallbackContext ctx)
    {
        Vector3 value = ctx.ReadValue<Vector3>();
        Acceleration = value;
    }
    void GetDevice_AngularAcceleration(InputAction.CallbackContext ctx)
    {
        Vector3 value = ctx.ReadValue<Vector3>();
        AngularAcceleration = value;
    }
}
