using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleInventoryUI : MonoBehaviour
{
    public GameObject panel;
    public Key toggleKey = Key.Tab;
    public MonoBehaviour playerController;
    public bool pauseTime = false;

    bool isOpen;
    CursorLockMode prevLock;
    bool prevCursorVisible;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || panel == null) return;
        if (kb[toggleKey].wasPressedThisFrame) Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);
        if (isOpen)
        {
            prevLock = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerController != null) playerController.enabled = false;
            if (pauseTime) Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = prevLock;
            Cursor.visible = prevCursorVisible;
            if (playerController != null) playerController.enabled = true;
            if (pauseTime) Time.timeScale = 1f;
        }
    }
}
