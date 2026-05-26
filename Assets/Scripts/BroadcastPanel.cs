using UnityEngine;
using UHFPS.Runtime;

public class BroadcastPanel : MonoBehaviour
{
    public TowerEndingManager endingManager;
    public string requiredItemGUID = "2e96a8436e6e4cc1a3238e1a70f4ee2e";

    public void TryBroadcast()
    {
        if (Inventory.Instance.ContainsItem(requiredItemGUID))
        {
            Inventory.Instance.RemoveItem(requiredItemGUID);
            endingManager.TriggerGoodEnding();
            Debug.Log("BROADCAST SUCCESS - Good Ending!");
        }
        else
        {
            Debug.Log("Need VHS tape to broadcast!");
        }
    }
}
