using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public float clickForce = 1f;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    void OnSpawned()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter;

        if (plane.Raycast(ray, out enter))
        {
            var hitPoint = ray.GetPoint(enter);
            var mouseDir = hitPoint - gameObject.transform.position;
            mouseDir = mouseDir.normalized;
            GetComponent<Rigidbody>().AddForce(new Vector3(mouseDir.x,0, mouseDir.z) * clickForce);
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
