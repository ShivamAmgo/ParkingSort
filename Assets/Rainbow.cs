using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rainbow : MonoBehaviour
{
    [SerializeField] Transform[] AllColors;
    [SerializeField] Vector3 FinalPos;
    [SerializeField]float JumpPower=3;
    [SerializeField] float Duration = 1;
    [SerializeField] int NumJumps = 2;
    Vector3 StartPos;
    private void Start()
    {
        StartPos=transform.position;
    }
    void PlayRainbow()
    {
        ActivateColors(true);
        transform.DOJump(FinalPos, JumpPower, NumJumps, Duration).SetEase(Ease.Linear).OnComplete(() => 
        {
            //ActivateColors(false);
            
            
        });

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            
            PlayRainbow();
        }
    }
    void ActivateColors(bool Activestatus)
    {
        foreach (Transform t in AllColors)
        {
            t.gameObject.SetActive(Activestatus);
        }
        transform.position = StartPos;
    }
}
