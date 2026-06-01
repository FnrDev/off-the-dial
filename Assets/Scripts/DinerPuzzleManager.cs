using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class DinerPuzzleManager : MonoBehaviour
{
[Header("UI")]
public TMP_Text sequenceUI;
    public static DinerPuzzleManager Instance;

    [Header("Puzzle Sequence")]
    public List<int> correctSequence = new List<int>() { 1, 2, 3, 4 };

    private List<int> playerSequence = new List<int>();
    private bool puzzleSolved = false;

    [Header("Reward")]
    public GameObject freezerReward;

    [Header("Freezer Door")]
    public FreezerDoor freezerDoor;

    [Header("Audio")]
    public AudioSource successAudio;
    public AudioSource wrongAudio;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
UpdateSequenceUI();
        if (freezerReward != null)
            freezerReward.SetActive(false);
    }

    public void RegisterInput(int input)
    {
        if (puzzleSolved)
            return;
        playerSequence.Add(input);
UpdateSequenceUI();
        int currentIndex = playerSequence.Count - 1;

        if (currentIndex >= correctSequence.Count)
        {
            playerSequence.Clear();
            return;
        }

        if (playerSequence[currentIndex] != correctSequence[currentIndex])
        {
            Debug.Log("Wrong sequence!");

            if (wrongAudio != null)
                wrongAudio.Play();
            sequenceUI.text = "WRONG";
            UpdateSequenceUI();

            playerSequence.Clear();
            UpdateSequenceUI();
            return;
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            Debug.Log("Puzzle Solved!");
            PuzzleSolved();
        }
    }
   private void UpdateSequenceUI()
{
    if (sequenceUI == null) return;

    string display = "";

    for (int i = 0; i < correctSequence.Count; i++)
    {
        if (i < playerSequence.Count)
            display += GetObjectName(playerSequence[i]) + " ";
        else
            display += "_ ";
    }

    sequenceUI.text = display;
}
private string GetObjectName(int id)
{
    switch (id)
    {
        case 1: return "Music";
        case 2: return "Coffee";
        case 3: return "Cold Storage";
        case 4: return "Register";
        default: return "?";
    }
}

    private void PuzzleSolved()
    {
StartCoroutine(HideSequenceUIAfterDelay());
        puzzleSolved = true;

        if (freezerReward != null)
            freezerReward.SetActive(true);

        if (freezerDoor != null)
            freezerDoor.UnlockDoor();

        // Once solved, stop the freezer itself from being interactable so the
        // player picks up the reward (VHS tape) instead of re-triggering the
        // "objects.freezer" prompt that sits in front of it. Moving it to the
        // Ignore Raycast layer takes it off the interaction raycast (cull mask
        // is Default + Interact), letting the ray reach the tape, while the
        // freezer still physically blocks the player.
        if (freezerDoor != null && freezerDoor.transform.parent != null)
        {
            GameObject freezerObj = freezerDoor.transform.parent.gameObject;
            freezerObj.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (freezerObj.TryGetComponent(out DinerPuzzleInteractable freezerInteract))
                freezerInteract.enabled = false;
        }

        if (successAudio != null)
            successAudio.Play();
    }

    private IEnumerator HideSequenceUIAfterDelay()
{
    if (sequenceUI == null) yield break;

    sequenceUI.text = "OPEN";

    yield return new WaitForSeconds(2f);

    sequenceUI.gameObject.SetActive(false);
}
}