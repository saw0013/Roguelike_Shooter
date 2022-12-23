using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManagerLogic
{
    public List<PlayerMovementAndLookNetwork> players;

    /// <summary>
    /// ��������� ������
    /// </summary>
    public int Difficult { get => players.Count; }

    public Guid MatchID
    {
        get => _matchid;
        set { _matchid = value; }
    }

    private Guid _matchid;

    /// <summary>
    /// ������� ������ � ������ ������� ������
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayerInGameManager(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        
        players.Add(player);
    }

    /// <summary>
    /// ������ ������ �� ����� ������� ������
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayerInGameManager(PlayerMovementAndLookNetwork player) => players.Remove(player);
}
