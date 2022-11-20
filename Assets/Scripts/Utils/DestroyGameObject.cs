using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestroyGameObject : MonoBehaviour
{
    public float delay;
    public UnityEvent onDestroy;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        onDestroy.Invoke();
        Destroy(gameObject);
    }
}
