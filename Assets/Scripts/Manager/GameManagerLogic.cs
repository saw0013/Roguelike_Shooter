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
    public List<EventTrigger> Door;

    /// <summary>
    /// Активный менеджер волн
    /// </summary>
    public ManagerWave ActiveWave
    {
        get => Waves.FirstOrDefault(w => w.isActive);
    }
    /// <summary>
    /// Сложность уровня
    /// </summary>
    public int Difficult { get => players.Count; }

    public Guid MatchID
    {
        get => _matchid;
        set { _matchid = value; }
    }

    private Guid _matchid;

    /// <summary>
    /// Добавим игрока в список игровой логики
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayerInGameManager(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        
        players.Add(player);
    }

    public void ActiveNextManagerWave()
    {
        var LastActiveindex = Waves.FindIndex(w => w.isActive); 

        Waves[LastActiveindex++].isActive = true;
    }

    public void AddWaveInGameManager(ManagerWave Wave)
    {
        if (Waves == null) Waves = new List<ManagerWave>();

        Waves.Add(Wave);
    }

    public void AddDoorInGameManager(EventTrigger door)
    {
        if (Door == null) Door = new List<EventTrigger>();

        Door.Add(door);
    }

    /// <summary>
    /// Удалим игрока из спика игровой логики
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayerInGameManager(PlayerMovementAndLookNetwork player) => players.Remove(player);
}
