using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;
    private int _currentScore;
    public List<Transform> SpawnPoints;
    public List<Transform> Goals;
    private Transform _currentGoal;
    public GameObject BoxPrefab;
    void Start()
    {
        _scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        _scoreText.text = "Score: 0";
        PickNewGoal();
    }
    
    public void Score()
    {
        Instantiate(BoxPrefab, SpawnPoints[Random.Range(0,  SpawnPoints.Count)].position, BoxPrefab.transform.rotation);
        PickNewGoal();
        _currentScore++;
        _scoreText.text = "Score: " + _currentScore;
    }

    void PickNewGoal()
    {
        if (_currentGoal)
        {
            _currentGoal.GetComponent<Renderer>().material.color = Color.black;
        }
        
        _currentGoal = Goals[Random.Range(0, Goals.Count)];
        _currentGoal.GetComponent<Renderer>().material.color = Color.green;
        _currentGoal.GetComponent<Goal>().IsCurrentGoal = true;
    }
}
