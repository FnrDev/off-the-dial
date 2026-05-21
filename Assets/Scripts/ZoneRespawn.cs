using UnityEngine;

/// <summary>
/// Wire RespawnPlayer() to GhostPatrol.OnPlayerCaught in the Inspector.
/// Assign spawnPoint (GasStation_SpawnPoint) and ghost (the Ghost GameObject).
/// </summary>
public class ZoneRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GhostPatrol ghost;

    public void RespawnPlayer()
    {
        // ── 1. Teleport player ────────────────────────────────────────────────
        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // Disable CharacterController before moving so Unity doesn't
                // snap the position back on the same frame.
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;

                if (cc != null) cc.enabled = true;
            }
        }

        // ── 2. Reset ghost to WP1 and resume patrol ───────────────────────────
        if (ghost != null)
            ghost.ResetToFirstWaypoint();
    }
}
