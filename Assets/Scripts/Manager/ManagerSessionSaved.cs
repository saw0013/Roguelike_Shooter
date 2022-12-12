using System;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirrorBasics;
using UnityEngine;

//[CreateAssetMenu(menuName = "Cosmoground/ManagerSession")]
[System.Serializable]
public class ManagerSessionSaved
{
    #region Variables

    [SerializeField] private List<PlayerMovementAndLookNetwork> players;
    [SerializeField] private List<GameObject> CreatedObjectsInMatch;
    [SerializeField] private List<GameObject> checkPoints;

    [SerializeField] private Dictionary<PlayerMovementAndLookNetwork, int> countShoot;
    [SerializeField] public Dictionary<PlayerMovementAndLookNetwork, int> PlayerScore;

    #endregion

    #region Propertyes
    /// <summary>
    /// Имя менеджера которое будем искать чтобы добавлять объекты. Совпадает с MatchID
    /// </summary>
    [SerializeField]
    protected string NameManager
    {
        get => _nameManager;
        set => _nameManager = value;
    }

    public Guid MatchID
    {
        get => _matchid;
        set => _matchid = value;
    }

    private Guid _matchid;

    private string _nameManager;

    /// <summary>
    /// Объекты которые были добавлены в комнату
    /// </summary>
    public List<GameObject> ObjectsWithMatch
    {
        get => CreatedObjectsInMatch == null ? CreatedObjectsInMatch : null;
    }

    #endregion

    #region Public Method

    /// <summary>
    /// Записываем выстрел игрока, чтобы потом посчитать сколько выстрелов было всего
    /// </summary>
    /// <param name="player"></param>
    public void AddCountShoot(PlayerMovementAndLookNetwork player)
    {
        if (countShoot == null)
            countShoot = new Dictionary<PlayerMovementAndLookNetwork, int>();

        if (!countShoot.ContainsKey(player))
            countShoot.Add(player, 1);

        else countShoot[player] += 1;
    }

    public int GetShootCount(PlayerMovementAndLookNetwork player)
    {
        if (countShoot == null)
            return 0;

        if (!countShoot.ContainsKey(player)) return 0;
        else return countShoot[player];
    }

    /// <summary>
    /// Добавляет Очки игроку
    /// </summary>
    /// <param name="player"></param>
    /// <param name="score"></param>
    public void ChangeScorePlayer(PlayerMovementAndLookNetwork player, int score = 10)
    {
        if (PlayerScore == null)
            PlayerScore = new Dictionary<PlayerMovementAndLookNetwork, int>();

        if (!PlayerScore.ContainsKey(player))
            PlayerScore.Add(player, score);

        else PlayerScore[player] += score;

        MatchMaker.PlayerScoreChange(player);
    }


    [Obsolete("Глупое расширение")]
    public void ChangeScore(PlayerMovementAndLookNetwork player, int score)
    {
        Debug.LogWarning("Добавляем очко");
        PlayerScore.ChangeScorePlayer(player, score);

    }

    /// <summary>
    /// Добавление пользователей в список комнаты
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        players.Add(player);
    }

    /// <summary>
    /// Удаление пользователей из списка комнаты
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) return;

        players.Remove(player);
    }

    /// <summary>
    /// Добавляем в список объекты которые спавним через NetworkServer.Spawn
    /// </summary>
    /// <param name="_gameObject"></param>
    public void AddObjectWithMatch(GameObject _gameObject)
    {
        if (CreatedObjectsInMatch == null)
            CreatedObjectsInMatch = new List<GameObject>();

        CreatedObjectsInMatch.Add(_gameObject);
    }

    public void RemoveObjectWithMatch(GameObject _gameObject)
    {
        //Если пустая коллекция или пустой объект, просто выйдем
        if (_gameObject == null || CreatedObjectsInMatch == null) return;

        CreatedObjectsInMatch.Remove(_gameObject);
    }

    /// <summary>
    /// Обобщённый метод, для добавления. ПОКА НЕ ИСПОЛЬЗОВАТЬ!!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_gameObject"></param>
    [Obsolete("Метод доделан не полностью, используй AddObjectWithMatch(GameObject)", true)]
    public void AddObjectWithMatch<T>(T _gameObject) where T : class
    {
        if (_gameObject != null)
            CreatedObjectsInMatch.Add(_gameObject as GameObject);
    }

    #endregion

}
