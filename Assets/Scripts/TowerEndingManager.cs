using System.Collections;
using UnityEngine;
using UHFPS.Runtime;

public class TowerEndingManager : MonoBehaviour
{
    [Header("Rain")]
    public GameObject rainSplash;
    public float rainFadeTime = 2f;

    [Header("Ending")]
    public GhostPatrol ghost;
    public Light directionalLight;
    public AudioSource ambientHorror;

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

        // Show ending UI - load main menu or
        // show credits screen
        Debug.Log("ENDING COMPLETE");
    }

    private void StopRain()
    {
        var rainModule = GameManager.Module<RainingModule>();
        if (rainModule != null)
            rainModule.FadeRaindrop(false, rainFadeTime);

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
