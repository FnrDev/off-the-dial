using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UHFPS.Runtime;

/// <summary>
/// Spawns UHFPS patrol zombies in several map regions (East / West / North / South)
/// and respawns each one a configurable delay after it dies.
///
/// For every region this builds an AIWaypointsGroup with a ring of AIWaypoints
/// snapped onto the baked NavMesh, then spawns the Zombie prefab nearby. The
/// UHFPS ZombiePatrolState automatically locks onto the closest waypoint group,
/// so each zombie patrols only its own region.
/// </summary>
public class ZombiePatrolSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PatrolRegion
    {
        public string Name = "Region";

        [Tooltip("Approximate world position of the region center. Snapped onto the NavMesh at runtime.")]
        public Vector3 Center;

        [Tooltip("Radius of the patrol waypoint ring around the center.")]
        public float PatrolRadius = 25f;

        [Tooltip("How many patrol waypoints to scatter around the center.")]
        public int WaypointCount = 5;

        [HideInInspector] public GameObject CurrentZombie;
        [HideInInspector] public AIWaypointsGroup Group;
        [HideInInspector] public bool IsDead;
    }

    [Header("Prefab")]
    [Tooltip("The UHFPS Zombie prefab (Assets/ThunderWire Studio/UHFPS/Content/Prefabs/NPC/Zombie.prefab).")]
    public GameObject ZombiePrefab;

    [Header("Patrol Regions")]
    public List<PatrolRegion> Regions = new();

    [Header("Spawning")]
    [Tooltip("Seconds to wait after a zombie dies before a fresh one is spawned in that region.")]
    public float RespawnDelay = 10f;

    [Tooltip("Search range used to snap region centers / waypoints / spawn points onto the baked NavMesh.")]
    public float NavSampleRange = 80f;

    public bool SpawnOnStart = true;

    private Transform _waypointsRoot;

    private void Reset()
    {
        BuildDefaultRegions();
    }

    private void BuildDefaultRegions()
    {
        // Spread across the developed area of Island_V2_cleaned2.
        // Y values are approximate (ground is ~-61); everything is snapped to the NavMesh.
        Regions = new List<PatrolRegion>
        {
            new PatrolRegion { Name = "North", Center = new Vector3(150f, -61f, -210f), PatrolRadius = 25f, WaypointCount = 5 },
            new PatrolRegion { Name = "West",  Center = new Vector3(90f,  -62f, -255f), PatrolRadius = 25f, WaypointCount = 5 },
            new PatrolRegion { Name = "East",  Center = new Vector3(380f, -55f, -380f), PatrolRadius = 25f, WaypointCount = 5 },
            new PatrolRegion { Name = "South", Center = new Vector3(245f, -61f, -445f), PatrolRadius = 25f, WaypointCount = 5 },
        };
    }

    private void Start()
    {
        if (!SpawnOnStart)
            return;

        if (Regions == null || Regions.Count == 0)
            BuildDefaultRegions();

        if (ZombiePrefab == null)
        {
            Debug.LogError("[ZombiePatrolSpawner] ZombiePrefab is not assigned. Aborting.");
            return;
        }

        StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        // Give a runtime NavMesh bake (if any) a frame to settle.
        yield return null;

        _waypointsRoot = new GameObject("PatrolWaypoints").transform;
        _waypointsRoot.SetParent(transform, false);

        foreach (var region in Regions)
        {
            BuildWaypointsForRegion(region);
            StartCoroutine(RegionRespawnLoop(region));
        }
    }

    private void BuildWaypointsForRegion(PatrolRegion region)
    {
        var groupGO = new GameObject("Waypoints_" + region.Name);
        groupGO.transform.SetParent(_waypointsRoot, false);
        groupGO.transform.position = SnapToNavMesh(region.Center, out var snapped)
            ? snapped
            : region.Center;

        if (groupGO.transform.position == region.Center)
            Debug.LogWarning($"[ZombiePatrolSpawner] Region '{region.Name}' center {region.Center} is not near the NavMesh. " +
                             "Move its Center onto walkable ground in the inspector.");

        var group = groupGO.AddComponent<AIWaypointsGroup>();
        group.Waypoints = new List<AIWaypoint>();
        region.Group = group;

        int placed = 0;
        for (int i = 0; i < region.WaypointCount; i++)
        {
            float ang = Mathf.Deg2Rad * (360f / Mathf.Max(1, region.WaypointCount)) * i;
            Vector3 ring = groupGO.transform.position +
                           new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * region.PatrolRadius;

            if (!SnapToNavMesh(ring, out var wpPos))
                continue;

            var wpGO = new GameObject($"WP_{region.Name}_{placed}");
            wpGO.transform.SetParent(groupGO.transform, false);
            wpGO.transform.position = wpPos;
            group.Waypoints.Add(wpGO.AddComponent<AIWaypoint>());
            placed++;
        }

        if (placed == 0)
            Debug.LogWarning($"[ZombiePatrolSpawner] Region '{region.Name}' produced 0 NavMesh waypoints. " +
                             "Check that a NavMesh is baked around its Center.");
    }

    private IEnumerator RegionRespawnLoop(PatrolRegion region)
    {
        while (true)
        {
            if (region.CurrentZombie == null)
            {
                region.CurrentZombie = SpawnZombie(region);
                yield return new WaitForSeconds(1f);
                continue;
            }

            bool gone = region.IsDead
                        || !region.CurrentZombie.activeInHierarchy;

            if (gone)
            {
                yield return new WaitForSeconds(RespawnDelay);
                if (region.CurrentZombie != null)
                    Destroy(region.CurrentZombie);
                region.CurrentZombie = null;
                region.IsDead = false;
                continue;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private GameObject SpawnZombie(PatrolRegion region)
    {
        Vector3 origin = region.Group != null ? region.Group.transform.position : region.Center;
        if (!SnapToNavMesh(origin, out var spawnPos))
        {
            Debug.LogWarning($"[ZombiePatrolSpawner] No NavMesh found to spawn a zombie in region '{region.Name}'.");
            return null;
        }

        var zombie = Instantiate(ZombiePrefab, spawnPos,
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        zombie.name = $"Zombie_{region.Name}";

        region.IsDead = false;
        var health = zombie.GetComponentInChildren<NPCHealth>();
        if (health != null)
            health.OnDeath.AddListener(() => region.IsDead = true);

        return zombie;
    }

    private bool SnapToNavMesh(Vector3 position, out Vector3 result)
    {
        if (NavMesh.SamplePosition(position, out var hit, NavSampleRange, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = position;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (Regions == null)
            return;

        Gizmos.color = Color.red;
        foreach (var region in Regions)
        {
            Gizmos.DrawWireSphere(region.Center, region.PatrolRadius);
            Gizmos.DrawLine(region.Center, region.Center + Vector3.up * 5f);
        }
    }
}
