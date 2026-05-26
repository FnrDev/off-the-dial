using UnityEngine;

public class DinerEntranceTrigger : MonoBehaviour
{
    public GameObject sequenceUI;

    private void Start()
    {
        if (sequenceUI != null)
            sequenceUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sequenceUI != null)
                sequenceUI.SetActive(true);
        }
    }
}