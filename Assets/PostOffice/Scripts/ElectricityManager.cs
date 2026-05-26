using UnityEngine;

public class ElectricityManager : MonoBehaviour
{
    [SerializeField] private Light[] lights;

    private void Start()
    {
        foreach (var l in lights)
        {
            if (l == null) continue;
            l.enabled = false;
        }
    }

    public void RestorePower()
    {
        foreach (var l in lights)
        {
            if (l == null) continue;
            l.enabled = true;
        }
    }
}