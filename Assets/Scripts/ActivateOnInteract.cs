using UnityEngine;
using UHFPS.Runtime;

public class ActivateOnInteract : MonoBehaviour, IInteractStart
{
    [Tooltip("GameObject to activate the first time the player presses E on this object.")]
    public GameObject target;

    private bool _activated;

    public void InteractStart()
    {
        if (_activated || target == null) return;
        _activated = true;
        target.SetActive(true);
    }
}
