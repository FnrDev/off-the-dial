using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public string collectedCode = "";
    public string correctCode = "417";
    public TextMeshProUGUI inputText;
    public GameObject door;

    private bool puzzleSolved = false;

    private void Awake()
    {
        Instance = this;
    }

    public void AddDigit(string digit)
    {
        if (puzzleSolved) return;

        collectedCode += digit;
        UpdateInputDisplay(collectedCode);
        CheckCode();
    }

    void CheckCode()
    {
        if (collectedCode.Length >= correctCode.Length)
        {
            if (collectedCode == correctCode)
            {
                Debug.Log("✅ CORRECT CODE - Puzzle Solved!");
                puzzleSolved = true;

                if (inputText != null)
                {
                    inputText.text = "Access Granted ✔ Door Unlocked";
                }

                if (door != null)
                {
                    door.transform.position += new Vector3(0, 3, 0);
                }
            }
            else
            {
                Debug.Log("❌ WRONG CODE - Resetting...");

                if (inputText != null)
                {
                    inputText.text = "Access Denied ❌";
                }

                collectedCode = "";
                Invoke(nameof(ResetInputText), 1.5f);
            }
        }
    }
void ResetInputText()
{
    if (inputText != null && !puzzleSolved)
    {
        inputText.text = "Code:";
    }
}
    public void UpdateInputDisplay(string currentInput)
    {
        if (inputText != null)
        {
            inputText.text = "Code: " + currentInput;
        }
    }
}