using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 0f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.05f;

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = flickerSpeed;
        }
    }
}
