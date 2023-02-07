using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnThrowForce : MonoBehaviour
{
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody
       .AddForce(
       new Vector3(
           Random.Range(-transform.forward.x, transform.forward.x) * Random.Range(-7f, 7f),
           transform.up.y * Random.Range(5f, 8f),
           Random.Range(-transform.forward.x, transform.forward.x) * Random.Range(-7f, 7f)), ForceMode.Impulse);
    }

}
