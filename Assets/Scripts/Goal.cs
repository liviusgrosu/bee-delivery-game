using UnityEngine;

public class Goal :  MonoBehaviour
{
    public bool IsCurrentGoal;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Box") && IsCurrentGoal)
        {
            _gameManager.Score();
            Destroy(collision.gameObject);
        }
    }
}
