using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonyColorsPro.ShaderGenerator.Enums;

public class Car : MonoBehaviour
{
    [SerializeField]SplineComputer splineAi;
    [SerializeField]SplineFollower m_splineFollower;
    [SerializeField] float FollowSpeed = 1;
    [SerializeField] Vector3 CollisionDir;
    [SerializeField] Collider BoxTrigger;
    [SerializeField] bool Interactable = false;
    [SerializeField] Transform CarBody;
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
    }
    private void OnDisable()
    {
        Car.OnCarCollision -= AfterCollision;  
    }

    private void AfterCollision()
    {
        CanFollow = false;
        if(m_splineFollower!=null)
        m_splineFollower.follow = false;
        //Collided = true;
       
        RB.isKinematic = false;
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        if(Interactable)
        InfoDeliver?.Invoke(this);
    }
    public void ActivateCar(bool activestatus)
    {
        if (!CanFollow) return;
        Following = true;
        m_splineFollower.followSpeed = FollowSpeed;
        m_splineFollower.follow = activestatus;
        IsActive = true;
        
    }
    public void DestinationReached()
    {
        Debug.Log("Pahuchhg gyaaa ");
        IsDestinationReached = true;
        //CanFollow = false;
        
        onDestinationReached?.Invoke(this);
        Following=false;
    }
    private void OnMouseUp()
    {
        if (m_splineFollower == null || Following) return;
        if (IsDestinationReached)
        {
            RotateCar();
        }
        ActivateCar(true);
    }
    void RotateCar()
    {
        
        m_splineFollower.direction = Spline.Direction.Backward;
        CarBody.localEulerAngles += new Vector3(0, 180, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Collided || other.transform.tag != "Car") return;

        Collided = true;
        CollisionEffect();
        
    }
    void CollisionEffect()
    {
        //BoxTrigger.enabled = false;
        RB.isKinematic = false;
        if (IsActive)
        {
           
            RB.AddForce(CollisionDir.z * transform.forward, ForceMode.Impulse);
        }
        
        OnCarCollision?.Invoke();
    }


}
