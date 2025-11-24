using UnityEngine;

public class WaterPuzzleDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puzzle Ball"))
        {
            Destroy(other.gameObject);
        }
    }    
}
