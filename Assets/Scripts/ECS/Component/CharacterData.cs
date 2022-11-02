using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Главный скрипт управления персонажа [Состояние персонажа]
/// </summary>
public class CharacterData : MonoBehaviour
{
    public List<MonoBehaviour> actionLevelUp;

    private int currentLevel = 1;
    private int score = 0;
    private int needScoreLevelUp = 20;

    private List<IItem> _items;

    public int CurrentLevel => currentLevel;

    public void Score(int scoreUp)
    {
        score += scoreUp;
        if (score >= needScoreLevelUp) LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        needScoreLevelUp *= 2;
        foreach (var action in actionLevelUp)
        {
            if (!(action is ILevelUp levelUp)) return;
            levelUp.LevelUp(this,currentLevel);
        }
    }
}
