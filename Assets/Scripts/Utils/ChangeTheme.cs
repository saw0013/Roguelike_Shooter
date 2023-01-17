using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using System.Linq;
using Debug = UnityEngine.Debug;

public class ChangeTheme : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string fmodEvent;

    private EventInstance _instance;

    private void Awake()
    {
        _instance = RuntimeManager.CreateInstance(fmodEvent);
        _instance.start();
    }
    public void ChangeMusic(string Music)
    {
        Debug.LogWarning(Music);

        if (Music == "Ambience")
        {
            _instance.setParameterByName("ToAmbience", 1);
            _instance.setParameterByName("ToGameMenu", 0);
            _instance.setParameterByName("ToButtleTheme", 0);
        }
        else if (Music == "BattleTheme")
        {
            _instance.setParameterByName("ToAmbience", 0);
            _instance.setParameterByName("ToGameMenu", 0);
            _instance.setParameterByName("ToButtleTheme", 1);
        }
        else if (Music == "GameMenu")
        {
            _instance.setParameterByName("ToAmbience", 0);
            _instance.setParameterByName("ToGameMenu", 1);
            _instance.setParameterByName("ToButtleTheme", 0);
        }

        float value = 1f;

        Debug.LogWarning(_instance.getParameterByName($"To{Music}", out value));

    }

}
