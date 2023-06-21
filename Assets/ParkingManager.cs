using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ParkingManager : MonoBehaviour
{
    [SerializeField] int StartSceneIndex = 0;
    [SerializeField] GameObject[] Win_FailPanel;
    [SerializeField] float WinLOsePanelDelay = 1.5f;
    [SerializeField] List<AudioClip> AllSounds;
    [SerializeField] TextMeshProUGUI LevelNumber;
    AudioSource audioSource;
    int CarsCount = 0;
    int CarsReachedCount = 0;

    public delegate void RoundWin(bool WinStatus);

    public static event RoundWin SetWin;
    bool CollisionStatus = false;

    public delegate void AllMusicSend(List<AudioClip> allSounds);

    public static event AllMusicSend SendAllSounds;
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

        Vibration.Init();

        //Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        //Car.onDestinationReached += ReachedCar;
        Car.InfoDeliver += RecieveCarInfo;
        Car.OnCarCollision += AfterCarCollision;
    }


    private void OnDisable()
    {
        //Car.onDestinationReached -= ReachedCar; 
        Car.InfoDeliver -= RecieveCarInfo;
        Car.OnCarCollision -= AfterCarCollision;
    }

    private void Start()
    {
        if (GAScript.Instance)
        {
            GAScript.Instance.LevelStart(PlayerPrefs.GetInt("Level", 1).ToString());
        }

        SendAllSounds?.Invoke(AllSounds);
        audioSource = GetComponent<AudioSource>();
        LevelNumber.text = "Level " + PlayerPrefs.GetInt("Level", 1);
    }

    public void ReachedCar(Car car)
    {
        CarsReachedCount++;
       // Debug.Log("Cars Reached " + CarsReachedCount + " Out of " + CarsCount);
        if (CarsReachedCount >= CarsCount)
        {
            SetWin?.Invoke(true);
            Debug.Log("Won");
            car.LastCarBrakeAnimation();
            DOVirtual.DelayedCall(WinLOsePanelDelay, () =>
            {
                Win_FailPanel[0].SetActive(true);
                audioSource.clip = AllSounds[2];
                audioSource.Play();
            });
        }
    }

    private void AfterCarCollision()
    {
        if (CollisionStatus)
        {
            return;
        }

        CollisionStatus = true;
        DOVirtual.DelayedCall(WinLOsePanelDelay, () =>
        {
            Win_FailPanel[1].SetActive(true);
            audioSource.clip = AllSounds[3];
            audioSource.Play();
        });
    }

    public void CarLeaved(Car car)
    {
        CarsReachedCount--;
       // Debug.Log("Cars Reached " + CarsReachedCount + " Out of " + CarsCount);
    }

    private void RecieveCarInfo(Car car)
    {
        CarsCount++;
    }

    public void Restart()
    {
        if (Win_FailPanel[1].activeInHierarchy && GAScript.Instance && ISManager.instance)
        {
            GAScript.Instance.LevelFail(PlayerPrefs.GetInt("Level", 1).ToString());
            ISManager.instance.ShowInterstitialAds();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DOTween.KillAll();
        Vibration.Vibrate(30);
    }

    public void NextLevel()
    {
        if (GAScript.Instance)
            GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("Level", 1).ToString());
        if (ISManager.instance)
            ISManager.instance.ShowInterstitialAds();
        // int index = SceneManager.GetActiveScene().buildIndex;
        // index++;
        //
        // if (index <= SceneManager.sceneCountInBuildSettings - 1)
        // {
        //
        //     SceneManager.LoadScene(index);
        // }
        // else
        // {
        //     SceneManager.LoadScene(StartSceneIndex);
        // }

        if (PlayerPrefs.GetInt("Level") >= (SceneManager.sceneCountInBuildSettings) - 1)
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
            var i = Random.Range(2, SceneManager.sceneCountInBuildSettings);
            PlayerPrefs.SetInt("ThisLevel", i);
            SceneManager.LoadScene(i);
        }
        else
        {
            PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        Vibration.Vibrate(30);
    }
}