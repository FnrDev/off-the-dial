using UnityEngine;
using UnityEngine.AI;

public class CreaturePatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float waitTime = 2f;
    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2f;
    }

    void Update()
    {
        if (agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypoint].position);
                waitTimer = 0;
            }
        }
    }
}