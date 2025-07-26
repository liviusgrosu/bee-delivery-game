using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private PickUp _pickUp;
    private void OnCollisionEnter(Collision other)
    {
        var obj = other.gameObject;
        if (obj.CompareTag("Player"))
        {
            var pickUp = obj.GetComponentInChildren<PickUp>();
            if (pickUp.PickedUpItem)
            {
                pickUp.DropPackage();
            }
        }
    }
}
