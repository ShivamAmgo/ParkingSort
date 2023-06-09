using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PoliceLight : MonoBehaviour
{
    [SerializeField] float LightsDelay = 0.5f;
    [SerializeField] Light BlueLight;
    [SerializeField] Light RedLight;


    private void OnEnable()
    {
        Car.OnCarCollision += CallPolice; 
    }
    private void OnDisable()
    {
        Car.OnCarCollision -= CallPolice; 
    }

    private void CallPolice()
    {
        StartCoroutine(PlayLightsAnimation());
    }

    private void Start()
    {
        
    }
    IEnumerator PlayLightsAnimation()
    {
        BlueLight.enabled = true;
        RedLight.enabled = false;
        yield return new WaitForSeconds(LightsDelay);
        RedLight.enabled = true;
        BlueLight.enabled = false;
        yield return new WaitForSeconds(LightsDelay);
        StartCoroutine(PlayLightsAnimation());

    }
}
