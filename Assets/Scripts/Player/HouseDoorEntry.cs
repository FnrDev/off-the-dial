using UnityEngine;

public class HouseDoorEntry : MonoBehaviour
{
    MeshCollider[] _colliders;

    void Awake()
    {
        var house = transform.parent;
        if (house != null) _colliders = house.GetComponentsInChildren<MeshCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) SetEnabled(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) SetEnabled(true);
    }

    void SetEnabled(bool enabled)
    {
        if (_colliders == null) return;
        foreach (var c in _colliders)
        {
            if (c == null) continue;
            c.enabled = enabled;
        }
    }
}
