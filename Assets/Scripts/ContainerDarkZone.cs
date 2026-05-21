using UnityEngine;
using UnityEngine.Rendering;
using UHFPS.Runtime;

// Enables the sea-container dark-zone post-processing volume only after its
// padlock has been unlocked. While the container is still locked the volume
// stays off, so approaching it looks normally lit. isUnlocked is polled so
// this also behaves correctly after a save/reload.
[RequireComponent(typeof(Volume))]
public class ContainerDarkZone : MonoBehaviour
{
    [Tooltip("The PadlockPuzzle that gates this container.")]
    public PadlockPuzzle padlock;

    Volume volume;

    void Awake()
    {
        volume = GetComponent<Volume>();
        Apply();
    }

    void Update()
    {
        Apply();
    }

    void Apply()
    {
        if (volume == null || padlock == null)
            return;

        bool unlocked = padlock.isUnlocked;
        if (volume.enabled != unlocked)
            volume.enabled = unlocked;
    }
}
