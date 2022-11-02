using UnityEngine;

public class JerkAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float _jerkSpeed;
    [SerializeField] private float _delayJerkl;

    private float jerkTime = float.MinValue;

    public void Execude()
    {
        if (Time.time < jerkTime + _delayJerkl) return;

        jerkTime = Time.time;

        transform.localPosition += transform.forward * _jerkSpeed;
    }
}
