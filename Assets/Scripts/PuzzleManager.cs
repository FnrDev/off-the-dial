using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public string collectedCode = "";
    public string correctCode = "714";
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
                puzzleSolved = true;

                if (inputText != null)
                {
                    inputText.text = "Access Granted - Door Unlocked";
                }

                if (door != null)
                {
                    door.transform.position += new Vector3(0, 3, 0);
                }
            }
            else
            {
                if (inputText != null)
                {
                    inputText.text = "Access Denied";
                }

                collectedCode = "";
                Invoke(nameof(ResetInputText), 1.2f);
            }
        }
    }

    void ResetInputText()
    {
        if (!puzzleSolved && inputText != null)
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