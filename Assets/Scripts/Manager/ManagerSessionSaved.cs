using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cosmoground/ManagerSession")]
public class ManagerSessionSaved : ScriptableObject
{
    [SerializeField] public List<PlayerMovementAndLookNetwork> players;

    /// <summary>
    /// ƒобавление пользователей в список комнаты
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(PlayerMovementAndLookNetwork player)
    {
        if (players == null) players = new List<PlayerMovementAndLookNetwork>();
        players.Add(player);
    }

}
