using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    int CarsCount = 0;
    int CarsReachedCount = 0;
    public delegate void RoundWin(bool WinStatus);
    public static event RoundWin SetWin;
    private void OnEnable()
    {
        Car.onDestinationReached += ReachedCar;
        Car.InfoDeliver += RecieveCarInfo;
    }

    

    private void OnDisable()
    {
        Car.onDestinationReached -= ReachedCar; 
        Car.InfoDeliver -= RecieveCarInfo;  
    }

    private void ReachedCar(Car car)
    {
        CarsReachedCount++;
        //Debug.Log("Cars Reached " + CarsReachedCount +" Out of "+CarsCount);
        if (CarsReachedCount >= CarsCount)
        {
            SetWin?.Invoke(true);
            Debug.Log("Won");
        }
    }
    private void RecieveCarInfo(Car car)
    {
        CarsCount++;
    }
}
