using UnityEngine;
using UnityEngine.SceneManagement;

public class PostOfficeEntranceTrigger : MonoBehaviour
{
    [SerializeField] private string sceneName = "PostOffice";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}