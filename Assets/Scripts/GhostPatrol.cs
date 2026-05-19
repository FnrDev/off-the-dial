using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GhostPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float waypointReachDistance = 1f;

    private CharacterController _controller;
    private int _currentIndex = 0;
    private float _verticalVelocity = 0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();

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
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[_currentIndex];
        if (target == null) return;

        Vector3 destination = target.position;
        Vector3 toTarget = destination - transform.position;
        float distanceXZ = new Vector2(toTarget.x, toTarget.z).magnitude;

        // Advance to next waypoint when within reach distance
        if (distanceXZ < waypointReachDistance)
        {
            _currentIndex = (_currentIndex + 1) % waypoints.Length;
            return;
        }

        // Move toward current waypoint (horizontal only)
        Vector3 moveDir = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

        // Apply gravity so the ghost stays grounded on terrain
        if (_controller.isGrounded)
        {
            _verticalVelocity = -0.5f; // small downward force to keep grounded
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 motion = moveDir * speed + Vector3.up * _verticalVelocity;
        _controller.Move(motion * Time.deltaTime);

        // Smoothly rotate to face direction of travel
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
