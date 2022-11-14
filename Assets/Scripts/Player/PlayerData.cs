using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _textHealth;

    [SerializeField] private float _maxHealth;

    public bool InputActive = true;
    public bool EscapeMenuActive;

    private float health;

    void Awake()
    {
        health = _maxHealth;
        _healthSlider.maxValue = _maxHealth / 100;
    }

    void Update()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health -= 10;
            ShowHP(health / 100);
        }
        // Test
    }

    public void ShowHP(float PlayerHp)
    {
        _healthSlider.DOValue(PlayerHp, Time.deltaTime * 20);
        _textHealth.text = $"{health}/{_maxHealth}";
    }
}
