using UnityEngine;
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

    [Header("Audio")]
    public AudioClip blowSound;
    public AudioClip wrongSound;

    private Transform _player;
    private GameObject _candlePrompt;
    private bool _isLit = true;
    private bool _wasInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        // GameObject.Find skips inactive objects, so find via active parent
        var hud = GameObject.Find("HUDPanel");
        if (hud != null)
        {
            var t = hud.transform.Find("CandlePrompt");
            if (t != null) _candlePrompt = t.gameObject;
        }

        SetLit(true);
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= interactDistance && _isLit;

        if (inRange && !_wasInRange)
            ShowPrompt(true);
        else if (!inRange && _wasInRange)
            ShowPrompt(false);

        _wasInRange = inRange;

        if (inRange && Keyboard.current.eKey.wasPressedThisFrame)
            TryBlow();
    }

    void OnDisable()
    {
        if (_wasInRange)
            ShowPrompt(false);
    }

    void ShowPrompt(bool show)
    {
        if (_candlePrompt != null)
            _candlePrompt.SetActive(show);
    }

    void TryBlow()
    {
        if (blowSound != null)
            AudioSource.PlayClipAtPoint(blowSound, transform.position);

        if (ChurchPuzzle.Instance != null)
            ChurchPuzzle.Instance.OnCandleBlown(candleIndex);
        else
            Debug.LogError("[CandleInteract] ChurchPuzzle.Instance is NULL!");
    }

    public void SetLit(bool lit)
    {
        _isLit = lit;

        if (!lit && _wasInRange)
            ShowPrompt(false);

        if (flameObject != null)
            flameObject.SetActive(lit);

        if (candleLight != null)
            candleLight.enabled = lit;
    }

    public void PlayWrongSound()
    {
        if (wrongSound != null)
            AudioSource.PlayClipAtPoint(wrongSound, transform.position);
    }

    public bool IsLit => _isLit;
}
