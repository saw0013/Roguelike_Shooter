using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameManagerLogic
{
    public List<PlayerMovementAndLookNetwork> players;
    public List<ManagerWave> Waves;

    /// <summary>
    /// �������� �������� ����
    /// </summary>
    public ManagerWave ActiveWave
    {
        get => Waves.FirstOrDefault(w => w.isActive);
    }
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

    public void AddWaveInGameManager(ManagerWave Wave)
    {
        if (Waves == null) Waves = new List<ManagerWave>();

        Waves.Add(Wave);
    }

    /// <summary>
    /// ������ ������ �� ����� ������� ������
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayerInGameManager(PlayerMovementAndLookNetwork player) => players.Remove(player);
}
