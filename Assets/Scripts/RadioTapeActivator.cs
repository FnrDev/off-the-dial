using UnityEngine;
using UHFPS.Runtime;

public class RadioTapeActivator : MonoBehaviour
{
    public Radio radio;
    public GameObject vhsTape;

    private bool _activated = false;
    private float _timer = 0f;
    private float _activateAfter = 3f;

    private void Update()
    {
        if (_activated) return;
        if (radio == null || vhsTape == null) return;

        if (radio.AudioSource != null && radio.AudioSource.isPlaying)
        {
            _timer += Time.deltaTime;
            if (_timer >= _activateAfter)
            {
                vhsTape.SetActive(true);
                _activated = true;
                Debug.Log("TAPE ACTIVATED");
            }
        }
        else
        {
            _timer = 0f;
        }
    }
}
