using UnityEngine;
using UHFPS.Runtime;

public class PostOfficeKeypadController : MonoBehaviour
{
    [SerializeField] private KeypadPuzzle keypadPuzzle;
    [SerializeField] private Light keypadLight;

    public void RestorePower()
    {
        keypadPuzzle.enabled = true;

        if (keypadLight != null)
        {
            keypadLight.enabled = true;
        }
    }
}