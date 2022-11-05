using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{

    public static NetworkPlayer Local { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Заспавнен наш игрок");
        }

        else Debug.Log("Заспавнен другой игрок");
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.HasInputAuthority) Runner.Despawn(Object);
    }
}
