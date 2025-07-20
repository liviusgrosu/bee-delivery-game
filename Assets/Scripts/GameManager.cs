using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int PackageGoal = 5;
    
    private int _packagesDelivered;
    private int _currentScore;
    public List<Transform> SpawnPoints;
    public List<Transform> Goals;
    private Transform _currentGoal;
    public GameObject BoxPrefab;
    
    public bool IsDoneLevel => _packagesDelivered >= PackageGoal;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        PlayerSpawnPoint.Instance.SpawnPlayer();
        UIManager.Instance.ScoreText.text = "Score: 0";
        UIManager.Instance.DeliveredPackagesText.text = $"Packages Remaining: {PackageGoal}";
        Instantiate(BoxPrefab, SpawnPoints[Random.Range(0,  SpawnPoints.Count)].position, BoxPrefab.transform.rotation);
        
        PickNewGoal();
    }
    
    public void Score()
    {
        Instantiate(BoxPrefab, SpawnPoints[Random.Range(0,  SpawnPoints.Count)].position, BoxPrefab.transform.rotation);
        PickNewGoal();
        _currentScore++;
        _packagesDelivered++;
        UIManager.Instance.ScoreText.text = "Score: " + _currentScore;
        UIManager.Instance.DeliveredPackagesText.text = $"Packages Remaining: {PackageGoal - _packagesDelivered}";
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

    public void CompleteLevel()
    {
        throw new NotImplementedException();
    }
}
