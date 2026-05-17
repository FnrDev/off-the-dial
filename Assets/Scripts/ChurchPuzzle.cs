using UnityEngine;

public class ChurchPuzzle : MonoBehaviour
{
    [Header("Candles (assign in order: 0,1,2,3,4)")]
    public GameObject[] candleFlames;  // drag the Flame child of each puzzle candle
    public Light[] candleLights;

    [Header("Puzzle Settings")]
    // Player must extinguish candles in this order: 5,2,1,3,4
    public int[] correctOrder = { 4, 1, 0, 2, 3 };

    [Header("References")]
    public GameObject altarReward;

    private int[] playerOrder = new int[5];
    private int currentStep = 0;
    private bool puzzleSolved = false;

    void Start()
    {
        // All puzzle candles start lit
        for (int i = 0; i < candleFlames.Length; i++)
        {
            candleFlames[i].SetActive(true);
            candleLights[i].enabled = true;
        }
    }

    public void ExtinguishCandle(int candleIndex)
    {
        if (puzzleSolved) return;

        playerOrder[currentStep] = candleIndex;
        candleFlames[candleIndex].SetActive(false);
        candleLights[candleIndex].enabled = false;
        currentStep++;

        if (currentStep == 5)
            CheckSolution();
    }

    void CheckSolution()
    {
        for (int i = 0; i < 5; i++)
        {
            if (playerOrder[i] != correctOrder[i])
            {
                ResetPuzzle();
                return;
            }
        }
        PuzzleSolved();
    }

    void ResetPuzzle()
    {
        currentStep = 0;
        // Turn all candles back on
        for (int i = 0; i < candleFlames.Length; i++)
        {
            candleFlames[i].SetActive(true);
            candleLights[i].enabled = true;
        }
        Debug.Log("Wrong order — candles reset!");
    }

    void PuzzleSolved()
    {
        puzzleSolved = true;
        if (altarReward != null)
            altarReward.SetActive(true);
        Debug.Log("Church puzzle solved! Altar unlocked.");
    }
}