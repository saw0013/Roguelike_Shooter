using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Mirror;
using FMODUnity;
using UnityEngine;

public class PlayAudio : NetworkBehaviour
{
    //TODO: Сделать утилиту, которая производит звук для всех игроков

    private StudioEventEmitter sound;

    private void Start() => sound = GetComponent<StudioEventEmitter>();

    public void PlayAudioOnPlayer()
    {
        if (hasAuthority) CmdPlayAudio();
        else Debug.LogWarning("Dont HasAuthority");
    } 

    [Command]
    private void CmdPlayAudio() => RpcPlayAudio();

    [ClientRpc]
    private void RpcPlayAudio() => sound.Play();
}
