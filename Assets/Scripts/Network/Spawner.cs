using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    private Vector3 GetRandomSpawnPoint()
    {
        //TODO: Сделать точки спавна и проверить занята ли точка спавна
        return new Vector3(0, 1, 0);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        if (runner.Topology == SimulationConfig.Topologies.Shared)
        {
            Debug.Log("OnConnectedToServer запускаем префаб игрока как локального игрока");
            runner.Spawn(playerPrefab, GetRandomSpawnPoint(), Quaternion.identity, runner.LocalPlayer);
        }

        else Debug.Log("OnConnectedToServer");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined мы сервер. Спавним игрока");
            runner.Spawn(playerPrefab, GetRandomSpawnPoint(), Quaternion.identity, runner.LocalPlayer);
        }
        else Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown"); ;
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnecteRequest");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
       
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
}
