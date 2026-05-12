using UnityEngine;

public class BookPuzzleManager : MonoBehaviour
{
    public BookSlot[] slots;
    public string finalCode = "5937";

    public void CheckPuzzle()
    {
        if (slots == null || slots.Length == 0)
            return;

        foreach (BookSlot slot in slots)
        {
            if (slot == null || !slot.IsOccupied())
                return;
        }

        PuzzleSolved();
    }

    void PuzzleSolved()
    {
        Debug.Log("PUZZLE SOLVED!");
        Debug.Log("CODE = " + finalCode);
    }
}