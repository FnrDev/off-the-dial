using UnityEngine;
using UHFPS.Runtime;

public class CandleInteract : MonoBehaviour, IInteractStart, IInteractTitle
{
    [Header("Settings")]
    [Tooltip("1-based index: Candle_1 = 1, Candle_10 = 10")]
    public int candleIndex = 1;

    [Header("Visual References")]
    public GameObject flameObject;
    public Light candleLight;

    [Header("Interact Prompt")]
    public string title = "Candle";
    public string useTitle = "Blow";

    [Header("Audio")]
    public AudioClip blowSound;
    public AudioClip wrongSound;

    private bool _isLit = true;
    private Collider _interactCollider;

    void Start()
    {
        _interactCollider = GetComponentInChildren<Collider>();
        SetLit(true);
    }

    public void InteractStart()
    {
        if (!_isLit) return;
        TryBlow();
    }

    public TitleParams InteractTitle()
    {
        return new TitleParams
        {
            title = title,
            button1 = useTitle,
            button2 = null
        };
    }

    void TryBlow()
    {
        if (ChurchPuzzle.Instance != null)
            ChurchPuzzle.Instance.OnCandleBlown(candleIndex);
        else
            Debug.LogError("[CandleInteract] ChurchPuzzle.Instance is NULL!");
    }

    public void PlayBlowSound()
    {
        if (blowSound != null)
            AudioSource.PlayClipAtPoint(blowSound, transform.position);
    }

    public void SetLit(bool lit)
    {
        _isLit = lit;

        if (flameObject != null)
            flameObject.SetActive(lit);

        if (candleLight != null)
            candleLight.enabled = lit;

        if (_interactCollider != null)
            _interactCollider.enabled = lit;
    }

    public void PlayWrongSound()
    {
        if (wrongSound != null)
            AudioSource.PlayClipAtPoint(wrongSound, transform.position);
    }

    public bool IsLit => _isLit;
}
