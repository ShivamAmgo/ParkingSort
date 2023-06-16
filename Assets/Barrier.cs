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
    [SerializeField] Vector3 TargetPosition;
    [SerializeField] float BarrierCloseDuration = 2;
    [SerializeField] float ClosingSpeedinTime = 1;
    [SerializeField] bool IsBarrierPost = false;
    Vector3 startingRot;
    Vector3 StartingPos;
    float TweenZ = 0;
    Tween ActiveTween;
    bool IsActive = true;
    
    
    private void Start()
    {
        startingRot = Target.eulerAngles;
        StartingPos = Target.position;
        TweenZ = startingRot.z;
        if(!IsBarrierPost)
            StartCoroutine(PlayBarrierAnimation());
        else
            StartCoroutine(PlayPostBarrierAnimation());
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
    IEnumerator PlayPostBarrierAnimation()
    {
        
        yield return new WaitForSeconds(BarrierCloseDuration);
        ActiveTween= Target.DOMoveY(TargetPosition.y, ClosingSpeedinTime).SetEase(Ease.Linear).OnStart(() =>
        {
            if (!IsActive)
            {
                ActiveTween.Kill();
            }
        });
        yield return new WaitForSeconds(BarrierCloseDuration+ClosingSpeedinTime);
        ActiveTween= Target.DOMoveY(StartingPos.y, ClosingSpeedinTime).SetEase(Ease.Linear).OnComplete(() => 
        {
            if (!IsActive) return;
            StartCoroutine(PlayPostBarrierAnimation());
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
            if(!IsBarrierPost)
            StopCoroutine(PlayBarrierAnimation());
            else
                StopCoroutine(PlayPostBarrierAnimation());
            DOTween.Kill(ActiveTween);
            
        }
    }
}
