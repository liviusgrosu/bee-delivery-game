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

    [SerializeField]
    private int _packageGoal = 1;
    public int PackagesDelivered;
    public float Paid;

    public Transform PickUpPoints;
    public Transform DropOffPoints;

    private Transform _currentPickupPoint;
    private Transform _currentDropoffPoint;
    
    private Transform _currentGoal;
    public GameObject BoxPrefab;
    
    public bool IsDoneLevel => PackagesDelivered >= _packageGoal;
    
    public static Action<bool> JobInProgress;
    private Action _onJobComplete;

    public GameObject PackageMarker;
    public Transform PlayerSpawn;
    
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
        UIManager.Instance.DeliveredPackagesText.text = $"Packages Delivered: {PackagesDelivered}/{_packageGoal}";
        UIManager.Instance.PayText.text = $"Pay: ${Paid}";
        
        PackageMarker.SetActive(false);
        PlayerSpawnPoint.Instance.SpawnPlayer();
    }

    public void AssignJob(string start, string end, float weight, float pay, Action onJobComplete)
    {
        JobInProgress?.Invoke(false);
        
        _onJobComplete = onJobComplete;
        _currentPickupPoint = PickUpPoints.Find($"{start} Pickup");
        _currentDropoffPoint = DropOffPoints.Find($"{end} Dropoff");

        _currentDropoffPoint.GetComponent<Goal>().IsCurrentGoal = true;
        
        var package = Instantiate(BoxPrefab, _currentPickupPoint.position, BoxPrefab.transform.rotation);
        var packageComponent =  package.GetComponent<Package>();
        packageComponent.Weight = weight;
        packageComponent.PayOut = pay;
        
        PackageMarker.SetActive(true);
        PackageMarker.GetComponent<Bobbing>().enabled = false;
        PackageMarker.transform.position = _currentPickupPoint.position + new Vector3(0, 4f, 0);
        PackageMarker.GetComponent<Bobbing>().enabled = true;
    }

    public void PickedUpPackage()
    {
        PackageMarker.GetComponent<Bobbing>().enabled = false;
        PackageMarker.transform.position = _currentDropoffPoint.position + new Vector3(0, 4f, 0);
        PackageMarker.GetComponent<Bobbing>().enabled = true;
    }

    public void DeliveredPackage(float payOut)
    {
        JobInProgress?.Invoke(true);
        _onJobComplete?.Invoke();
        _onJobComplete = null;
        
        PackagesDelivered++;
        Paid += payOut;
        UIManager.Instance.DeliveredPackagesText.text = $"Packages Delivered: {PackagesDelivered}/{_packageGoal}";
        UIManager.Instance.PayText.text = $"Pay: ${Paid}";
        PackageMarker.SetActive(false);

        if (IsDoneLevel)
        {
            PackageMarker.SetActive(true);
            PackageMarker.GetComponent<Bobbing>().enabled = false;
            PackageMarker.transform.position = PlayerSpawn.position + new Vector3(0, 2f, 0);
            PackageMarker.GetComponent<Bobbing>().enabled = true;
            UIManager.Instance.GoBackHomeText.gameObject.SetActive(true);
        }
    }

    public void CompleteLevel()
    {
        throw new NotImplementedException();
    }
}
