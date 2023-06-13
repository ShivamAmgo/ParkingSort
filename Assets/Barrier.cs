using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Xml;
using Unity.VisualScripting;

public class Barrier : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform Target;
    [SerializeField] Vector3 TargetRotation;
    [SerializeField] float BarrierCloseDuration = 2;
    [SerializeField] float ClosingSpeedinTime = 1;
    Vector3 startingRot;
    float TweenZ = 0;
    Tween ActiveTween;
    bool IsActive = true;
    
    private void Start()
    {
        startingRot = Target.eulerAngles;
        TweenZ = startingRot.z;
        StartCoroutine(PlayBarrierAnimation());
    }
    IEnumerator PlayBarrierAnimation()
    {
        
        yield return new WaitForSeconds(BarrierCloseDuration);
        ActiveTween= DOTween.To(() => TweenZ, value => TweenZ = value, TargetRotation.z, ClosingSpeedinTime).SetEase(Ease.Linear).OnUpdate(() => 
        {
            if (!IsActive) return;
            Target.eulerAngles = new Vector3(Target.eulerAngles.x, Target.eulerAngles.y, TweenZ);
        });
        //TweenZ=Target.eulerAngles.z;

        yield return new WaitForSeconds(BarrierCloseDuration);
        ActiveTween= DOTween.To(() => TweenZ, value => TweenZ = value, startingRot.z, ClosingSpeedinTime).SetEase(Ease.Linear).OnUpdate(() =>
        {
            if (!IsActive) return;
            Target.eulerAngles = new Vector3(Target.eulerAngles.x, Target.eulerAngles.y, TweenZ);
        }).OnComplete(() => 
        {
            TweenZ = startingRot.z;
            if (!IsActive) return;
            StartCoroutine(PlayBarrierAnimation());
        });
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (!IsActive) return;
        Debug.Log("Barricades");
        if (other.tag=="Car")
        {
            IsActive = false;
            Debug.Log("Car takrayi");
            StopCoroutine(PlayBarrierAnimation());
            DOTween.Kill(ActiveTween);
            
            

        }
    }
}
