using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public int Health = 100;
    public bool isDamage;

    public void Damage(int damage)
    {
        Health -= damage;
        isDamage = true;
    }
}
