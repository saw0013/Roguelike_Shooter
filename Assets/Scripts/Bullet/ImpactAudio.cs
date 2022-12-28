using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class ImpactAudio : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter[] _audioClipImpactRandom;
    [SerializeField] EImpactAudio impact;

    private StudioEventEmitter _audioSource;
    void Awake() => _audioSource = GetComponent<StudioEventEmitter>();

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
                _audioSource = _audioClipImpactRandom[Random.Range(1, _audioClipImpactRandom.Length)];
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
