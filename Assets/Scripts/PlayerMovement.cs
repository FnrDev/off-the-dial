using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float mouseSensitivity = 2f;

    [Header("Noise (Creature AI will read this later)")]
    public float currentNoiseLevel = 0f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseLook();
        Move();
        Crouch();
    }

    void MouseLook()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * 0.1f;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * 0.1f;

        transform.Rotate(0, mouseX, 0);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 70f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Move()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float h = 0f;
        float v = 0f;

        if (keyboard.aKey.isPressed) h = -1f;
        if (keyboard.dKey.isPressed) h =  1f;
        if (keyboard.sKey.isPressed) v = -1f;
        if (keyboard.wKey.isPressed) v =  1f;

        bool sprinting = keyboard.leftShiftKey.isPressed && !isCrouching;

        float speed = isCrouching ? crouchSpeed :
                      sprinting   ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(Vector3.down * 9.8f * Time.deltaTime);

        // Noise level
        if (move.magnitude > 0.1f)
        {
            if (isCrouching)    currentNoiseLevel = 0.1f;
            else if (sprinting) currentNoiseLevel = 1.0f;
            else                currentNoiseLevel = 0.4f;
        }
        else
        {
            currentNoiseLevel = 0f;
        }
    }

    void Crouch()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftCtrlKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? 1f : 2f;
        }
    }
}