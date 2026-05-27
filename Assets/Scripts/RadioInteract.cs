using UnityEngine;
using UHFPS.Runtime;

public class RadioInteract : MonoBehaviour, IInteractStart
{
    public AudioSource radioAudioSource;
    public AudioClip dinerPuzzleClip;
    public DialogueTrigger dialogueTrigger;

    public bool TriggerOnInteract;

    private bool subtitleTriggered = false;

    public void InteractStart()
    {
        if (!TriggerOnInteract) return;
        Fire();
    }

    void Update()
    {
        if (subtitleTriggered || TriggerOnInteract) return;

        if (radioAudioSource != null &&
            radioAudioSource.isPlaying &&
            (dinerPuzzleClip == null || radioAudioSource.clip == dinerPuzzleClip))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (subtitleTriggered) return;
        subtitleTriggered = true;
        Debug.Log("RadioInteract Fire() called!");
        if (dialogueTrigger != null)
            dialogueTrigger.TriggerDialogue();
        else
            Debug.Log("dialogueTrigger is NULL!");
    }
}
