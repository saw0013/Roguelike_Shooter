using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Mirror;
using FMODUnity;
using UnityEngine;

public class PlayAudio : NetworkBehaviour
{
    //TODO: ������� �������, ������� ���������� ���� ��� ���� �������

    private StudioEventEmitter sound;

    private void Start() => sound = GetComponent<StudioEventEmitter>();

    public void PlayAudioOnPlayer() => CmdPlayAudio();

    [Command]
    private void CmdPlayAudio() => RpcPlayAudio();

    [ClientRpc]
    private void RpcPlayAudio() => sound.Play();
}
