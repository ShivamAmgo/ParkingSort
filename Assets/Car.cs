using DG.Tweening;
using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Car : MonoBehaviour
{
    //[SerializeField]SplineComputer splineAi;
    [SerializeField] SplineFollower m_splineFollower;
    [SerializeField] float FollowSpeed = 1;
    [SerializeField] Vector3 CollisionDir;

    [SerializeField] bool Interactable = false;
    [SerializeField] Transform CarBody;

    [SerializeField] GameObject CollisionFX;

    //[SerializeField] Transform CarBodyChild;
    [SerializeField] float BrakeTilt = 5;
    [SerializeField] float BrakeAnimationDuration = 0.25f;

    AudioSource audioSource;

    //[SerializeField] float ImpactForce;
    bool CanFollow = true;
    bool Collided = false;
    bool IsDestinationReached = false;
    bool Following = false;
    AudioClip BrakesSound;
    AudioClip CarRevSound;

    public delegate void OnDestinationReached(Car car);

    public delegate void CarCollision();

    public delegate void DeliverCarInfo(Car car);

    public static event DeliverCarInfo InfoDeliver;
    public static event CarCollision OnCarCollision;
    public static event OnDestinationReached onDestinationReached;
    Rigidbody RB;
    bool IsActive = false;

    private void OnEnable()
    {
        Car.OnCarCollision += AfterCollision;
        ParkingManager.SetWin += CheckWin;
        ParkingManager.SendAllSounds += OnSendAllSounds;
    }

    private void OnDisable()
    {
        Car.OnCarCollision -= AfterCollision;
        ParkingManager.SetWin -= CheckWin;
        ParkingManager.SendAllSounds -= OnSendAllSounds;
    }

    private void OnSendAllSounds(List<AudioClip> allSounds)
    {
        BrakesSound = allSounds[1];
        CarRevSound = allSounds[0];
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.isKinematic = true;
        audioSource = GetComponentInParent<AudioSource>();
        m_splineFollower = GetComponent<SplineFollower>();
        SplineComputer SC = m_splineFollower.spline;
        if (!Interactable)
        {
            //SplineMesh sm = GetComponent<SplineMesh>();
            //sm.enabled = false;
            SC.transform.GetComponent<MeshRenderer>().enabled = false;
            SC.transform.GetComponent<SplineMesh>().enabled = false;
            return;
        }


        SC.transform.GetComponent<MeshRenderer>().enabled = true;
        SC.transform.GetComponent<SplineMesh>().enabled = false;
        InfoDeliver?.Invoke(this);
        if (m_splineFollower == null) return;
        m_splineFollower.follow = false;
        m_splineFollower.onBeginningReached += DestinationReached;
        m_splineFollower.onEndReached += DestinationReached;
    }

    private void CheckWin(bool WinStatus)
    {
        if (!Interactable) return;
        if (WinStatus)
        {
            CanFollow = false;
            m_splineFollower.follow = false;
        }

        Debug.Log("Won " + WinStatus);
    }

    private void AfterCollision()
    {
        audioSource.Stop();
        CanFollow = false;
        if (m_splineFollower != null)
            m_splineFollower.follow = false;
        //Collided = true;

        RB.isKinematic = false;
    }


    public void ActivateCar(bool activestatus)
    {
        if (!CanFollow) return;
        Following = true;
        m_splineFollower.followSpeed = FollowSpeed;
        m_splineFollower.follow = activestatus;
        IsActive = true;
        PlayAudioFX(CarRevSound, true);
        Vibration.Vibrate(30);
        Debug.Log("TapOncar");
    }

    public void DestinationReached(double d)
    {
        Debug.Log("Pahuchhg gyaaa ");
        IsDestinationReached = true;
        //CanFollow = false;


        if (m_splineFollower.direction == Spline.Direction.Backward)
        {
            PlayBrakeAnimation(-BrakeTilt);
        }
        else
        {
            PlayBrakeAnimation(BrakeTilt);
        }

        onDestinationReached?.Invoke(this);
        Following = false;
        Vibration.Vibrate(30);

    }

    private void OnMouseUp()
    {
        if (m_splineFollower == null || Following || !Interactable) return;
        if (IsDestinationReached)
        {
            RotateCar();
            return;
        }

        ActivateCar(true);
    }

    void RotateCar()
    {
        if (m_splineFollower.direction == Spline.Direction.Forward)
            m_splineFollower.direction = Spline.Direction.Backward;
        else
            m_splineFollower.direction = Spline.Direction.Forward;


        CarBody.localEulerAngles += new Vector3(0, 180, 0);
        ActivateCar(true);
    }

    public void PlayBrakeAnimation(float Rot_x)
    {
        Vector3 carbodyangles = CarBody.localEulerAngles;
        PlayAudioFX(BrakesSound, false);
        DOTween.To(() => carbodyangles, value => carbodyangles = value, carbodyangles + new Vector3(Rot_x, 0, 0),
            BrakeAnimationDuration).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).OnUpdate
            (() => { CarBody.localEulerAngles = carbodyangles; });
    }

    public void LastCarBrakeAnimation()
    {
        PlayBrakeAnimation(BrakeTilt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Collided) return;

        if (other.tag == "Car")
        {
            other.GetComponentInParent<Car>().CollisionEffect();
            Vibration.Vibrate(30);

        }

        if (other.tag == "Car" || other.tag == "Barrier")
        {
            CollisionEffect();
            Vibration.Vibrate(30);

            
        }

        //Debug.Log(transform.name+ " collides " +other.name);
    }

    public void CollisionEffect()
    {
        if (Collided) return;
        audioSource.Stop();
        Collided = true;
        RB.isKinematic = false;
        if (IsActive)
        {
            RB.AddForce(CollisionDir.z * transform.forward, ForceMode.Impulse);
            //Debug.Log(transform.name + " forced");
        }

        //Debug.Log(transform.name + " Collided");
        PlayCollideFX(CollisionFX);
        OnCarCollision?.Invoke();
    }

    void PlayCollideFX(GameObject FX)
    {
        if (FX == null) return;
        FX.SetActive(false);
        FX.SetActive(true);
    }

    void PlayAudioFX(AudioClip clip, bool IsLooping)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = IsLooping;
        audioSource.Play();
    }
}