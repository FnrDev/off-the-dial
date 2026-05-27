using UnityEngine;
using UHFPS.Runtime;

public class ChurchRadioDialogueTrigger : MonoBehaviour, IInteractStart
{
    public AudioSource radioAudioSource;
    public AudioClip broadcastClip;       // drag your ChurchRadio.mp3 here
    public DialogueTrigger dialogueTrigger;
    public bool TriggerOnInteract;

    [Header("Sound Width")]
    [Range(0f, 360f)]
    [Tooltip("Stereo spread in degrees. 0 = mono point source, 360 = full surround. Try 80–120 for a wide room-filling radio feel.")]
    public float stereoSpread = 90f;
    [Range(0f, 1f)]
    [Tooltip("0 = fully 2D (same volume everywhere), 1 = fully 3D positional. Lower this to make the radio feel less like a point source.")]
    public float spatialBlend = 0.6f;

    // Tracks whether dialogue has already fired for the current audio playback
    // so the Update() path doesn't spam-trigger every frame while audio is playing.
    // Resets automatically when audio stops, so the next play fires dialogue again.
    private bool dialogueFiredForCurrentPlay = false;

    // Called by UHFPS when player presses E on this object
    public void InteractStart()
    {
        if (!TriggerOnInteract) return;
        Fire();
    }

    private void Update()
    {
        if (TriggerOnInteract) return;

        if (radioAudioSource != null &&
            radioAudioSource.isPlaying &&
            (broadcastClip == null || radioAudioSource.clip == broadcastClip))
        {
            if (!dialogueFiredForCurrentPlay)
            {
                dialogueFiredForCurrentPlay = true;
                Fire();
            }
        }
        else
        {
            // Audio stopped — reset so the next playback fires dialogue again
            dialogueFiredForCurrentPlay = false;
        }
    }

    private void Fire()
    {
        // Play the church hint audio (restarts from beginning each time)
        if (radioAudioSource != null && broadcastClip != null)
        {
            radioAudioSource.spread      = stereoSpread;
            radioAudioSource.spatialBlend = spatialBlend;
            radioAudioSource.clip        = broadcastClip;
            radioAudioSource.Play();
        }

        // Reset the dialogue trigger so it can fire again, then trigger it
        if (dialogueTrigger != null)
        {
            dialogueTrigger.ResetDialogue();
            dialogueTrigger.TriggerDialogue();
        }
    }
}
