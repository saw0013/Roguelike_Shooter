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
    /// ��� ��������� ������� ����� ������ ����� ��������� �������. ��������� � MatchID
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
    /// ������� ������� ���� ��������� � �������
    /// </summary>
    public List<GameObject> ObjectsWithMatch
    {
        get => CreatedObjectsInMatch == null ? CreatedObjectsInMatch : null;
    }

    #endregion

    #region Public Method

    /// <summary>
    /// ���������� ������� ������, ����� ����� ��������� ������� ��������� ���� �����
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
    /// ��������� ���� ������
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


    [Obsolete("������ ����������")]
    public void ChangeScore(PlayerMovementAndLookNetwork player, int score)
    {
        Debug.LogWarning("��������� ����");
        PlayerScore.ChangeScorePlayer(player, score);

    }

    /// <summary>
    /// ���������� ������������� � ������ �������
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        players.Add(player);
    }

    /// <summary>
    /// �������� ������������� �� ������ �������
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) return;

        players.Remove(player);
    }

    /// <summary>
    /// ��������� � ������ ������� ������� ������� ����� NetworkServer.Spawn
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
        //���� ������ ��������� ��� ������ ������, ������ ������
        if (_gameObject == null || CreatedObjectsInMatch == null) return;

        CreatedObjectsInMatch.Remove(_gameObject);
    }

    /// <summary>
    /// ���������� �����, ��� ����������. ���� �� ������������!!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_gameObject"></param>
    [Obsolete("����� ������� �� ���������, ��������� AddObjectWithMatch(GameObject)", true)]
    public void AddObjectWithMatch<T>(T _gameObject) where T : class
    {
        if (_gameObject != null)
            CreatedObjectsInMatch.Add(_gameObject as GameObject);
    }

    #endregion

}
