/*A patient was left isolated during an outbreak.
Their voice is trapped in the radio.
Fragments of their final moments are scattered in the room.*/


using UnityEngine;
using TMPro;

public class ClueInteract : MonoBehaviour
{
    public string clueText;
    public string clueDigit;
    public TextMeshProUGUI clueUI;
    public TextMeshProUGUI promptText;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowClue();
        }
    }

    void ShowClue()
    {
        if (clueUI != null)
        {
            clueUI.text = clueText;
        }

        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.AddDigit(clueDigit);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
    {
        playerInRange = true;

        if (promptText != null)
        {
            promptText.text = "Press E to Inspect";
        }
    }
    }

    void OnTriggerExit(Collider other)
    {
           if (other.CompareTag("Player"))
    {
        playerInRange = false;

        if (promptText != null)
        {
            promptText.text = "";
        }
    }
    }
}