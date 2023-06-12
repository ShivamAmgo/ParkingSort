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
    public static ParkingManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //Application.targetFrameRate = 60;
    }
    private void OnEnable()
    {
        //Car.onDestinationReached += ReachedCar;
        Car.InfoDeliver += RecieveCarInfo;
    }

    

    private void OnDisable()
    {
        //Car.onDestinationReached -= ReachedCar; 
        Car.InfoDeliver -= RecieveCarInfo;  
    }

    public void ReachedCar(Car car)
    {
        CarsReachedCount++;
        //Debug.Log("Cars Reached " + CarsReachedCount +" Out of "+CarsCount);
        if (CarsReachedCount >= CarsCount)
        {
            SetWin?.Invoke(true);
            Debug.Log("Won");
        }
        
    }
    public void CarLeaved(Car car)
    {
        CarsReachedCount--;
        //Debug.Log("Cars Reached " + CarsReachedCount + " Out of " + CarsCount);
    }
    private void RecieveCarInfo(Car car)
    {
        CarsCount++;
    }
}
