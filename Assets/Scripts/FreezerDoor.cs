using UnityEngine;

public class FreezerDoor : MonoBehaviour
{
    public bool isUnlocked = false;
    public float openAngle = 90f;
    public float openSpeed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
public AudioSource doorAudio;
    void Start()
    {
        closedRotation = transform.localRotation;
openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0));    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

   public void UnlockDoor()
{
    isUnlocked = true;
    isOpen = true;

    if (doorAudio != null)
        doorAudio.Play();

    Debug.Log("Freezer door unlocked and opened!");
}
}