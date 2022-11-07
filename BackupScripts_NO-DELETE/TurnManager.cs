using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurnManager : MonoBehaviour
{
    private List<PlayerMovementAndLookNetwork> players = new List<PlayerMovementAndLookNetwork>();

    public void AddPlayers(PlayerMovementAndLookNetwork player) => players.Add(player);
}
