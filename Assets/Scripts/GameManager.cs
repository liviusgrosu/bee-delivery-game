using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public struct CurrentJob
{
    private string pickupLocation;
    private string dropOffLocation;
    private float wieght;
    private float pay;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int PackageGoal = 5;
    
    private int _packagesDelivered;
    private int _currentScore;

    public Transform PickUpPoints;
    public Transform DropOffPoints;

    private Transform _currentPickupPoint;
    private Transform _currentDropoffPoint;
    
    // private List<Transform> PickUpPoints;
    // private List<Transform> DropOffPoints;
    private Transform _currentGoal;
    public GameObject BoxPrefab;
    
    public bool IsDoneLevel => _packagesDelivered >= PackageGoal;

    public GameObject PackageMarker;
    
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
        PackageMarker.SetActive(false);
        PlayerSpawnPoint.Instance.SpawnPlayer();
    }

    public void AssignJob(string start, string end)
    {
        _currentPickupPoint = PickUpPoints.Find($"{start} Pickup");
        _currentDropoffPoint = DropOffPoints.Find($"{end} Dropoff");

        _currentDropoffPoint.GetComponent<Goal>().IsCurrentGoal = true;
        
        Instantiate(BoxPrefab, _currentPickupPoint.position, BoxPrefab.transform.rotation);
        
        PackageMarker.SetActive(true);


        PackageMarker.GetComponent<Bobbing>().enabled = false;
        PackageMarker.transform.position = _currentPickupPoint.position + new Vector3(0, 1f, 0);
        PackageMarker.GetComponent<Bobbing>().enabled = true;
    }

    public void PickedUpPackage()
    {
        PackageMarker.GetComponent<Bobbing>().enabled = false;
        PackageMarker.transform.position = _currentDropoffPoint.position + new Vector3(0, 2f, 0);
        PackageMarker.GetComponent<Bobbing>().enabled = true;
    }

    public void DeliveredPackage()
    {
        PackageMarker.SetActive(false);
    }

    public void CompleteLevel()
    {
        throw new NotImplementedException();
    }
}
