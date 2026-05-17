using UnityEngine;

public class CandleInteract : MonoBehaviour
{
    [Header("Settings")]
    public int candleIndex;  // set this to 0,1,2,3,4 in Inspector
    public ChurchPuzzle churchPuzzle;  // drag ChurchPuzzleManager here

    private bool isExtinguished = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f))
            {
                if (hit.collider.gameObject == gameObject && !isExtinguished)
                {
                    isExtinguished = true;
                    churchPuzzle.ExtinguishCandle(candleIndex);
                    Debug.Log("Candle " + candleIndex + " extinguished!");
                }
            }
        }
    }
}