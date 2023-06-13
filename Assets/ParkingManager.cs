using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParkingManager : MonoBehaviour
{
    [SerializeField] int StartSceneIndex = 0;
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
    public void Restart()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        /*
        if(GAScript.Instance)
            GAScript.Instance.LevelCompleted(SceneManager.GetActiveScene().name);
            */

        int index = SceneManager.GetActiveScene().buildIndex;
        index++;

        if (index <= SceneManager.sceneCountInBuildSettings - 1)
        {

            SceneManager.LoadScene(index);
        }
        else
        {
            SceneManager.LoadScene(StartSceneIndex);
        }
    }
}
