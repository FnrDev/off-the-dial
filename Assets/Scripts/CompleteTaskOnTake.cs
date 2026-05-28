using UnityEngine;
using UHFPS.Runtime;

public class CompleteTaskOnTake : MonoBehaviour, IInteractStart
{
    [Tooltip("Task id passed to TaskListUI.Complete when this item is interacted with.")]
    public string taskId;

    public void InteractStart()
    {
        if (string.IsNullOrEmpty(taskId)) return;
        TaskListUI.Complete(taskId);
    }
}
