using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMOD.Studio;
using Mirror;
using UnityEngine;

public class CameraToPlayerTarget : NetworkBehaviour
{
    // Start is called before the first frame update
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    //public override void OnStartLocalPlayer()
    //{
    //    vcam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    //    base.OnStartLocalPlayer();
    //}

}

