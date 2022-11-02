using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityTarget : IAbility
{
    List<GameObject> Target { get; set; }
}
