using UnityEngine;
using Cosmo;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] protected OnReceiveDamage _onStartReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
    public OnReceiveDamage onStartReceiveDamage { get { return _onStartReceiveDamage; } protected set { _onStartReceiveDamage = value; } }
    public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }

    // Start is called before the first frame update
    public virtual void TakeDamage(Damage damage)
    {
        onStartReceiveDamage.Invoke(damage);
        onReceiveDamage.Invoke(damage);
    }

}
