using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSlot : MonoBehaviour
{
    bool Parked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (Parked) return;
        if (other.tag == "Car")
        {
            Parked = true;
            ParkingManager.Instance.ReachedCar(other.GetComponent<Car>());
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!Parked) return;
        if (other.tag == "Car")
        {
            Parked = false;
            ParkingManager.Instance.CarLeaved(other.GetComponent<Car>());

        }
    }
}
