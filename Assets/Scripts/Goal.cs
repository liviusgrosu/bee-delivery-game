using UnityEngine;

public class Goal :  MonoBehaviour
{
    public bool IsCurrentGoal;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Box") && IsCurrentGoal)
        {
            var pay = collision.gameObject.GetComponent<Package>().PayOut;
            GameManager.Instance.DeliveredPackage(pay);
            IsCurrentGoal = false;
            Destroy(collision.gameObject);
        }
    }
}
