using UnityEngine;
using UHFPS.Runtime;

public class TowerEndingManager : MonoBehaviour
{
    [Header("Rain")]
    public GameObject rainSplash;
    public float rainFadeTime = 2f;

    [Header("Ending")]
    public bool broadcastComplete = false;

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

        Debug.Log("Tower reached - Rain started");
    }

    public void TriggerGoodEnding()
    {
        Debug.Log("GOOD ENDING triggered");
        StopRain();
        // TODO: fog lift, dawn light, ghost dissolve
    }

    public void TriggerBadEnding()
    {
        Debug.Log("BAD ENDING triggered");
        // TODO: fog thickens, ghost climbs
    }

    private void StopRain()
    {
        var rainModule = GameManager.Module<RainingModule>();
        if (rainModule != null)
            rainModule.FadeRaindrop(false, rainFadeTime);

        if (rainSplash != null)
            rainSplash.SetActive(false);
    }
}
