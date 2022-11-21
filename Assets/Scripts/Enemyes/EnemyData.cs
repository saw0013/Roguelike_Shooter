using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : EnemyBehaviour
{
    [Space(30)]
    [Header("Enemy Data"), Tooltip("Данные об враге")]
    [SerializeField] private float health;
    void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    public override void FieldOfViewCheck()
    {
        base.FieldOfViewCheck();
    }

    public override IEnumerator FOVRoutine()
    {
        return base.FOVRoutine();
    }
}
