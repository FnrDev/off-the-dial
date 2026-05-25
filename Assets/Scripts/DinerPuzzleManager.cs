using System.Collections.Generic;
using UnityEngine;

public class DinerPuzzleManager : MonoBehaviour
{
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
        if (freezerReward != null)
            freezerReward.SetActive(false);
    }

    public void RegisterInput(int input)
    {
        if (puzzleSolved)
            return;

        playerSequence.Add(input);

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

            playerSequence.Clear();
            return;
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            Debug.Log("Puzzle Solved!");
            PuzzleSolved();
        }
    }

    private void PuzzleSolved()
    {
        puzzleSolved = true;

        if (freezerReward != null)
            freezerReward.SetActive(true);

        if (freezerDoor != null)
            freezerDoor.UnlockDoor();

        if (successAudio != null)
            successAudio.Play();
    }
}