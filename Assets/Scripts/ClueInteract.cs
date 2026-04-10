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
        if (promptText != null)
        {
            promptText.text = "";
        }

        if (clueUI != null)
        {
            clueUI.text = clueText;

            CancelInvoke(nameof(ClearClueText));
            Invoke(nameof(ClearClueText), 2f);
        }

        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.AddDigit(clueDigit);
        }
    }

    void ClearClueText()
    {
        if (clueUI != null)
        {
            clueUI.text = "";
        }

        if (playerInRange && promptText != null)
        {
            promptText.text = "Press E to Inspect";
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

            if (clueUI != null)
            {
                clueUI.text = "";
            }
        }
    }
}