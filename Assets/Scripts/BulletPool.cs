using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    void OnSpawned()
    {
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void OnDespawned()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        EZ_PoolManager.Despawn(other.transform);
    }
}
