using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyData : EnemyBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected OnReceiveDamage _onStartReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
    [SerializeField] protected OnDead _onDead = new OnDead();
    public OnReceiveDamage onStartReceiveDamage { get { return _onStartReceiveDamage; } protected set { _onStartReceiveDamage = value; } }
    public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }
    public OnDead onDead { get { return _onDead; } protected set { _onDead = value; } }

    private bool isDead = false;

    [Space(30)]
    [Header("Enemy Data"), Tooltip("Health")]
    [SerializeField] private TMP_Text _textHP;
    [SerializeField] private float _maxHealth = 150;
    [SerializeField] private Slider _healthSliderRpc;

    [SyncVar(hook = nameof(SyncHealth))]
    public float _SyncHealth;

    private float health;
    [SerializeField] private bool FillHealth = true;


    void Start()
    {
        if (FillHealth)
        {
            health = _maxHealth;
            _healthSliderRpc.maxValue = _maxHealth / 100;
            _healthSliderRpc.value = health / 100;
        }
        StartCoroutine(FOVRoutine());
        OnStart();
    }

    public override void Update()
    {
        base.Update();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorAttack;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorNormal;
    }


    #region Server Client Call ChangeHealth
    //����� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void ClientServerChangeHp(float hp)
    {
        if (isServer) ChangeHealthValue(hp);//���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
        else CmdChangeHealth(hp); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
    }
    //����� ������� ����� ���������� Health � ������������ � ������������������ ���������
    void SyncHealth(float oldvalue, float newValue) => health = newValue; //����������� ������ ��� �������� - ������ � �����. 

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
        RpcShowHP(newValue);
    }

    /// <summary>
    /// ������� ��� ���� �������� HP ��� ������� ����� ���� �����
    /// </summary>
    /// <param name="PlayerHp"></param>
    [ClientRpc]
    void RpcShowHP(float PlayerHp)
    {
        _healthSliderRpc.DOValue(PlayerHp / 100, Time.deltaTime * 20);
        _textHP.text = PlayerHp.ToString();
    }
    #endregion

    #region BASE class

    public override void FieldOfViewCheck()
    {
        base.FieldOfViewCheck();
    }

    public override IEnumerator FOVRoutine()
    {
        return base.FOVRoutine();
    }

    public override void OnStart() => base.OnStart();

    #endregion

    #region Virtual method

    public virtual void TakeDamage(Damage damage)
    {
        Debug.Log("����� ����� ������� ����");
        if (damage != null)
        {
            Debug.Log("���� �� null");
            onReceiveDamage.Invoke(damage);

            if (health > 0)
            {
                Debug.Log("���� 1");
                ClientServerChangeHp(health - damage.damageValue);
            }

            if (damage.damageValue > 0)
                onReceiveDamage.Invoke(damage);

            if (!isDead && health <= 0)
            {
                Debug.Log("���� 2");
                isDead = true;
                onDead.Invoke(gameObject);
            }
        }
    }

    #endregion
}
