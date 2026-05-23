using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class CandleInteract : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("1-based index: Candle_1 = 1, Candle_10 = 10")]
    public int candleIndex = 1;

    [Header("Visual References")]
    public GameObject flameObject;
    public Light candleLight;

    [Header("Interaction")]
    public float interactDistance = 1.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public TextMeshProUGUI promptText;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip blowOutSound;
    
    private Transform _player;
    private bool _isLit = true;
    private bool _playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            Debug.Log($"[CandleInteract] Candle_{candleIndex}: Player found successfully.");
        }
        else
        {
            Debug.LogError($"[CandleInteract] Candle_{candleIndex}: No GameObject tagged 'Player' found!");
        }

        SetLit(true);
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        _playerInRange = dist <= interactDistance;

        if (promptText != null)
        {
            if (_playerInRange)
            {
                if (_isLit)
                {
                    promptText.text = "[E] to Blow Out";
                }
                else
                {
                    promptText.text = ""; // IMPORTANT: no prompt if already off
                }
            }
            else
            {
                promptText.text = "";
            }
        }

        if (_playerInRange && _isLit && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log($"[CandleInteract] Candle_{candleIndex}: E pressed, calling OnCandleBlown.");
            TryBlow();
        }
    }

    void TryBlow()
    {
        if (ChurchPuzzle.Instance != null)
            ChurchPuzzle.Instance.OnCandleBlown(candleIndex);
        else
            Debug.LogError("[CandleInteract] ChurchPuzzle.Instance is NULL!");
    }

    public void SetLit(bool lit)
    {
        // Always force the state regardless of current _isLit value
        _isLit = lit;

        if (flameObject != null)
            flameObject.SetActive(lit);

        if (candleLight != null)
            candleLight.enabled = lit;

        // Clear prompt when extinguished
        if (!lit && promptText != null)
            promptText.text = "";

            if (!lit && audioSource != null && blowOutSound != null)
        audioSource.PlayOneShot(blowOutSound);
    }

    public bool IsLit => _isLit;
}