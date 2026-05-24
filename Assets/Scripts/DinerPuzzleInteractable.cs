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

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);

        // Play sound
        if (audioSource != null)
            audioSource.Play();

        // Send signal to puzzle manager
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