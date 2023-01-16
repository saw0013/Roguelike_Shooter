using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using System.Linq;

public class ChangeTheme : MonoBehaviour
{
    private EventInstance instanceOne;

    private EventInstance instanceSecond;

    private void Awake()
    {
        instanceOne = RuntimeManager.CreateInstance("event:/Sound/Menu/TensionSoundtrack");
        instanceSecond = RuntimeManager.CreateInstance("event:/Sound/Menu/Wind");

        instanceOne.start();
        instanceOne.release();

        instanceSecond.start();
        instanceSecond.release();
    }

    public void ChangeBattleTheme(string BattlPatch)
    {
        //FindSound();
        instanceSecond.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        instanceOne = RuntimeManager.CreateInstance(BattlPatch);
        instanceOne.start();
        instanceOne.release();
    }

    public void ChangeAmbientTheme(string AmbientPatch)
    {
        //FindSound();
        instanceOne.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        instanceSecond = RuntimeManager.CreateInstance(AmbientPatch);
        instanceSecond.start();
        instanceSecond.release();
    }
}
