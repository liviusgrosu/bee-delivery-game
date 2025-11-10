using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryPOI : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("Active");
    private static readonly int Inactive = Animator.StringToHash("Inactive");
    
    public Transform AnimalSpawn;
    public Transform PackageSpawn;
    public GameObject Marker;
    private Collider _collider;

    private const string AnimalResourceFolder = "Animals";
    [SerializeField] private GameObject packagePrefab;
    private GameObject _animalInstance;
    
    private void Awake()
    {
        Marker.SetActive(false);
        _collider = GetComponent<SphereCollider>();
    }
    
    public void Init(bool state, float packageHealth = 100f, float payout = 0f, float potentialTip = 0f)
    {
        var animalList = GameManager.Instance.AnimalList;
        var selectedAnimal = animalList[Random.Range(0, animalList.Length)];
        var animalPrefab = Resources.Load<GameObject>($"{AnimalResourceFolder}/{selectedAnimal}");
        _animalInstance = Instantiate(animalPrefab, AnimalSpawn.position, AnimalSpawn.rotation);
        _animalInstance.transform.parent = AnimalSpawn;
        _animalInstance.GetComponent<Animator>().SetTrigger(Active);

        if (state)
        {
            var package = Instantiate(packagePrefab, PackageSpawn.position, PackageSpawn.rotation);
            package.GetComponent<Package>().PayOut = payout;
            package.GetComponent<Package>().PotentialTip = potentialTip;
            package.GetComponent<Package>().SetHealth(packageHealth);
        }
        
        _collider.enabled = !state;
        Marker.SetActive(true);
    }

    public void Disable()
    {
        _collider.enabled = false;
        _animalInstance.GetComponent<Animator>().SetTrigger(Inactive);
        Destroy(_animalInstance, 10f);
        Marker.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.DeliveringPackage &&
            !PackagePickupController.Instance.IsHoldingPackage && 
            other.transform.CompareTag("Package"))
        {
            var package = other.GetComponent<Package>();
            var totalTip = package.PotentialTip * package.GetHealthPercentage();
            package.Interactable = false;
            Destroy(other.gameObject, 10f);
            Disable();
            GameManager.Instance.DeliveredPackage(package.PayOut, totalTip);
        }
    }
}
