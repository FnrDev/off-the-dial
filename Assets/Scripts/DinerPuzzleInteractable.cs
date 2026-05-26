using UnityEngine;

public class DinerPuzzleInteractable : MonoBehaviour
{
    [Header("Puzzle")]
    public int objectID;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Prompt")]
    public GameObject interactPrompt;

    private bool playerInRange = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    // UHFPS will call this from On Take Event
    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);

        // Play sound
        if (audioSource != null)
            audioSource.Play();

        // Send signal to puzzle manager
        if (DinerPuzzleManager.Instance != null)
            DinerPuzzleManager.Instance.RegisterInput(objectID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}