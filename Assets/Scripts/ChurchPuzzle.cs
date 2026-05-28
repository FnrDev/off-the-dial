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

    [Header("Reward Sound")]
    public AudioClip rewardSound;
    public float rewardVolume = 1.8f;

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

        SetCandleLit(candleIndex, false);

        if (candleIndex != correctSequence[_currentStep])
        {
            Debug.Log($"[ChurchPuzzle] Wrong candle! Expected {correctSequence[_currentStep]}, got {candleIndex}. Resetting.");
            PlayWrongSoundOn(candleIndex);
            StartCoroutine(ResetAfterDelay());
            return;
        }

        _currentStep++;
        Debug.Log($"[ChurchPuzzle] Step {_currentStep}/{correctSequence.Length} — Candle {candleIndex} correct.");

        if (_currentStep >= correctSequence.Length)
            StartCoroutine(SolvePuzzle());
    }

    void PlayWrongSoundOn(int candleIndex)
    {
        int i = candleIndex - 1;
        if (i >= 0 && i < allCandles.Length && allCandles[i] != null)
            allCandles[i].PlayWrongSound();
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ResetAllCandles();
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

            if (rewardSound != null)
                AudioSource.PlayClipAtPoint(rewardSound, rewardObject.transform.position, rewardVolume);

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
