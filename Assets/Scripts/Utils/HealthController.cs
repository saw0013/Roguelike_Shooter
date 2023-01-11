using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cosmo;
using MirrorBasics;

public class HealthController : NetworkBehaviour, IHealthController
{
    #region Variables

    [SerializeField] private TypeController typeController;

    [Header("===UI===")]
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] protected Slider _healthDamageSlider;

    [SerializeField] protected Slider _healthSliderRpc;
    [SerializeField] protected Slider _healthDamageSliderRpc;

    [SerializeField] protected TMP_Text _textHealth;

    [Space(5), Header("===HealthController===")]
    [SerializeField] protected bool _isDead;
    [SerializeField] protected float _currentHealth;
    public bool isImmortal = false;

    public bool fillHealthOnStart = true;
    public int maxHealth = 100;

    [SyncVar(hook = nameof(SyncHealth))]//������ ������� ����� ����������������.
    public float _SyncHealth;

    [SerializeField] protected float _healthRecovery = 0;

    public float healthRecoveryDelay = 0f;

    [HideInInspector]
    protected float _currentHealthRecoveryDelay;

    [SerializeField] private bool _destroyAfterDie = false;

    [SerializeField, Range(0.1f, 10)] private float _delayDestroly = 3.0f;

    #region Properties
    public virtual int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        protected set
        {
            maxHealth = value;
        }
    }

    public virtual float currentHealth
    {
        get
        {
            return _currentHealth;
        }
        protected set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                onChangeHealth.Invoke(_currentHealth);
            }

            if (!_isDead && _currentHealth <= 0)
            {
                if(_destroyAfterDie) 
                {
                    Debug.LogWarning("������ ����");
                    StartCoroutine(DestroyAfterDie());
                    MatchMaker.ManagerLogic(GetComponent<NetworkMatch>().matchId).ActiveWave.SetKilledEnemy();
                }

                _isDead = true;

                onDead.Invoke(gameObject);
            }
            else if (isDead && _currentHealth > 0)
            {
                _isDead = false;
            }
        }
    }

    

    private IEnumerator DestroyAfterDie()
    {
        Debug.LogWarning("��������� �����������");
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }

    public virtual bool isDead
    {
        get
        {
            if (!_isDead && currentHealth <= 0)
            {
                _isDead = true;
                onDead.Invoke(gameObject);
            }
            return _isDead;
        }
        set
        {
            _isDead = value;
        }
    }

    public virtual float healthRecovery { get { return _healthRecovery; } set { _healthRecovery = value; } }

    public virtual float currentHealthRecoveryDelay { get { return _currentHealthRecoveryDelay; } set { _currentHealthRecoveryDelay = value; } }

    #endregion

    #region Events

    [Space(10), Header("===Events==="), Tooltip("������� ������� �������� ��")]
    public List<CheckHealthEvent> checkHealthEvents = new List<CheckHealthEvent>();
    [SerializeField] protected OnReceiveDamage _onStartReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnDead _onDead = new OnDead();
    public ValueChangedEvent onChangeHealth;

    public OnReceiveDamage onStartReceiveDamage { get { return _onStartReceiveDamage; } protected set { _onStartReceiveDamage = value; } }
    public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }
    public OnDead onDead { get { return _onDead; } protected set { _onDead = value; } }
    public UnityEvent onResetHealth;

    #endregion

    internal bool inHealthRecovery;

    #endregion

    #region Client\Server Callback

    //����� ���� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void SyncHealth(float oldvalue, float newValue)
    {
        currentHealth = newValue; //����������� ������ ��� �������� - ������ � �����. 

        //Rpc�����
        var _tweenOn = _healthSliderRpc.DOValue(newValue / 100, Time.deltaTime * 20);
        _tweenOn.onComplete = () => _healthDamageSliderRpc.DOValue(_healthSliderRpc.value, Time.deltaTime * 25);

        //�� �������������� � ���� ������ ��� ������
        if (typeController == TypeController.Enemy) _textHealth.text = $"{newValue}/{MaxHealth}";

        LocalShowHP(newValue);
    }

    protected void ClientServerChangeHp(float hp)
    {
        if (isServer) ChangeHealthValue(hp);//���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
        else CmdChangeHealth(hp); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
    }

    /// <summary>
    /// �����, ������� ����� ������ ���������� _SyncHealth. ���� ����� ����� ����������� ������ �� ������� � ������ �� ������
    /// </summary>
    /// <param name="newValue"></param>
    [Server]
    public void ChangeHealthValue(float newValue)
    {
        _SyncHealth = newValue;
    }

    /// <summary>
    /// ����� ������ � ���������������� ��
    /// </summary>
    /// <param name="newValue"></param>
    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(float newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue);  //��������� � ����������������� ��������� ����������
    }


    #region ��������� ��
    /// <summary>
    /// ��������� ��������� �� � ������� ����� ����
    /// </summary>
    /// <param name="PlayerHp"></param>
    protected void LocalShowHP(float PlayerHp)
    {
        if (isLocalPlayer) //���� �� ��������� �����
        {
            if (_healthSlider != null)
            {
                var _tweenOn = _healthSlider.DOValue(PlayerHp / 100, Time.deltaTime * 20);
                _tweenOn.onComplete = () => _healthDamageSlider.DOValue(_healthSlider.value, Time.deltaTime * 25);

                var _tweenColorChange = _textHealth.DOColor(Color.red, Time.deltaTime * 15);
                _tweenColorChange.onComplete = () => _textHealth.DOColor(Color.white, Time.deltaTime * 15);
            }

            _textHealth.text = $"{PlayerHp}/{MaxHealth}";
        }
    }
    #endregion

    #endregion

    #region Virtuals Instance Interface

    protected virtual void Awake()
    {
        if (_healthSlider != null)
            _healthSlider.maxValue = MaxHealth / 100;

        if (_healthDamageSlider != null)
            _healthDamageSlider.maxValue = MaxHealth / 100;

        _healthSliderRpc.maxValue = MaxHealth / 100;
        _healthSliderRpc.value = MaxHealth / 100;

        _healthDamageSliderRpc.maxValue = MaxHealth / 100;
        _healthDamageSliderRpc.value = MaxHealth / 100;

        _textHealth.text = $"{MaxHealth}/{MaxHealth}";
    }

    protected virtual void Start()
    {
        if (fillHealthOnStart)
            currentHealth = maxHealth;
        currentHealthRecoveryDelay = healthRecoveryDelay;

        _SyncHealth = MaxHealth;
    }


    protected virtual bool canRecoverHealth
    {
        get
        {
            return (currentHealth >= 0 && healthRecovery > 0 && currentHealth < maxHealth);
        }
    }

    #region Instance IHealthController
    protected virtual IEnumerator RecoverHealth()
    {
        inHealthRecovery = true;
        while (canRecoverHealth && !isDead)
        {
            HealthRecovery();
            yield return null;
        }
        inHealthRecovery = false;
    }

    protected virtual void HealthRecovery()
    {
        if (!canRecoverHealth || isDead) return;
        if (currentHealthRecoveryDelay > 0)
            currentHealthRecoveryDelay -= Time.deltaTime;
        else
        {
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            if (currentHealth < maxHealth)
                currentHealth += healthRecovery * Time.deltaTime;
        }
    }

    /// <summary>
    /// ��������� ��� ��������� currentHealth (������������� ��� ������������� ��������)
    /// </summary>
    /// <param name="value">Value to change</param>
    public virtual void AddHealth(int value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            onDead.Invoke(gameObject);
        }
        HandleCheckHealthEvents();
    }

    /// <summary>
    /// �������� ������� �������� ���������
    /// </summary>
    /// <param name="value"></param>
    public virtual void ChangeHealth(int value)
    {
        currentHealth = value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            onDead.Invoke(gameObject);
        }
        HandleCheckHealthEvents();
    }

    /// <summary>
    /// �������� ������� �������� �� ������������� ��������
    /// </summary>
    /// <param name="health">target health</param>
    public virtual void ResetHealth(float health)
    {
        Debug.LogWarning("Recovery Health");
        currentHealth = health;
        onResetHealth.Invoke();
        if (isDead) isDead = false;
    }
    /// <summary>
    /// �������� ������� �������� �� �������������
    /// </summary>
    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
        onResetHealth.Invoke();
        if (isDead) isDead = false;
    }

    /// <summary>
    /// �������� ������������ �������� ���������
    /// </summary>
    /// <param name="value"></param>
    public virtual void ChangeMaxHealth(int value)
    {
        maxHealth += value;
        if (maxHealth < 0)
            maxHealth = 0;
    }

    /// <summary>
    /// ���������� �������� HealthRecovery, ����� ������ �������������� ��������.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetHealthRecovery(float value)
    {
        healthRecovery = value;
        StartCoroutine(RecoverHealth());
    }

    #endregion

    /// <summary>
    /// ��������� ���� � �������� ��������
    /// </summary>
    /// <param name="damage">damage</param>
    public virtual void TakeDamage(Damage damage)
    {
        if (damage != null)
        {
            onStartReceiveDamage.Invoke(damage);
            currentHealthRecoveryDelay = currentHealth <= 0 ? 0 : healthRecoveryDelay;

            if (currentHealth > 0 && !isImmortal)
            {
                if (isServer) //���� �� ��������� ��������� ������ �� �������
                    _SyncHealth -= damage.damageValue;

                else if (isClient) //���� ��������� ��������� ������ �� �������
                {
                    if (hasAuthority) //���� �� � ��� ����� �������� ������
                    {
                        Debug.LogWarning("���� hasAuthority"); 
                        CmdChangeHealth(currentHealth - damage.damageValue);
                    }
                    else Debug.LogWarning("���� hasAuthority"); //���� ���� �� ���������
                }
            }


            if (damage.damageValue > 0)
                onReceiveDamage.Invoke(damage);
            HandleCheckHealthEvents();
        }
    }

   

    protected virtual void HandleCheckHealthEvents()
    {
        var events = checkHealthEvents.FindAll(e => (e.healthCompare == CheckHealthEvent.HealthCompare.Equals && currentHealth.Equals(e.healthToCheck)) ||
                                                    (e.healthCompare == CheckHealthEvent.HealthCompare.HigherThan && currentHealth > (e.healthToCheck)) ||
                                                    (e.healthCompare == CheckHealthEvent.HealthCompare.LessThan && currentHealth < (e.healthToCheck)));

        for (int i = 0; i < events.Count; i++)
        {
            events[i].OnCheckHealth.Invoke();
        }
        if (currentHealth < maxHealth && this.gameObject.activeInHierarchy && !inHealthRecovery)
            StartCoroutine(RecoverHealth());
    }

    enum TypeController
    {
        Player, Enemy
    }
    #endregion

    #region CLASS ChechHealthEvent
    [System.Serializable]
    public class CheckHealthEvent
    {
        public int healthToCheck;
        public bool disableEventOnCheck;

        public enum HealthCompare
        {
            Equals,
            HigherThan,
            LessThan
        }

        public HealthCompare healthCompare = HealthCompare.Equals;

        public UnityEngine.Events.UnityEvent OnCheckHealth;
    }

    [System.Serializable]
    public class ValueChangedEvent : UnityEvent<float>
    {

    }
    #endregion
}

