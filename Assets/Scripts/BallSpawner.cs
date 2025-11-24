using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BallSpawner : MonoBehaviour, ITriggerObjects
{
    [SerializeField]
    private GameObject ballPrefab;
    
    public void Trigger()
    {
        var ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
    }
}
