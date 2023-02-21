using Cosmoground;
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

            ((ShooterNetworkManager)NetworkManager.singleton).spawnPrefabs.ForEach
                (obj =>
                {
                    if (obj.tag == "Buffs")
                    {
                        var _obj = Object.Instantiate(obj);
                        SceneManager.MoveGameObjectToScene(_obj, scene);
                        NetworkServer.Spawn(_obj);
                    }

                });

            //Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));
            // GameObject reward = Object.Instantiate(((ShooterNetworkManager)NetworkManager.singleton).ItemHP);
            // SceneManager.MoveGameObjectToScene(reward, scene);
            // NetworkServer.Spawn(reward);
        }
    }
}