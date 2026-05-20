using System.Collections;
using UnityEngine;

/// Correct extinguish order: Candle 5 → 2 → 1 → 8 → 10

public class ChurchPuzzle : MonoBehaviour
{
    public static ChurchPuzzle Instance { get; private set; }

    [Header("All Candles (drag Candle 1 to Candle 10 in order)")]
    public CandleInteract[] allCandles;

    [Header("Puzzle Sequence (1-based candle indices)")]
    [Tooltip("Correct blow-out order. Default: 5, 2, 1, 8, 10")]
    public int[] correctSequence = { 5, 2, 1, 8, 10 };

    [Header("Reward")]
    [Tooltip("Any GameObject to activate on solve: door, chest, key item, etc.")]
    public GameObject rewardObject;

    private int[] _playerSequence;
    private int _currentStep = 0;
    private bool _puzzleSolved = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[ChurchPuzzle] Instance set in Awake.");
    }

    void Start()
    {
        _playerSequence = new int[correctSequence.Length];
        ResetAllCandles();
    }

    public void OnCandleBlown(int candleIndex)
    {
        if (_puzzleSolved) return;

        _playerSequence[_currentStep] = candleIndex;
        SetCandleLit(candleIndex, false);
        _currentStep++;

        Debug.Log($"[ChurchPuzzle] Step {_currentStep}/{correctSequence.Length} — Candle {candleIndex} blown out.");

        if (_currentStep >= correctSequence.Length)
        {
            StartCoroutine(CheckSolutionDelayed());
        }
    }

    // Small delay so the last candle visually blows out before reset
    IEnumerator CheckSolutionDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        CheckSolution();
    }

    void CheckSolution()
    {
        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (_playerSequence[i] != correctSequence[i])
            {
                Debug.Log($"[ChurchPuzzle] Wrong sequence! Resetting.");
                ResetAllCandles();
                return;
            }
        }

        StartCoroutine(SolvePuzzle());
    }

    void ResetAllCandles()
    {
        _currentStep = 0;
        _playerSequence = new int[correctSequence.Length];

        foreach (var candle in allCandles)
        {
            if (candle != null)
                candle.SetLit(true);
        }

        Debug.Log("[ChurchPuzzle] All candles re-lit. Puzzle reset.");
    }

    IEnumerator SolvePuzzle()
    {
        _puzzleSolved = true;
        Debug.Log("[ChurchPuzzle] Puzzle SOLVED!");

        yield return new WaitForSeconds(1.5f);

        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ChurchPuzzle] Reward activated: {rewardObject.name}");
        }
        else
        {
            Debug.Log("[ChurchPuzzle] No reward object assigned.");
        }
    }

    void SetCandleLit(int candleIndex, bool lit)
    {
        int arrayIndex = candleIndex - 1;
        if (arrayIndex >= 0 && arrayIndex < allCandles.Length && allCandles[arrayIndex] != null)
            allCandles[arrayIndex].SetLit(lit);
        else
            Debug.LogWarning($"[ChurchPuzzle] Candle index {candleIndex} out of range or not assigned.");
    }

    public void ForceReset()
    {
        _puzzleSolved = false;
        ResetAllCandles();
    }
}