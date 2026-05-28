using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UHFPS.Input;
using UHFPS.Runtime;

public class TowerEndingManager : MonoBehaviour
{
    private const string ContinueHint = "\n\n<size=70%><i>Press [E] to return to the main menu.</i></size>";

    [Header("Rain")]
    public GameObject rainSplash;
    public float rainFadeTime = 2f;
    public AudioSource rainAmbience;

    [Header("Ending")]
    public GhostPatrol ghost;
    public Light directionalLight;
    public AudioSource ambientHorror;

    [Header("Ending Screen")]
    [TextArea(6, 20)]
    public string endingText =
        "The dial finally tunes.\n\nFive voices, five fragments, one broadcast going out from the top of the tower. Whatever was holding Oxley in the static loosens its grip — the radio finds a clear channel, and the island starts to breathe again.\n\nYou did what no one else could: you listened.";

    private bool _rainStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_rainStarted)
        {
            StartRain();
        }
    }

    public void StartRain()
    {
        _rainStarted = true;

        if (rainSplash != null)
            rainSplash.SetActive(true);

        if (rainAmbience != null)
            rainAmbience.Play();

        var rainModule = GameManager.Module<RainingModule>();
        if (rainModule != null)
            rainModule.FadeRaindrop(true, rainFadeTime);
    }

    public void TriggerGoodEnding()
    {
        StartCoroutine(GoodEndingSequence());
    }

    private IEnumerator GoodEndingSequence()
    {
        // Stop rain
        StopRain();
        if (ambientHorror != null)
            ambientHorror.Stop();
        AudioListener.volume = 0f;
        yield return new WaitForSeconds(3f);

        // Warm dawn light
        if (directionalLight != null)
        {
            directionalLight.gameObject.SetActive(true);
            directionalLight.color =
                new Color(1f, 0.85f, 0.5f);
            directionalLight.intensity = 1.5f;
        }
        yield return new WaitForSeconds(2f);

        // Dissolve ghost
        if (ghost != null)
            ghost.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        yield return ShowEndingScreen();
    }

    private IEnumerator ShowEndingScreen()
    {
        var gameManager = GameManager.Instance;
        var presence = PlayerPresenceManager.Instance;
        if (gameManager == null) yield break;

        if (presence != null) presence.FreezePlayer(true);

        gameManager.ShowPaperInfo(true, false, endingText + ContinueHint);

        yield return null;
        while (!InputManager.ReadButtonOnce(this, Controls.USE))
            yield return null;

        gameManager.ShowPaperInfo(false, false);
        AudioListener.volume = 1f;

        SceneManager.LoadScene(SaveGameManager.MM);
    }

    private void StopRain()
    {
        var rainModule = GameManager.Module<RainingModule>();
        if (rainModule != null)
            rainModule.FadeRaindrop(false, rainFadeTime);

        if (rainAmbience != null)
            rainAmbience.Stop();

        if (rainSplash != null)
            rainSplash.SetActive(false);
    }

    public void TriggerBadEnding()
    {
        StartCoroutine(BadEndingSequence());
    }

    private IEnumerator BadEndingSequence()
    {
        // Make rain heavier
        var rainModule = GameManager.Module<RainingModule>();
        if (rainModule != null)
            rainModule.FadeRaindrop(true, 0.5f);

        yield return new WaitForSeconds(3f);

        // Ghost climbs - move ghost to tower base
        if (ghost != null)
        {
            ghost.transform.position =
                new Vector3(434.6715f, -53.8532f,
                -455.6354f);
        }

        yield return new WaitForSeconds(3f);

        // Game over
        Debug.Log("ENDING COMPLETE");
    }
}
