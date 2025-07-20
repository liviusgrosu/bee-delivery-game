using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Package Menu/Order")]
public class OrderData : ScriptableObject
{
    public string Pickup;
    public string Dropoff;
    public float Pay;
    public float Weight;
}
