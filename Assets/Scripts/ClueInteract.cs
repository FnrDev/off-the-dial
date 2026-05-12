using UnityEngine;
using TMPro;

public class ClueInteract : MonoBehaviour
{
    public string clueText;
    public string clueDigit;
    public TextMeshProUGUI clueUI;
    public TextMeshProUGUI promptText;

    public Transform player;
    public float interactDistance = 3f;

    private bool playerInRange = false;

    void Update()
    {
        if (player == null) return;

        playerInRange = Vector3.Distance(transform.position, player.position) <= interactDistance;

        if (playerInRange)
            promptText.text = "Press E to Inspect";
        else
            promptText.text = "";

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowClue();
        }
    }

    void ShowClue()
    {
        HeartMonitorBeep beep = GetComponent<HeartMonitorBeep>();
if (beep != null)
{
    beep.PlayPattern();
}
        promptText.text = "";
        clueUI.text = clueText;

        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.AddDigit(clueDigit);
        }

        CancelInvoke(nameof(ClearClueText));
        Invoke(nameof(ClearClueText), 2f);
    }

    void ClearClueText()
    {
        clueUI.text = "";

        if (playerInRange)
            promptText.text = "Press E to Inspect";
    }
}