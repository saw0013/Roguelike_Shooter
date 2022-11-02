using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour, ILevelUp
{
    private CharacterHealth Health;

    private int levelNeed;

    public void LevelUp(CharacterData data, int level)
    {
        if (Health == null)
        {
            Health = FindObjectOfType<CharacterHealth>();
            if (Health == null) return;
        }

        if(data.CurrentLevel >= levelNeed)
        {
            Health.Health += 50;
            levelNeed += 2;
        }
    }
}
