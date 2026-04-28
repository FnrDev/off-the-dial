using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightToggle : MonoBehaviour
{
    public Key toggleKey = Key.E;
    public float onAlpha = 1f;
    public float offAlpha = 0.35f;

    MonoBehaviour flashlightItem;
    FieldInfo lightField;
    FieldInfo panelField;
    FieldInfo equippedField;
    MethodInfo setLightState;
    CanvasGroup hudPanel;

    void Awake()
    {
        flashlightItem = GetComponent("FlashlightItem") as MonoBehaviour;
        if (flashlightItem != null)
        {
            var t = flashlightItem.GetType();
            lightField = t.GetField("FlashlightLight", BindingFlags.Public | BindingFlags.Instance);
            setLightState = t.GetMethod("SetLightState", BindingFlags.Public | BindingFlags.Instance);
            panelField = t.GetField("flashlightPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            equippedField = t.GetField("isEquipped", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || flashlightItem == null || lightField == null) return;
        if (!flashlightItem.enabled || !flashlightItem.gameObject.activeInHierarchy) return;

        bool isEquipped = equippedField != null && (bool)equippedField.GetValue(flashlightItem);
        if (!isEquipped) return;

        var light = lightField.GetValue(flashlightItem) as Light;
        if (light == null) return;

        if (kb[toggleKey].wasPressedThisFrame)
        {
            bool newState = !light.enabled;
            if (setLightState != null) setLightState.Invoke(flashlightItem, new object[] { newState });
            else light.enabled = newState;
        }

        SyncHudAlpha(light.enabled);
    }

    void SyncHudAlpha(bool lightOn)
    {
        if (hudPanel == null && panelField != null)
            hudPanel = panelField.GetValue(flashlightItem) as CanvasGroup;
        if (hudPanel == null) return;
        float target = lightOn ? onAlpha : offAlpha;
        if (!Mathf.Approximately(hudPanel.alpha, target))
            hudPanel.alpha = Mathf.MoveTowards(hudPanel.alpha, target, Time.deltaTime * 4f);
    }
}
