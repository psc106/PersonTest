﻿using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SpawnEffectsBase : MonoBehaviour
{
    [SerializeField] GameObject spawnVFX;
    [SerializeField] float animationDuration = 1f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
        if(spawnVFX != null)
        {
            Instantiate(spawnVFX, transform.position, Quaternion.identity);

        }

        GetComponent<AudioSource>().Play();
    }


}