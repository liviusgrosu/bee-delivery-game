using UnityEngine;

public class Goal :  MonoBehaviour
{
    public bool IsCurrentGoal;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Box") && IsCurrentGoal)
        {
            GameManager.Instance.DeliveredPackage();
            IsCurrentGoal = false;
            Destroy(collision.gameObject);
        }
    }
}
