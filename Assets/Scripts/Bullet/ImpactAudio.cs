using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpactAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClipImpactRandom;
    [SerializeField] EImpactAudio impact;

    private AudioSource _audioSource;
    void Awake() => _audioSource = GetComponent<AudioSource>();

    private void Start()
    {
        switch(impact)
        {
            case EImpactAudio.metal:
                break;
            case EImpactAudio.wood:
                break;
            case EImpactAudio.sand:
                break;
            case EImpactAudio.concrete:
                break;

            case EImpactAudio.@default:
                _audioSource.clip = _audioClipImpactRandom[Random.Range(1, _audioClipImpactRandom.Length)];
                _audioSource.Play();
                break;
        }
    }

    //TODO : Найти звуки и сделать префикс

    enum EImpactAudio
    {
        wood,
        metal,
        sand,
        concrete,
        @default
    }

}
