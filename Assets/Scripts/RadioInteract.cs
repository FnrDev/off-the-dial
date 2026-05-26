using UnityEngine;
using UHFPS.Runtime;

public class RadioInteract : MonoBehaviour
{
    public AudioSource radioAudioSource;
    public AudioClip dinerPuzzleClip;
    public DialogueTrigger dialogueTrigger;

    private bool subtitleTriggered = false;

    void Update()
    {
        if (subtitleTriggered) return;

        if (radioAudioSource != null &&
            radioAudioSource.isPlaying &&
            radioAudioSource.clip == dinerPuzzleClip)
        {
            subtitleTriggered = true;

            if (dialogueTrigger != null)
                dialogueTrigger.TriggerDialogue();
        }
    }
}