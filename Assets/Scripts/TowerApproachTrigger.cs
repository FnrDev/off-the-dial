using UnityEngine;

public class TowerApproachTrigger : MonoBehaviour
{
    public TowerEndingManager endingManager;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            endingManager.StartRain();
        }
    }
}
