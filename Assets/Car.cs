using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Car : MonoBehaviour
{
    //[SerializeField]SplineComputer splineAi;
    [SerializeField]SplineFollower m_splineFollower;
    [SerializeField] float FollowSpeed = 1;
    [SerializeField] Vector3 CollisionDir;
    
    [SerializeField] bool Interactable = false;
    [SerializeField] Transform CarBody;
    [SerializeField] GameObject CollisionFX;
    //[SerializeField] float ImpactForce;
    bool CanFollow = true;
    bool Collided = false;
    bool IsDestinationReached = false;
    bool Following = false;
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
    }

    private void OnDisable()
    {
        Car.OnCarCollision -= AfterCollision;
        ParkingManager.SetWin -= CheckWin;
    }
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.isKinematic = true;
        if (!Interactable) return;
        m_splineFollower = GetComponent<SplineFollower>();
            InfoDeliver?.Invoke(this);
        if (m_splineFollower == null) return;
        m_splineFollower.follow = false;
        m_splineFollower.onBeginningReached+=DestinationReached;
        m_splineFollower.onEndReached+=DestinationReached;
    }
    private void CheckWin(bool WinStatus)
    {
        if (WinStatus) 
        {
            CanFollow = false;
            m_splineFollower.follow = false;
        }
        Debug.Log("Won " + WinStatus);
    }

    private void AfterCollision()
    {
        CanFollow = false;
        if(m_splineFollower!=null)
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
        
    }
    public void DestinationReached(double d)
    {
        //Debug.Log("Pahuchhg gyaaa ");
        IsDestinationReached = true;
        //CanFollow = false;
        
        onDestinationReached?.Invoke(this);
        Following=false;
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
        if(m_splineFollower.direction==Spline.Direction.Forward)
            m_splineFollower.direction = Spline.Direction.Backward;
        else
            m_splineFollower.direction=Spline.Direction.Forward;


        CarBody.localEulerAngles += new Vector3(0, 180, 0);
        ActivateCar(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Collided) return;

        if (other.tag == "Car")
        {
            other.GetComponentInParent<Car>().CollisionEffect();
            
        }
        
        if (other.tag == "Car" || other.tag == "Barrier")
        {
            CollisionEffect();
        }
        
        //Debug.Log(transform.name+ " collides " +other.name);
        
        
    }
    public void CollisionEffect()
    {
        if (Collided) return;
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

}
