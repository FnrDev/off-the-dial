using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostPatrol : MonoBehaviour
{
    // ─── Patrol ───────────────────────────────────────────────────────────────
    public Transform[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float waypointReachDistance = 1f;

    // ─── Detection & chase ────────────────────────────────────────────────────
    [Tooltip("Maximum distance at which the ghost can spot the player (requires line of sight).")]
    public float detectionRange = 8f;
    public float chaseSpeed = 4f;
    [Tooltip("Distance the player must reach before the ghost gives up the chase.")]
    public float escapeRange = 20f;
    public float catchDistance = 1.5f;
    public float alertDuration = 2f;

    // ─── Events ───────────────────────────────────────────────────────────────
    public UnityEvent OnPlayerCaught;

    // ─── Private state ────────────────────────────────────────────────────────
    public enum GhostState { Patrol, Alert, Chase, Return }

    private NavMeshAgent _agent;
    private Transform _player;
    private GhostState _state = GhostState.Patrol;
    private int _currentIndex = 0;
    private bool _destinationSet = false;
    private float _alertTimer = 0f;
    private bool _playerCaughtFired = false;

    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        _agent.angularSpeed = 0f;   // manual rotation handled below
        _agent.autoBraking = false;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning("GhostPatrol: No GameObject tagged 'Player' found. " +
                             "Detection disabled — ghost will patrol only.");

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("GhostPatrol: No waypoints assigned on " + gameObject.name);
            enabled = false;
            return;
        }

        _currentIndex = NearestWaypointIndex();
        SetPatrolDestination(_currentIndex);
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        float distToPlayer = _player != null
            ? Vector3.Distance(transform.position, _player.position)
            : float.MaxValue;

        switch (_state)
        {
            case GhostState.Patrol: UpdatePatrol(distToPlayer); break;
            case GhostState.Alert:  UpdateAlert(distToPlayer);  break;
            case GhostState.Chase:  UpdateChase(distToPlayer);  break;
            case GhostState.Return: UpdateReturn(distToPlayer); break;
        }

        SmoothRotate();
    }

    // ─── State: Patrol ────────────────────────────────────────────────────────

    void UpdatePatrol(float distToPlayer)
    {
        // Only enter Alert if the ghost can actually see the player.
        // A wall between them means CanSeePlayer returns false even if
        // the player is within detectionRange.
        if (CanSeePlayer(distToPlayer))
        {
            EnterAlert();
            return;
        }

        if (!_destinationSet) return;
        if (_agent.pathPending) return;

        if (_agent.remainingDistance <= waypointReachDistance)
        {
            _currentIndex = (_currentIndex + 1) % waypoints.Length;
            SetPatrolDestination(_currentIndex);
        }
    }

    // ─── State: Alert ─────────────────────────────────────────────────────────

    void UpdateAlert(float distToPlayer)
    {
        // Face the player while alert
        if (_player != null)
        {
            Vector3 dir = (_player.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, look, rotationSpeed * Time.deltaTime);
            }
        }

        _alertTimer += Time.deltaTime;

        // After alert duration → start chase regardless of LOS
        // (ghost committed once it spotted the player)
        if (_alertTimer >= alertDuration)
        {
            EnterChase();
            return;
        }

        // Player ran far away before chase started → give up
        if (_player == null || distToPlayer > escapeRange)
        {
            EnterReturn();
        }
    }

    // ─── State: Chase ─────────────────────────────────────────────────────────

    void UpdateChase(float distToPlayer)
    {
        // Catch player
        if (distToPlayer <= catchDistance && !_playerCaughtFired)
        {
            _playerCaughtFired = true;
            OnPlayerCaught?.Invoke();
        }

        // Player escaped → return to patrol
        if (distToPlayer > escapeRange)
        {
            EnterReturn();
            return;
        }

        // Keep chasing
        if (_player != null)
            _agent.SetDestination(_player.position);
    }

    // ─── State: Return ────────────────────────────────────────────────────────

    void UpdateReturn(float distToPlayer)
    {
        // Only re-engage if the ghost can SEE the player again.
        // Without this LOS check the ghost would oscillate between
        // Return and Alert whenever the player hides within detectionRange,
        // never reaching a waypoint and never resuming patrol.
        if (CanSeePlayer(distToPlayer))
        {
            EnterAlert();
            return;
        }

        if (_agent.pathPending) return;

        // Reached the waypoint → resume Patrol
        if (_agent.remainingDistance <= waypointReachDistance)
        {
            EnterPatrol();
        }
    }

    // ─── State transitions ────────────────────────────────────────────────────

    void EnterAlert()
    {
        _state = GhostState.Alert;
        _alertTimer = 0f;
        _agent.isStopped = true;
        _agent.speed = speed;
    }

    void EnterChase()
    {
        _state = GhostState.Chase;
        _playerCaughtFired = false;
        _agent.isStopped = false;
        _agent.speed = chaseSpeed;

        if (_player != null)
            _agent.SetDestination(_player.position);
    }

    void EnterReturn()
    {
        _state = GhostState.Return;
        _agent.isStopped = false;
        _agent.speed = speed;

        _currentIndex = NearestWaypointIndex();
        SetPatrolDestination(_currentIndex);
    }

    void EnterPatrol()
    {
        _state = GhostState.Patrol;
        _agent.isStopped = false;
        _agent.speed = speed;

        _currentIndex = (_currentIndex + 1) % waypoints.Length;
        SetPatrolDestination(_currentIndex);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns true only if the player is within detectionRange AND
    /// a linecast on the Default layer hits no obstacle between the ghost
    /// and the player. Trigger colliders and non-Default layers are ignored.
    /// Falls back to false (not visible) when nothing is hit.
    /// </summary>
    bool CanSeePlayer(float distToPlayer)
    {
        // Fast distance reject before doing any physics work
        if (_player == null || distToPlayer > detectionRange) return false;

        Vector3 eyePos    = transform.position + Vector3.up * 1.5f;
        Vector3 playerPos = _player.position   + Vector3.up * 1.0f;

        // Only test against the Default layer (layer 0).
        // This ignores triggers, NavMesh, UI, and any custom layers so
        // only solid world geometry can block line of sight.
        int defaultLayer = 1 << LayerMask.NameToLayer("Default");

        // If nothing on the Default layer sits between ghost and player,
        // the path is clear and the ghost can see the player.
        // If something IS hit, a wall or prop is blocking sight.
        bool blocked = Physics.Linecast(eyePos, playerPos, defaultLayer);

        return !blocked;
    }

    void SetPatrolDestination(int index)
    {
        if (waypoints[index] == null) return;
        _agent.isStopped = false;
        _agent.SetDestination(waypoints[index].position);
        _destinationSet = true;
    }

    int NearestWaypointIndex()
    {
        int nearest = 0;
        float closest = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Vector3 diff = waypoints[i].position - transform.position;
            float dist = new Vector2(diff.x, diff.z).magnitude;
            if (dist < closest) { closest = dist; nearest = i; }
        }
        return nearest;
    }

    void SmoothRotate()
    {
        if (_state == GhostState.Alert) return;

        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }

    // ─── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Teleport the ghost to WP1 and restart patrol.
    /// Called by ZoneRespawn after the player is caught.
    /// </summary>
    public void ResetToFirstWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (waypoints[0] == null) return;
        StartCoroutine(ResetCoroutine());
    }

    private System.Collections.IEnumerator ResetCoroutine()
    {
        // Halt agent and clear path before warping
        _agent.isStopped = true;
        _agent.ResetPath();

        // Warp to WP1. NavMeshAgent requires one frame after Warp before
        // SetDestination is reliable — calling both same frame can silently fail.
        _agent.Warp(waypoints[0].position);

        // Set state to Patrol with no destination so Update() stays idle
        // during the yield (the !_destinationSet guard in UpdatePatrol handles this)
        _state = GhostState.Patrol;
        _destinationSet = false;
        _playerCaughtFired = false;
        _alertTimer = 0f;
        _agent.speed = speed;

        yield return null; // one frame for Warp to settle

        // Safe to set destination now
        _currentIndex = 1 % waypoints.Length;
        SetPatrolDestination(_currentIndex);
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        // Detection range — yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Escape range — red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, escapeRange);

        // Catch distance — magenta
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}
