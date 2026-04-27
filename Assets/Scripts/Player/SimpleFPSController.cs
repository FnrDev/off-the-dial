using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    public float moveSpeed = 4.5f;
    public float sprintMultiplier = 1.7f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float mouseSensitivity = 0.04f;
    public Transform cameraTransform;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip walkClip;
    public AudioClip sprintClip;
    public float walkPitch = 1.0f;
    public float sprintPitch = 1.35f;

    CharacterController controller;
    float pitch;
    float yVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (footstepSource == null) footstepSource = GetComponent<AudioSource>();
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            if (footstepSource.clip == null && walkClip != null) footstepSource.clip = walkClip;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null) return;

        Vector2 mouseDelta = mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
        float mx = mouseDelta.x * mouseSensitivity;
        float my = mouseDelta.y * mouseSensitivity;
        transform.Rotate(0f, mx, 0f);
        if (cameraTransform != null)
        {
            pitch = Mathf.Clamp(pitch - my, -85f, 85f);
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        float h = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                - (kb.aKey.isPressed || kb.leftArrowKey.isPressed  ? 1f : 0f);
        float v = (kb.wKey.isPressed || kb.upArrowKey.isPressed   ? 1f : 0f)
                - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);

        Vector3 input = transform.right * h + transform.forward * v;
        if (input.sqrMagnitude > 1f) input.Normalize();
        float speed = moveSpeed * (kb.leftShiftKey.isPressed ? sprintMultiplier : 1f);

        if (controller.isGrounded)
        {
            yVelocity = -2f;
            if (kb.spaceKey.wasPressedThisFrame)
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        yVelocity += gravity * Time.deltaTime;

        Vector3 motion = input * speed + Vector3.up * yVelocity;
        controller.Move(motion * Time.deltaTime);

        UpdateFootsteps(input.sqrMagnitude > 0.01f, controller.isGrounded, kb.leftShiftKey.isPressed);

        if (kb.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void UpdateFootsteps(bool moving, bool grounded, bool sprinting)
    {
        if (footstepSource == null) return;
        bool shouldPlay = moving && grounded;
        if (shouldPlay)
        {
            var desired = (sprinting && sprintClip != null) ? sprintClip : walkClip;
            if (desired != null && footstepSource.clip != desired)
            {
                footstepSource.clip = desired;
                footstepSource.Play();
            }
            footstepSource.pitch = sprinting ? sprintPitch : walkPitch;
            if (!footstepSource.isPlaying) footstepSource.Play();
        }
        else if (footstepSource.isPlaying)
        {
            footstepSource.Pause();
        }
    }
}
