
using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{

public TextMeshProUGUI resultText;
    public static PuzzleManager Instance;

    public string collectedCode = "";
    public string correctCode = "121";

    public TextMeshProUGUI inputText;
    public GameObject door;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddDigit("1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddDigit("2");
        if (Input.GetKeyDown(KeyCode.Alpha3)) AddDigit("3");
        if (Input.GetKeyDown(KeyCode.Alpha4)) AddDigit("4");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (collectedCode.Length > 0)
                collectedCode = collectedCode.Substring(0, collectedCode.Length - 1);
        }

        UpdateUI();

        if (collectedCode == correctCode)
        {
            OpenDoor();
        }

        if (collectedCode == correctCode)
{
    resultText.text = "ACCESS GRANTED";
    OpenDoor();
}
else if (collectedCode.Length == 3)
{
    resultText.text = "ACCESS DENIED";
}
if (Input.GetKeyDown(KeyCode.R))
{
    collectedCode = "";
}
    }

    public void AddDigit(string digit)
    {
        if (collectedCode.Length < 3)
            collectedCode += digit;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (inputText != null)
            inputText.text = "Code: " + collectedCode;
    }

    void OpenDoor()
    {
        Debug.Log("Door Opened!");

        if (door != null)
            door.SetActive(false);
    }
}
// using UnityEngine;
// using TMPro;

// public class PuzzleManager : MonoBehaviour
// {
//     public static PuzzleManager Instance;

//     public string collectedCode = "";
//     public string correctCode = "121";
//     public TextMeshProUGUI inputText;
//     public GameObject door;

//     private bool puzzleSolved = false;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     public void AddDigit(string digit)
//     {
//         if (puzzleSolved) return;

//         collectedCode += digit;
//         UpdateInputDisplay(collectedCode);
//         CheckCode();
//     }

//     void CheckCode()
//     {
//         if (collectedCode.Length >= correctCode.Length)
//         {
//             if (collectedCode == correctCode)
//             {
//                 puzzleSolved = true;

//                 if (inputText != null)
//                 {
//                     inputText.text = "Access Granted - Door Unlocked";
//                 }

//                 if (door != null)
//                 {
//                     door.transform.position += new Vector3(0, 3, 0);
//                 }
//             }
//             else
//             {
//                 if (inputText != null)
//                 {
//                     inputText.text = "Access Denied";
//                 }

//                 collectedCode = "";
//                 Invoke(nameof(ResetInputText), 1.2f);
//             }
//         }
//     }

//     void ResetInputText()
//     {
//         if (!puzzleSolved && inputText != null)
//         {
//             inputText.text = "Code:";
//         }
//     }

//     public void UpdateInputDisplay(string currentInput)
//     {
//         if (inputText != null)
//         {
//             inputText.text = "Code: " + currentInput;
//         }
//     }
// }


