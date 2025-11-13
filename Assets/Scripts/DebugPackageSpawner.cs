using UnityEngine;

public class DebugPackageSpawner : MonoBehaviour
{
    public GameObject LargePackagePrefab;
    public GameObject SmallPackagePrefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var package = Instantiate(SmallPackagePrefab, transform.position, Quaternion.identity);
            Destroy(package, 1f);    
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var package = Instantiate(LargePackagePrefab, transform.position, Quaternion.identity);
            Destroy(package, 1f);    
        }
    }
}
