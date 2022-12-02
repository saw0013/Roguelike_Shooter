using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cosmo
{
    public class HealthController : NetworkBehaviour, IHealthController
    {
        #region Variables
        [Header("===UI===")]
        public Transform ItemsGrind;
        [SerializeField] protected Slider _healthSlider;
        [SerializeField] protected Slider _healthSliderRpc;
        [SerializeField] protected TMP_Text _textHealth;
        [SerializeField] protected TMP_Text _textGuard;
        [SerializeField] internal TMP_Text _nameDisplayRpc;

        [Space(5), Header("===HealthController===")]
        [SerializeField] protected bool _isDead;
        [SerializeField] protected float _currentHealth;
        public bool isImmortal = false;

        public bool fillHealthOnStart = true;
        public int maxHealth = 100;

        [SyncVar(hook = nameof(SyncHealth))]//ƒанные которые будем синхронизировать.
        public float _SyncHealth;

        [SerializeField] protected float _healthRecovery = 0;

        public float healthRecoveryDelay = 0f;

        [HideInInspector]
        protected float _currentHealthRecoveryDelay;

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
                    _isDead = true;
                    onDead.Invoke(gameObject);
                }
                else if (isDead && _currentHealth > 0)
                {
                    _isDead = false;
                }
            }
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

        [Space(10), Header("===Events==="), Tooltip("—обытие которое проверит ’ѕ")]
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

        //метод ’” ј который будет выставл€ть Health в соответствии с синхронизированным значением
        void SyncHealth(float oldvalue, float newValue)
        {
            //if (NetworkServer.active) return;
            currentHealth = newValue; //об€зательно делаем два значени€ - старое и новое. 
        }

        protected void ClientServerChangeHp(float hp)
        {
            if (isServer) ChangeHealthValue(hp);//если мы €вл€емс€ сервером, то переходим к непосредственному изменению переменной
            else CmdChangeHealth(hp); //в противном случае делаем на сервер запрос об изменении переменной
        }

        /// <summary>
        /// метод, который будет мен€ть переменную _SyncHealth. Ётот метод будет выполн€тьс€ только на сервере и мен€ть ’ѕ игрока
        /// </summary>
        /// <param name="newValue"></param>
        [Server]
        public void ChangeHealthValue(float newValue)
        {
            _SyncHealth = newValue;
        }

        /// <summary>
        /// Ѕудем мен€ть и синхронизировать ’ѕ
        /// </summary>
        /// <param name="newValue"></param>
        [Command] //обозначаем, что этот метод должен будет выполн€тьс€ на сервере по запросу клиента
        public void CmdChangeHealth(float newValue) //об€зательно ставим Cmd в начале названи€ метода
        {
            ChangeHealthValue(newValue);  //переходим к непосредственному изменению переменной
            RpcShowHP(newValue);
        }

        /// <summary>
        /// ќбновим дл€ всех клиентов HP над головой чтобы было видно
        /// </summary>
        /// <param name="PlayerHp"></param>
        [ClientRpc]
        void RpcShowHP(float PlayerHp) => _healthSliderRpc.DOValue(PlayerHp / 100, Time.deltaTime * 20);

        #region Ћокальные ’ѕ
        /// <summary>
        /// ќтобразим локальные ’ѕ в верхнем левом углу
        /// </summary>
        /// <param name="PlayerHp"></param>
        protected void LocalShowHP(float PlayerHp)
        {
            if (isLocalPlayer) //≈сли мы локальный игрок
            {
                //currentHealth = PlayerHp;
                _healthSlider.DOValue(PlayerHp / 100, Time.deltaTime * 20);
                _textHealth.text = $"{PlayerHp}/{MaxHealth}";
            }
        }
        #endregion

        #endregion

        #region Virtuals Instance Interface

        protected virtual void Awake()
        {
            _healthSlider.maxValue = MaxHealth / 100;
            _healthSliderRpc.maxValue = MaxHealth / 100;
        }

        protected virtual void Start()
        {
            if (fillHealthOnStart)
                currentHealth = maxHealth;
            currentHealthRecoveryDelay = healthRecoveryDelay;
        }

        protected virtual bool canRecoverHealth
        {
            get
            {
                return (currentHealth >= 0 && healthRecovery > 0 && currentHealth < maxHealth);
            }
        }

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
        /// ”величить или уменьшить currentHealth (положительные или отрицательные значени€)
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
        /// »зменить текущее здоровье персонажа
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
        /// —бросить текущее здоровье до определенного значени€
        /// </summary>
        /// <param name="health">target health</param>
        public virtual void ResetHealth(float health)
        {
            currentHealth = health;
            onResetHealth.Invoke();
            if (isDead) isDead = false;
        }
        /// <summary>
        /// —бросить текущее здоровье до максимального
        /// </summary>
        public virtual void ResetHealth()
        {
            currentHealth = maxHealth;
            onResetHealth.Invoke();
            if (isDead) isDead = false;
        }

        /// <summary>
        /// »зменить максимальное здоровье персонажа
        /// </summary>
        /// <param name="value"></param>
        public virtual void ChangeMaxHealth(int value)
        {
            maxHealth += value;
            if (maxHealth < 0)
                maxHealth = 0;
        }

        /// <summary>
        /// ”становите значение HealthRecovery, чтобы начать восстановление здоровь€.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetHealthRecovery(float value)
        {
            healthRecovery = value;
            StartCoroutine(RecoverHealth());
        }

        /// <summary>
        /// ѕрименить урон к текущему здоровью
        /// </summary>
        /// <param name="damage">damage</param>
        public virtual void TakeDamage(Damage damage)
        {
            if (hasAuthority)
            {
                if (damage != null)
                {
                    onStartReceiveDamage.Invoke(damage);
                    currentHealthRecoveryDelay = currentHealth <= 0 ? 0 : healthRecoveryDelay;

                    if (currentHealth > 0 && !isImmortal)
                    {
                        ClientServerChangeHp(currentHealth - damage.damageValue);
                        LocalShowHP(currentHealth - damage.damageValue);
                    }

                    if (damage.damageValue > 0)
                        onReceiveDamage.Invoke(damage);
                    HandleCheckHealthEvents();
                }
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
}
