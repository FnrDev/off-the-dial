using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    private bool isOn = false;

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.fKey.wasPressedThisFrame)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            Debug.Log("F pressed, flashlight is now: " + isOn);
        }
    }
}