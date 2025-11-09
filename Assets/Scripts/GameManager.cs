using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int _packageGoal = 1;
    public int PackagesDelivered;
    public float Paid;

    public Transform DeliveryPoints;
    
    private Transform _currentPickupPoint;
    private Transform _currentDropoffPoint;
    
    private Transform _currentGoal;
    
    public bool IsDoneLevel => PackagesDelivered >= _packageGoal;
    
    public static Action<bool> JobInProgress;
    private Action _onJobComplete;
    private const string AnimalResourceFolder = "Prefabs/Animals";

    [HideInInspector] public string[] AnimalList = {"Colobus", "Gecko", "Herring", "Pudu", "Sparrow", "Squid", "Taipan"};

    public bool DeliveringPackage;
    
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
    }

    public void AssignJob(string start, string end, float pay, Action onJobComplete)
    {
        JobInProgress?.Invoke(false);
        
        _onJobComplete = onJobComplete;
        _currentPickupPoint = DeliveryPoints.Find(start);
        _currentDropoffPoint = DeliveryPoints.Find(end);
        _currentPickupPoint.GetComponent<DeliveryPOI>().Init(true, pay);
        DeliveringPackage = false;
    }

    public void PickedUpPackage()
    {
        if (!DeliveringPackage)
        {
            _currentPickupPoint.GetComponent<DeliveryPOI>().Disable();
            _currentDropoffPoint.GetComponent<DeliveryPOI>().Init(false);
            DeliveringPackage = true;
        }
    }

    public void DeliveredPackage(float payOut)
    {
        DeliveringPackage = false;
        PackagesDelivered++;
        JobInProgress?.Invoke(true);
        _onJobComplete?.Invoke();
        _onJobComplete = null;
        Paid += payOut;
        UIManager.Instance.DeliveredPackagesText.text = $"Packages Delivered: {PackagesDelivered}/{_packageGoal}";
        UIManager.Instance.PayText.text = $"Pay: ${Paid}";
        
    }
}
