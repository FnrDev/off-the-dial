using UnityEngine;
using UHFPS.Runtime;

public class BroadcastPanel : MonoBehaviour
{
    public TowerEndingManager endingManager;
    public string requiredItemGUID = "2e96a8436e6e4cc1a3238e1a70f4ee2e";

    public void TryBroadcast()
    {
        if (Inventory.Instance.carryingItems != null &&
            CountTapes() >= 5)
        {
            for (int i = 0; i < 5; i++)
                Inventory.Instance.RemoveItem(requiredItemGUID);
            endingManager.TriggerGoodEnding();
            Debug.Log("BROADCAST SUCCESS - Good Ending!");
        }
        else
        {
            int count = CountTapes();
            Debug.Log("Need all 5 voice fragments. Currently have: " + count + "/5");

            // Show message to player via GameManager
            GameManager.Instance.ShowHintMessage(
                "Collect all 5 voice fragments. " +
                count + "/5 collected.", 3f);
        }
    }

    private int CountTapes()
    {
        int count = 0;
        foreach (var item in Inventory.Instance.carryingItems)
        {
            if (item.Key.ItemGuid == requiredItemGUID)
                count += item.Key.Quantity;
        }
        return count;
    }
}
