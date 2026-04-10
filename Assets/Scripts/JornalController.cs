using UnityEngine;
using UnityEngine.InputSystem;
public class JornalController : MonoBehaviour
{
[Header("Settings")]
    public GameObject journalPanel; 
    
    private bool isOpen = false;

    void Start()
    {
        // Start with the journal hidden
        if (journalPanel != null)
            journalPanel.SetActive(false);
    }

    void Update()
    {
        // click j key to open the jornal
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    void ToggleJournal()
    {
        isOpen = !isOpen;
        journalPanel.SetActive(isOpen);

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
