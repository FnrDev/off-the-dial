using UnityEngine;
using UHFPS.Runtime;

[RequireComponent(typeof(InteractableItem))]
public class CompleteTaskOnTake : MonoBehaviour
{
    [Tooltip("Task id passed to TaskListUI.Complete when this item is taken.")]
    public string taskId;

    private InteractableItem _item;

    private void Awake()
    {
        _item = GetComponent<InteractableItem>();
    }

    private void OnEnable()
    {
        if (_item != null) _item.OnTakeEvent.AddListener(OnTaken);
    }

    private void OnDisable()
    {
        if (_item != null) _item.OnTakeEvent.RemoveListener(OnTaken);
    }

    private void OnTaken()
    {
        if (string.IsNullOrEmpty(taskId)) return;
        TaskListUI.Complete(taskId);
    }
}
