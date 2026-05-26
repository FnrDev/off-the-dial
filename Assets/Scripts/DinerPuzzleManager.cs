using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        sequenceUI.text = "OPEN";
        puzzleSolved = true;

        if (freezerReward != null)
            freezerReward.SetActive(true);

        if (freezerDoor != null)
            freezerDoor.UnlockDoor();

        if (successAudio != null)
            successAudio.Play();
    }
}