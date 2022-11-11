using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using Mirror.Examples.NetworkRoom;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    internal class Spawner
    {
        /// <summary>
        /// Установка спавн
        /// </summary>
        /// <param name="scene"></param>
        internal static void InitialSpawn(Scene scene)
        {
            if (!NetworkServer.active) return;

            //for (int i = 0; i < 10; i++)
            SpawnReward(scene);
        }

        /// <summary>
        /// При спавне
        /// </summary>
        /// <param name="scene"></param>
        internal static void SpawnReward(Scene scene)
        {
            if (!NetworkServer.active) return;

            Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));
            GameObject cp = Object.Instantiate(((GameNetworkManager)NetworkManager.singleton).playerPrefab, spawnPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(cp, scene);
            NetworkServer.Spawn(cp);
            //GameObject reward = Object.Instantiate(((GameNetworkManager)NetworkManager.singleton).player, spawnPosition, Quaternion.identity);
            //SceneManager.MoveGameObjectToScene(reward, scene);
            //NetworkServer.Spawn(reward);
        }
    }
}