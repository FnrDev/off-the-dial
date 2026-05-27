using UnityEngine;
using UHFPS.Runtime;

public class OfficeRadioDialogueTrigger : MonoBehaviour, IInteractStart
{
    public AudioSource radioAudioSource;
    public AudioClip broadcastClip;
    public DialogueTrigger dialogueTrigger;
    public bool TriggerOnInteract;

    private bool triggered = false;

    public void InteractStart()
    {
        if (!TriggerOnInteract) return;
        Fire();
    }

    private void Update()
    {
        if (triggered || TriggerOnInteract) return;

        if (radioAudioSource != null &&
            radioAudioSource.isPlaying &&
            (broadcastClip == null || radioAudioSource.clip == broadcastClip))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (triggered) return;
        triggered = true;
        if (dialogueTrigger != null)
            dialogueTrigger.TriggerDialogue();
    }
}