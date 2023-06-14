using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelCompleteView : MonoBehaviour
{
    public Transform titleParent, emoji, button;

    private void OnEnable()
    {
        StartCoroutine(ShowUi());
    }

    IEnumerator ShowUi()
    {
        titleParent.gameObject.SetActive(true);
        titleParent.DOScale(Vector3.zero, 0.5f).From();
        
        yield return new WaitForSeconds(0.55f);
        emoji.gameObject.SetActive(true);
        emoji.DOScale(Vector3.zero, 0.5f).From();
        
        yield return new WaitForSeconds(0.55f);
        button.gameObject.SetActive(true);
        button.DOScale(Vector3.zero, 0.5f).From();
    }
}
