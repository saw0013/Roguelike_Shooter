using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Cosmoground/ManagerSession")]
[System.Serializable]
public class ManagerSessionSaved
{
    [SerializeField] private List<PlayerMovementAndLookNetwork> players;
    [SerializeField] private List<GameObject> CreatedObjectsInMatch;

    [SerializeField]
    public string NameManager
    {
        get => _nameManager;
        set => _nameManager = value;
    }

    private string _nameManager;
    /// <summary>
    /// ƒобавление пользователей в список комнаты
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        players.Add(player);
    }

    /// <summary>
    /// ”даление пользователей из списка комнаты
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        players.Remove(player);
    }

}
