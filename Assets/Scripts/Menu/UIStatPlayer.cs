using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStatPlayer : MonoBehaviour
{
    [Header("TextUI")]
    [SerializeField] private TMP_Text _ammoWatedText;
    [SerializeField] private TMP_Text _enemyKilledText;
    [SerializeField] private TMP_Text _buffPickText;
    [SerializeField] private TMP_Text _allScoreText;

    public void SetStatPlayerText(int ammoWasted, int enemyKilled, int buffPick, int allScore)
    {
        _ammoWatedText.text = $"Патронов потрачено : {ammoWasted}";
        _enemyKilledText.text = $"Убито врагов : {enemyKilled}";
        _buffPickText.text = $"Подобрано бонусов : {buffPick}";
        _allScoreText.text = $"Общий счет: {allScore}";
    }
}
