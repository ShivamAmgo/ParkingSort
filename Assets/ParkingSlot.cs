using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ParkingSlot : MonoBehaviour
{
    bool Parked = false;
    Tween Activetween;
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
            //Activetween.Kill();
            Parked = false;
           
                ParkingManager.Instance.CarLeaved(other.GetComponent<Car>());
            
        }
    }
}
