using UnityEngine;
using System.Collections;

public class HeartMonitorBeep : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip beepClip;

    void Start()
    {
        // StartCoroutine(BeepPattern());
    }

    IEnumerator BeepPattern()
    {
        while (true)
        {
            // 1 beep
            PlayBeep();
            yield return new WaitForSeconds(1f);

            // 2 beeps
            PlayBeep();
            yield return new WaitForSeconds(0.25f);
            PlayBeep();
            yield return new WaitForSeconds(1f);

            // 1 beep
            PlayBeep();
            yield return new WaitForSeconds(1.5f);
        }
    }

    void PlayBeep()
    {
        if (audioSource != null && beepClip != null)
            audioSource.PlayOneShot(beepClip);
    }
    public void PlayPattern()
{
    StartCoroutine(BeepPattern());
}
}