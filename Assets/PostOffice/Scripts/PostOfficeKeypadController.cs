using UnityEngine;
using UHFPS.Runtime;

public class PostOfficeKeypadController : MonoBehaviour
{
    [SerializeField] private KeypadPuzzle keypadPuzzle;
    [SerializeField] private Light keypadLight;

    private void Start()
    {
        keypadPuzzle.enabled = false;
        var col = keypadPuzzle.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public void RestorePower()
{
    keypadPuzzle.enabled = true;
    var col = keypadPuzzle.GetComponent<Collider>();
    if (col != null) col.enabled = true;
}
}