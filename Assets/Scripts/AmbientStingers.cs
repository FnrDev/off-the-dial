using UnityEngine;

public class AmbientStingers : MonoBehaviour
{
    public AudioClip[] clips;
    public float minInterval = 8f;
    public float maxInterval = 20f;
    public float volume = 0.5f;
    public float pitchJitter = 0.1f;

    AudioSource src;
    float nextTime;

    void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        src.priority = 80;
        ScheduleNext();
    }

    void Update()
    {
        if (clips == null || clips.Length == 0) return;
        if (Time.time < nextTime) return;
        var clip = clips[Random.Range(0, clips.Length)];
        if (clip != null)
        {
            src.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
            src.PlayOneShot(clip, volume);
        }
        ScheduleNext();
    }

    void ScheduleNext()
    {
        nextTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
