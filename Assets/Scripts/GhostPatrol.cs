using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float waypointReachDistance = 1f;

    private NavMeshAgent _agent;
    private int _currentIndex = 0;
    private bool _destinationSet = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        _agent.angularSpeed = 0f; // disable agent auto-rotation; we handle it manually
        _agent.autoBraking = false; // keep moving without slowing near waypoints

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("GhostPatrol: No waypoints assigned on " + gameObject.name);
            enabled = false;
            return;
        }

        // Start at the nearest waypoint so patrol begins cleanly
        // regardless of where the Ghost is placed in the scene
        float closestDist = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Vector3 diff = waypoints[i].position - transform.position;
            float dist = new Vector2(diff.x, diff.z).magnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                _currentIndex = i;
            }
        }

        SetDestination(_currentIndex);
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (!_destinationSet) return;

        // Wait until the agent path is ready before checking distance
        if (_agent.pathPending) return;

        if (_agent.remainingDistance <= waypointReachDistance)
        {
            _currentIndex = (_currentIndex + 1) % waypoints.Length;
            SetDestination(_currentIndex);
        }

        // Smoothly rotate to face direction of travel
        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void SetDestination(int index)
    {
        if (waypoints[index] == null) return;
        _agent.SetDestination(waypoints[index].position);
        _destinationSet = true;
    }
}
