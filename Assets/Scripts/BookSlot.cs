using UnityEngine;

public class BookSlot : MonoBehaviour
{
    [Header("Correct Book")]
    public int requiredPuzzleID;

    public BookPuzzleManager puzzleManager;

    private bool occupied = false;

    private void OnTriggerEnter(Collider other)
    {
        if (occupied)
            return;

        PuzzleBook book = other.GetComponent<PuzzleBook>();

        if (book == null)
            return;

        if (book.puzzleID == requiredPuzzleID)
        {
            AcceptBook(book);
        }
        else
        {
            RejectBook(book);
        }
    }

    void AcceptBook(PuzzleBook book)
    {
        occupied = true;

        book.transform.position = transform.position;
        book.transform.rotation = transform.rotation;

        Rigidbody rb = book.GetComponent<Rigidbody>();

        if (rb != null)
            rb.isKinematic = true;

        Debug.Log("Correct Book Placed!");

        puzzleManager.CheckPuzzle();
    }

    void RejectBook(PuzzleBook book)
    {
        Debug.Log("Wrong Book!");
    }

    public bool IsOccupied()
    {
        return occupied;
    }
}