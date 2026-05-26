using UnityEngine;

public class PostOfficeGhostZone : MonoBehaviour
{
    [SerializeField] private GhostPatrol ghost;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ghost.gameObject.SetActive(true);
        ghost.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ghost.enabled = false;
    }
}